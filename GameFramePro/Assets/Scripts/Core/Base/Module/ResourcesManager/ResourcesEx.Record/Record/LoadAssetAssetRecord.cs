using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;


namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 通过AssetBundle  或者Resources 加载的真实资源 (非AssetBundle 资源)/// </summary>
    [System.Serializable]
    public class LoadAssetAssetRecord : LoadAssetBaseRecord
    {
        /// <summary>/// 加载到的资源信息/// </summary>
        public LoadBasicAssetInfor LoadBasicObjectAssetInfor { get; protected set; }
        
        
        
        

        #region 构造函数

        public LoadAssetAssetRecord()
        {
        }

        public LoadAssetAssetRecord(string assetUrl, LoadedAssetTypeEnum typeEnum, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            Initial(assetUrl, typeEnum, assetInformation, manager);
        }


        public void Initial(string assetUrl, LoadedAssetTypeEnum typeEnum, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            base.Initial(assetUrl, typeEnum, manager);
            LoadBasicObjectAssetInfor = assetInformation;
            ReferenceCount = 0;
        }

        #endregion


        #region 基类重写和实现

        public override bool IsReferenceEnable
        {
            get { return LoadBasicObjectAssetInfor != null ? LoadBasicObjectAssetInfor.IsLoadAssetEnable : false; }
        }

        protected int InstanceID
        {
            get { return IsReferenceEnable ? LoadBasicObjectAssetInfor.InstanceID : 0; }
        }

        /// <summary>/// 判断参数值指定的两个资源是否相同/// </summary>
        public virtual bool isReferenceEqual(LoadAssetAssetRecord record)
        {
            if (record == null) return false;
            if (record.LoadBasicObjectAssetInfor.IsLoadAssetEnable == false) return false;
            if (LoadBasicObjectAssetInfor.IsLoadAssetEnable == false) return false;
            if (record.LoadBasicObjectAssetInfor.LoadAssetType != LoadBasicObjectAssetInfor.LoadAssetType) return false;

            return record.LoadBasicObjectAssetInfor.AssetUrl == LoadBasicObjectAssetInfor.AssetUrl;
        }

   

        public override void NotifyReleaseRecord()
        {
            LoadBasicObjectAssetInfor.ReleaseAsset();
            base.NotifyReleaseRecord();
            LoadBasicObjectAssetInfor = null;
        }

      

        #endregion
    }
}
