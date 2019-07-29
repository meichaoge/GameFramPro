using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 特指被GameObject 引用的资源
    /// </summary>
    [System.Serializable]
    public class BaseAssetReference2
    {
        #region IAssetReference接口实现

        public BaseLoadAssetRecord CurLoadAssetRecord { get; set; }

        public BaseBeReferenceAssetInfor ReferenceAssetInfor { get; set; }


        #endregion

        #region   编辑器下显示数据

#if UNITY_EDITOR
        [SerializeField]
        private ResourcesLoadAssetRecord Debug_ResourcesLoadAssetRecord_Current;
        [SerializeField]
        private AssetBundleSubAssetLoadRecord Debug_AssetBundleSubAssetLoadRecord_Current;
        [SerializeField]
        private BaseBeReferenceAssetInfor Debug_ReferenceAssetInfor;

       

        public void UpdateView()
        {
            Debug_ReferenceAssetInfor = ReferenceAssetInfor;

            if (CurLoadAssetRecord != null)
            {
                if(CurLoadAssetRecord is ResourcesLoadAssetRecord)
                {
                    Debug_ResourcesLoadAssetRecord_Current = CurLoadAssetRecord as ResourcesLoadAssetRecord;
                    Debug_AssetBundleSubAssetLoadRecord_Current = null;
                }

                if (CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
                {
                    Debug_AssetBundleSubAssetLoadRecord_Current = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
                    Debug_ResourcesLoadAssetRecord_Current = null;
                }
            }
            else
            {
                Debug_ResourcesLoadAssetRecord_Current = null;
                Debug_AssetBundleSubAssetLoadRecord_Current = null;
            }
        }
#endif

        #endregion


        public virtual BaseAssetReference2 ModifyComponentReference<T>(T component, BaseAssetReference2 newAssetRecord) where T : Component
        {
            if (newAssetRecord == null)
            {
                if (CurLoadAssetRecord != null)
                    CurLoadAssetRecord.ReduceReference();

                CurLoadAssetRecord = null;
                ReferenceAssetInfor = null;
                return this;
            }


            if (CurLoadAssetRecord == null)
            {
                CurLoadAssetRecord = newAssetRecord.CurLoadAssetRecord;
                CurLoadAssetRecord.AddReference();
                ReferenceAssetInfor = newAssetRecord.ReferenceAssetInfor;
                return this;
            }//第一次赋值

            if (newAssetRecord.CurLoadAssetRecord is ResourcesLoadAssetRecord)
            {
                ModifyToResourcesAsset(component, newAssetRecord);
            }
            else if (newAssetRecord.CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                ModifyToAssetBundleAsset(component, newAssetRecord);
            }
            else
            {
                Debug.LogError("AttachComponentReference Fail,Not Define AssetType", newAssetRecord.GetType());
            }
            return this;
        }


        /// <summary>
        /// 当切使用Resources 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        protected virtual void ModifyToResourcesAsset<T>(T component, BaseAssetReference2 newAssetRecord) where T : Component
        {
            ResourcesLoadAssetRecord modifyResourcesRecord = newAssetRecord.CurLoadAssetRecord as ResourcesLoadAssetRecord;

            if (CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从AssetBundle 资源{2}到Resouces资源{3}转换", component.gameObject.name, component.GetType(),
                    CurAssetBundleLoadAssetRecord.AssetUrl, modifyResourcesRecord.AssetUrl));
#endif
                CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = modifyResourcesRecord;
                ReferenceAssetInfor = newAssetRecord.ReferenceAssetInfor;

                CurLoadAssetRecord.AddReference();
                return;
            }//上一次使用的是AssetBundle 资源

            if (CurLoadAssetRecord.AssetUrl == modifyResourcesRecord.AssetUrl)
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToResourcesAsset Success!! 赋值相同的资源 直接返回 ", CurLoadAssetRecord.AssetUrl);
#endif
                return;
            } //资源没有改变


            CurLoadAssetRecord.ReduceReference();
            CurLoadAssetRecord = modifyResourcesRecord;
            //    getAssetFromRecordAction(component, newAssetRecord,out this.mReferenceInstanceID,out this.mReferenceAssetType);
            ReferenceAssetInfor = newAssetRecord.ReferenceAssetInfor;

            CurLoadAssetRecord.AddReference();
#if UNITY_EDITOR
            Debug.LogInfor("AttachComponentReference 修改了资源引用 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return;
        }
        /// <summary>
        /// 当使用到AssetBundle 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        protected virtual void ModifyToAssetBundleAsset<T>(T component, BaseAssetReference2 newAssetRecord) where T : Component
        {
            AssetBundleSubAssetLoadRecord modifyAssetBundleRecord = newAssetRecord.CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
            if (CurLoadAssetRecord is ResourcesLoadAssetRecord)
            {
                var CurResourcesLoadAssetRecord = CurLoadAssetRecord as ResourcesLoadAssetRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从Resouces资源{2}到AssetBundle 资源{3}转换", component.gameObject.name, component.GetType(),
                    CurResourcesLoadAssetRecord.AssetUrl, modifyAssetBundleRecord.AssetUrl));
#endif
                CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = modifyAssetBundleRecord;
                ReferenceAssetInfor = newAssetRecord.ReferenceAssetInfor;
                CurLoadAssetRecord.AddReference();
                return;
            }//上一次使用的是AssetBundle 资源


            var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;

            if (CurAssetBundleLoadAssetRecord.IsReferenceSameAsset(modifyAssetBundleRecord))
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToAssetBundleAsset Success!! 赋值相同的资源AssetBundle{0}  直接返回 ", CurAssetBundleLoadAssetRecord.AssetUrl);
#endif
                return;
            } //资源没有改变


            CurLoadAssetRecord.ReduceReference();
            CurLoadAssetRecord = modifyAssetBundleRecord;
            ReferenceAssetInfor = newAssetRecord.ReferenceAssetInfor;
            CurLoadAssetRecord.AddReference();
#if UNITY_EDITOR
            Debug.LogInfor("ModifyToAssetBundleAsset 修改了资源引用! 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return;
        }

    }
}