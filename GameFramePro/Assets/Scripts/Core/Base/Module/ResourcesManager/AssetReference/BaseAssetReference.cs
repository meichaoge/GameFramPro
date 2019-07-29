using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{
    /// <summary>
    /// 定义了如何从参数 assetRecord 中获取组件component 需要的对象，并赋值给组件component ，返回引用的对象实例iD.如果返回0说明没有处理，-1表示组件没有引用这个记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <param name="assetRecord">定义了组件从哪里取数据，一般是用 BaseLoadAssetRecord 中定义的 TargetAsset 对象</param>
    /// <returns>返回引用的对象实例iD.如果返回0说明没有处理，-1表示组件没有引用这个记录</returns>
    public delegate BaseBeReferenceAssetInfor GetAssetFromRecordHandler<T>(T component, BaseLoadAssetRecord assetRecord) where T : Component;

    /// <summary>
    /// 定义了如何从参数 component 组件的所有引用其他资源链表 allComponentReferences 中获取满足条件的第一个引用信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <param name="allComponentReferences"> 组件的所有引用其他资源链表 </param>
    /// <returns></returns>
    public delegate IAssetReference GetCurReferenceHandler<T>(T component, LinkedList<IAssetReference> allComponentReferences) where T : Component;


    /// <summary>
    /// 特指被GameObject 引用的资源
    /// </summary>
    [System.Serializable]
    public abstract class BaseAssetReference<T> : IAssetReference where T : Component
    {
        #region IAssetReference接口实现

        public ILoadAssetRecord CurLoadAssetRecord { get; protected set; }
        public BaseBeReferenceAssetInfor ReferenceAssetInfor { get; protected set; }
       

        protected T TargetComponent { get; }


        #endregion

        #region   编辑器下显示数据

#if UNITY_EDITOR
        [SerializeField]
        private ResourcesLoadAssetRecord Debug_ResourcesLoadAssetRecord_Current;
        [SerializeField]
        private AssetBundleSubAssetLoadRecord Debug_AssetBundleSubAssetLoadRecord_Current;
        [SerializeField]
        private int Debug_ReferenceInstanceID;
        [SerializeField]
        private string Debug_ReferenceType;

        public void UpdateView()
        {
            Debug_ReferenceInstanceID = ReferenceAssetInfor.ReferenceInstanceID;
            Debug_ReferenceType = ReferenceAssetInfor.ReferenceAssetType != null ? ReferenceAssetInfor.ReferenceAssetType.ToString() : string.Empty;
            if (CurLoadAssetRecord != null)
            {
                if ((int)CurLoadAssetRecord.AssetLoadedType < 100)
                {
                    Debug_ResourcesLoadAssetRecord_Current = CurLoadAssetRecord as ResourcesLoadAssetRecord;
                    Debug_AssetBundleSubAssetLoadRecord_Current = null;
                }
                else
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


        public abstract BaseAssetReference<T> AttachComponentReference(T component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<T> getAssetFromRecordAction);

        /// <summary>
        /// 当切使用Resources 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        protected virtual void ModifyToResourcesAsset(T component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<T> getAssetFromRecordAction)
        {
            ResourcesLoadAssetRecord modifyResourcesRecord = newAssetRecord as ResourcesLoadAssetRecord;

            if (CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从AssetBundle 资源{2}到Resouces资源{3}转换", component.gameObject.name, component.GetType(),
                    CurAssetBundleLoadAssetRecord.AssetUrl, modifyResourcesRecord.AssetUrl));
#endif
                CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = modifyResourcesRecord;
                ReferenceAssetInfor =   getAssetFromRecordAction(component, modifyResourcesRecord);

                CurLoadAssetRecord.AddReference();
                return;
            }//上一次使用的是AssetBundle 资源

            if (CurLoadAssetRecord.AssetUrl == newAssetRecord.AssetUrl)
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToResourcesAsset Success!! 赋值相同的资源 直接返回 ", CurLoadAssetRecord.AssetUrl);
#endif
                return;
            } //资源没有改变


            CurLoadAssetRecord.ReduceReference();

            CurLoadAssetRecord = newAssetRecord;
            //    getAssetFromRecordAction(component, newAssetRecord,out this.mReferenceInstanceID,out this.mReferenceAssetType);
            ReferenceAssetInfor = getAssetFromRecordAction(component, newAssetRecord);

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
        protected virtual void ModifyToAssetBundleAsset(T component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<T> getAssetFromRecordAction)
        {
            AssetBundleSubAssetLoadRecord modifyAssetBundleRecord = newAssetRecord as AssetBundleSubAssetLoadRecord;
            if (CurLoadAssetRecord is ResourcesLoadAssetRecord)
            {
                var CurResourcesLoadAssetRecord = CurLoadAssetRecord as ResourcesLoadAssetRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从Resouces资源{2}到AssetBundle 资源{3}转换", component.gameObject.name, component.GetType(),
                    CurResourcesLoadAssetRecord.AssetUrl, modifyAssetBundleRecord.AssetUrl));
#endif
                CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = modifyAssetBundleRecord;
                ReferenceAssetInfor = getAssetFromRecordAction(component, modifyAssetBundleRecord);
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
            CurLoadAssetRecord = newAssetRecord;
            ReferenceAssetInfor = getAssetFromRecordAction(component, modifyAssetBundleRecord);
            CurLoadAssetRecord.AddReference();
#if UNITY_EDITOR
            Debug.LogInfor("ModifyToAssetBundleAsset 修改了资源引用! 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return;
        }

    }
}