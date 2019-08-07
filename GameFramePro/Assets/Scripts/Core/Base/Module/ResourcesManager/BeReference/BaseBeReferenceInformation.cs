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
        #region 编辑器显示

#if UNITY_EDITOR
        [SerializeField] protected LoadAssetAssetRecord Debug_ParentReferenceAssetRecord;
        [SerializeField] protected string Debug_ReferenceAssetType;
        [SerializeField] protected int Debug_ReferenceCount;
        [SerializeField] protected UnityEngine.Object Debug_mReferenceAsset;

        public void UpdateDebugView()
        {
            Debug_ParentReferenceAssetRecord = ParentReferenceAssetRecord;
            Debug_ReferenceAssetType = ReferenceAssetType.Name;
            Debug_mReferenceAsset = mReferenceAsset;
            Debug_ReferenceCount = ReferenceCount;
        }

#endif

        #endregion


        /// <summary>///  被引用的资源的类型/// </summary>
        public Type ReferenceAssetType { get; protected set; }

        //被引用的次数
        public int ReferenceCount { get; protected set; } = 0;

        //对外只 提供访问接口
        protected UnityEngine.Object mReferenceAsset = null;

        /// <summary>/// 标示引用的资源是否有效/// </summary>
        public virtual bool IsReferenceAssetEnable
        {
            get { return mReferenceAsset != null; }
        }

        public string AssetName
        {
            get { return IsReferenceAssetEnable ? mReferenceAsset.name : string.Empty; }
        }

        public LoadAssetAssetRecord ParentReferenceAssetRecord { get; set; } //源资源的记录 (用于通知父级的)


        #region 构造函数

        public BaseBeReferenceInformation()
        {
        }


        public BaseBeReferenceInformation(BaseBeReferenceInformation sources)
        {
            if (sources != null)
                InitialedBeReferenceInformation(sources.mReferenceAsset, sources.ReferenceAssetType, sources.ParentReferenceAssetRecord);
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
            ParentReferenceAssetRecord = assetRecord;

            if (ParentReferenceAssetRecord == null)
                Debug.LogError("当前资源引用的源资源为null");
        }

        #endregion

        #region 状态判断接口

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

        /// <summary>/// 判断两个引用对象是否引用相同的资源/// </summary>
        public virtual bool IsReferenceEqual(BaseBeReferenceInformation reference)
        {
            if (reference == null) return false;
            if (IsReferenceAssetEnable == false && reference.IsReferenceAssetEnable == false)
                return true; //都是无效资源

            if (IsReferenceAssetEnable)
                return mReferenceAsset.Equals(reference.mReferenceAsset); //当前对象有效则判断对象是否是同一个
            return false;
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
            ParentReferenceAssetRecord.AddBeReferenceInformation(this);
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

            if (ParentReferenceAssetRecord != null)
                ParentReferenceAssetRecord.ReduceBeReferenceInformation(this);
#if UNITY_EDITOR
            else
            {
                Debug.LogError("SetAudioClip Fail,referenceAssetRecord 没有赋值");
            }
#endif
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

        /// <summary>/// 重新定义对象名/// </summary>
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
                ResourcesUtility.SettAudioClipFromReference(audioSource, this);
            }
        }


        public void SetSprite(Image targetImage)
        {
            if (IsReferenceAssetEnable == false) return;
            if (mReferenceAsset is Sprite)
            {
                ResourcesUtility.SetImageSpriteFromReference(targetImage, this);
            }
        }

        #endregion
    }
}
