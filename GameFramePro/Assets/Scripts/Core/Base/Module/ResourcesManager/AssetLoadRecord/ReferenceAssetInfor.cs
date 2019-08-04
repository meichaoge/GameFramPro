using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>/// 加载的资源被处理后真正引用的资源或者组件/// </summary>
    [System.Serializable]
    public class ReferenceAssetInfor
    {
        /// <summary>///  被引用的资源的类型/// </summary>
        public Type ReferenceAssetType;// { get; protected set; }

        //被引用的次数
        public int ReferenceCount = 0;// { get; protected set; }

        //对外提供访问接口
        protected UnityEngine.Object mReferenceAsset = null;


        #region 构造函数

        public ReferenceAssetInfor() { }
        // 被真正引用的资源 可能是CurLoadAssetRecord 中资源的某个组件或者实例化的对象
        public ReferenceAssetInfor(UnityEngine.Object asset, Type referenceType)
        {
            ReferenceAssetType = referenceType;
            mReferenceAsset = asset;
        }

        #endregion

        #region 状态判断接口

        public virtual bool IsReferenceAssetEnable { get { return mReferenceAsset != null; } }
        public string AssetName { get { if (IsReferenceAssetEnable) return mReferenceAsset.name;return string.Empty; } }
        public  bool IsNameEqual(string goName)
        {
            if (IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.name == goName;
        }
        //是否引用相同的资源
        public virtual bool  IsReferenceEqual(UnityEngine.Object asset)
        {
            if (asset == null) return false;
            if (IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.Equals(asset);
        }
        public virtual bool IsReferenceEqual(ReferenceAssetAndRecord reference)
        {
            if (reference == null) return false;
            if (IsReferenceAssetEnable == false) return false;
            if (reference.ReferenceAsset.IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.Equals(reference.ReferenceAsset.mReferenceAsset);
        }
        #endregion

        #region 当前对象引用操作
        
        
        //增加引用
        public void AddReference()
        {
            if (IsReferenceAssetEnable == false)
            {
                Debug.LogError("资源  已经被卸载了, 无法增加引用");
                return;
            }
            ++ReferenceCount;
        }
        //减少引用
        public virtual void ReduceReference(bool isforceDelete = false)
        {
            if (IsReferenceAssetEnable == false)
            {
                Debug.LogError("资源  已经被卸载了, 无法减少引用");
                return;
            }
            --ReferenceCount;
            if (isforceDelete)
                ReferenceCount = 0;

            if (ReferenceCount <= 0)
                ReleaseReference();
    }


        public virtual void ReleaseReference()
        {
            if (mReferenceAsset == null) return;
            ResourcesManager.UnLoadAsset(mReferenceAsset);
            mReferenceAsset = null;
        }



        #endregion


        #region 资源修改接口

        /// <summary>
        /// 重新定义对象名
        /// </summary>
        /// <param name="newGameObjectName"></param>
        public void ModifyGameObjectName(string newGameObjectName)
        {
            if (IsReferenceAssetEnable == false) return;
            mReferenceAsset.name = newGameObjectName;
        }


        #endregion

        #region 资源设置接口
        public void SetAudioClip(AudioSource audioSource)
        {
            if (IsReferenceAssetEnable == false) return;
            if (mReferenceAsset is AudioClip)
            {
                audioSource.clip = mReferenceAsset as AudioClip;
                AddReference();
            }
        }
        #endregion

    }
}