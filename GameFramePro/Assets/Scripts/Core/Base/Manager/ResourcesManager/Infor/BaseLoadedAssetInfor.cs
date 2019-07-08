using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 资源加载时候的类型状态(Resources&LocalStore&AssetBundle) 后面可能对不同的类型进行处理
    /// </summary>
    public enum LoadedAssetTypeEnum
    {
        None = 0,  //未知的类型

        Resources_UnKnown = 1, //记录时候无法判断的类型
        Resources_Prefab,
        Resources_Sprite,
        Resources_Texture,
        Resources_TextAsset,
        Resources_Model,
        Resources_Material,
        Resources_Shader,

        //***本地存储的 可能是运行时记录的资源&网络下载资源&缓存资源
        LocalStore_UnKnown = 100, //记录时候无法判断的类型
        LocalStore_Prefab,
        LocalStore_Sprite,
        LocalStore_Texture,
        LocalStore_TextAsset,
        LocalStore_Model,
        LocalStore_Material,
        LocalStore_Shader,


        AssetBundle_UnKnown = 200, //记录时候无法判断的类型
        AssetBundle_Prefab,
        AssetBundle_Sprite,
        AssetBundle_Texture,
        AssetBundle_TextAsset,
        AssetBundle_Model,
        AssetBundle_Material,
        AssetBundle_Shader,

    }



    /// <summary>
    /// 加载的资源的基本信息
    /// </summary>
    public class BaseLoadedAssetInfor : IDisposable
    {
        public int mInstanceID;
        public string mAssetPath;
        public string mAssetName; //不带扩展名
        public int mReferenceCount; //引用次数
        public LoadedAssetTypeEnum mLoadedAssetTypeEnum = LoadedAssetTypeEnum.None;
        public Object mTargetAsset = null;
        public IAssetManager mAssetManager; //被哪个管理器管理
        public int RemainTimeToDelete { get; private set; } //当引用为0 时候 还有多少秒后删除这个对象
        public long MarkToDeleteTime { get; private set; } //标记为要删除时候时间(单位毫秒)


        public BaseLoadedAssetInfor(string assetPath, LoadedAssetTypeEnum typeEnum, Object asset, IAssetManager manager)
              : this(assetPath, System.IO.Path.GetFileNameWithoutExtension(assetPath), typeEnum, asset, manager)
        {

        }


        public BaseLoadedAssetInfor(string assetPath, string assetName, LoadedAssetTypeEnum typeEnum, Object asset, IAssetManager manager)

        {
            mAssetPath = assetPath;
            mAssetName = assetName;
            mInstanceID = asset.GetInstanceID();
            mReferenceCount = 1;
            mLoadedAssetTypeEnum = typeEnum;
            mAssetManager = manager;
        }

        public void AddReference()
        {
            if (mTargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0} Name={1} 已经被卸载了 无法增加引用", mAssetPath, mAssetName);
                return;
            }
            ++mReferenceCount;
        }
        /// <summary>
        /// 减少引用次数 
        /// </summary>
        /// <param name="isforceDelete">=true 时会强制删除这个对象</param>
        public void ReduceReference(bool isforceDelete = false)
        {
            if (isforceDelete)
            {
                mAssetManager.NotifyAssetForceDelete(this);
                this.Dispose();
                return;
            }

            if (mTargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0} Name={1} 已经被卸载了 无法减少加引用", mAssetPath, mAssetName);
                return;
            }

            --mReferenceCount;
            if (mReferenceCount == 0)
            {
                MarkToDeleteTime = System.DateTime.UtcNow.Second;
                mAssetManager.NotifyAssetNoReference(this);
            }
        }

        /// <summary>
        /// 每一秒被调用一次
        /// </summary>
        public void TimeTick()
        {
            --RemainTimeToDelete;
        }



        public void Dispose()
        {
            mTargetAsset = null;
            mAssetManager = null;
            RemainTimeToDelete = 0;
            mReferenceCount = 0;
        }


    }
}