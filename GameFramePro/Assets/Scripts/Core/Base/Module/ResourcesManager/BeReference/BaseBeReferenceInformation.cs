using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using UnityEngine.UI;


namespace GameFramePro
{
    /// <summary>/// 加载的资源被处理后真正引用的资源或者组件/// </summary>
    [System.Serializable]
    public class BaseBeReferenceInformation
    {
        /// <summary>///  被引用的资源的类型/// </summary>
        public Type ReferenceAssetType; // { get; protected set; }

        //被引用的次数
        public int ReferenceCount = 0; // { get; protected set; }

        //对外提供访问接口
        protected UnityEngine.Object mReferenceAsset = null;

        protected LoadAssetAssetRecord referenceAssetRecord { get; private set; } //源资源的记录


        #region 构造函数

        public BaseBeReferenceInformation()
        {
        }

        // 被真正引用的资源 可能是CurLoadAssetRecord 中资源的某个组件或者实例化的对象
        public BaseBeReferenceInformation(UnityEngine.Object asset, Type referenceType, LoadAssetAssetRecord assetRecord)
        {
            InitialedBeReferenceInformation(asset, referenceType, assetRecord);
        }

        public void InitialedBeReferenceInformation(UnityEngine.Object asset, Type referenceType, LoadAssetAssetRecord assetRecord)
        {
            ReferenceAssetType = referenceType;
            mReferenceAsset = asset;
            referenceAssetRecord = assetRecord;
        }

        #endregion

        #region 状态判断接口

        public virtual bool IsReferenceAssetEnable
        {
            get { return mReferenceAsset != null; }
        }

        public string AssetName
        {
            get { return IsReferenceAssetEnable ? mReferenceAsset.name : string.Empty; }
        }

        public bool IsNameEqual(string goName)
        {
            if (IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.name == goName;
        }

        //是否引用相同的资源
        public virtual bool IsReferenceEqual(UnityEngine.Object asset)
        {
            if (asset == null) return false;
            if (IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.Equals(asset);
        }

        public virtual bool IsReferenceEqual(ReferenceAssetAndRecord reference)
        {
            if (reference == null) return false;
            if (IsReferenceAssetEnable == false) return false;
            if (reference.BeReferenceAsset.IsReferenceAssetEnable == false) return false;
            return mReferenceAsset.Equals(reference.BeReferenceAsset.mReferenceAsset);
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
        public void ReduceReference(bool isForceDelete = false)
        {
            if (IsReferenceAssetEnable == false)
            {
                Debug.LogError("资源  已经被卸载了, 无法减少引用");
                return;
            }

            --ReferenceCount;
            if (isForceDelete)
                ReferenceCount = 0;

            if (ReferenceCount <= 0)
                ReleaseReference();
        }

        protected virtual void ReleaseReference()
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
                if (referenceAssetRecord != null)
                    referenceAssetRecord.AddReference();
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("SetAudioClip Fail,referenceAssetRecioRecord 没有赋值");
#endif
                }
            }
        }


        public void SetSprite(Image targetImage)
        {
            if (IsReferenceAssetEnable == false) return;
            if (mReferenceAsset is Sprite)
            {
                targetImage.sprite = mReferenceAsset as Sprite;
                AddReference();
                if (referenceAssetRecord != null)
                    referenceAssetRecord.AddReference();
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("SetAudioClip Fail,referenceAssetRecord 没有赋值");
#endif
                }
            }
        }

        #endregion
    }
}
