using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
    /// 加载的资源信息
    /// </summary>
    public interface ILoadAssetRecord
    {
        /// <summary>
        /// 加载时候Unity 生成的实例ID (不可靠)
        /// </summary>
        int InstanceID { get; }
        /// <summary>
        /// 唯一标示一个资源的，通常赋值为加载这个资源时候使用的相对路径(相对于Resouces 目录)
        /// </summary>
        string AssetUrl { get; }
        /// <summary>
        /// 引用次数
        /// </summary>
        int ReferenceCount { get; }



        /// <summary>
        /// 加载的方式和类型
        /// </summary>
        LoadedAssetTypeEnum AssetLoadedType { get; }

        /// <summary>
        /// 当引用为0 时候 还有多少秒后删除这个对象
        /// </summary>
        float RemainTimeToBeDelete { get; }

        /// <summary>
        /// 标记为要删除时候时间(单位毫秒)
        /// </summary>
        long MarkToDeleteTime { get;   }

        /// <summary>
        /// 加载的资源
        /// </summary>
        UnityEngine.Object TargetAsset { get; }
        /// <summary>
        /// 被哪个管理器管理
        /// </summary>
        IAssetManager BelongAssetManager { get; }





        /// <summary>
        /// 计数滴答
        /// </summary>
        /// <param name="tickTime"></param>
        /// <returns>返回当前对象是否有效</returns>
        bool TimeTick(float tickTime);

        /// <summary>
        /// 增加引用计数
        /// </summary>
        void AddReference();
        /// <summary>
        /// 较少引用计数
        /// </summary>
        /// <param name="isforceDelete"></param>
        void ReduceReference(bool isforceDelete = false);




        /// <summary>
        /// 引用次数为0时候的处理逻辑(这时候不一定要销毁自己，后台可以存活一定的时间)
        /// </summary>
        void NotifyNoReference();
        /// <summary>
        /// 后台时候被重新引用
        /// </summary>
        bool  NotifyReReference();

        /// <summary>
        /// 资源的引用次数改变时候
        /// </summary>
        void NotifyReferenceChange(); 



        /// <summary>
        /// 超过最大后台存活时间后释放自己
        /// </summary>
        void NotifyReleaseRecord();

    }
}