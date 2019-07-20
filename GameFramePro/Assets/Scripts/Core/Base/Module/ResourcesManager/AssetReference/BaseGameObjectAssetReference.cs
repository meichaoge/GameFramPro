using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{
    /// <summary>
    /// 特指被GameObject 引用的资源
    /// </summary>
    [System.Serializable]
    public abstract class BaseGameObjectAssetReference<T> : IAssetReference where T : Component
    {
        public ILoadAssetRecord CurLoadAssetRecord { get; protected set; }

        protected T TargetComponent { get; }

        #region   编辑器下显示数据

#if UNITY_EDITOR
        [SerializeField]
        private ResourcesLoadAssetRecord mResourcesLoadAssetRecord_Current;
        [SerializeField]
        private AssetBundleSubAssetLoadRecord mAssetBundleSubAssetLoadRecord_Current;

        public void UpdateView()
        {
            if (CurLoadAssetRecord != null)
            {
                if ((int)CurLoadAssetRecord.AssetLoadedType < 100)
                {
                    mResourcesLoadAssetRecord_Current = CurLoadAssetRecord as ResourcesLoadAssetRecord;
                    mAssetBundleSubAssetLoadRecord_Current = null;
                }
                else
                {
                    mAssetBundleSubAssetLoadRecord_Current = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
                    mResourcesLoadAssetRecord_Current = null;
                }
            }
            else
            {
                mResourcesLoadAssetRecord_Current = null;
                mAssetBundleSubAssetLoadRecord_Current = null;
            }
        }
#endif

        #endregion


        public abstract BaseGameObjectAssetReference<T> AttachComponentReference(T component, ILoadAssetRecord newAssetRecord, System.Action<T, ILoadAssetRecord> getAssetFromRecordAction);

        /// <summary>
        /// 当切使用Resources 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        /// <returns></returns>
        protected virtual    BaseGameObjectAssetReference<T> ModifyToResourcesAsset(T component, ILoadAssetRecord newAssetRecord, System.Action<T, ILoadAssetRecord> getAssetFromRecordAction)
        {
            ResourcesLoadAssetRecord modifyResourcesRecord = newAssetRecord as ResourcesLoadAssetRecord;

            if (CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从AssetBundle 资源{2}到Resouces资源{3}转换", component.gameObject.name, component.GetType(),
                    CurAssetBundleLoadAssetRecord.AssetUrl , modifyResourcesRecord.AssetUrl));
#endif
                CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = modifyResourcesRecord;
                getAssetFromRecordAction(component, modifyResourcesRecord);
                CurLoadAssetRecord.AddReference();
                return this;
            }//上一次使用的是AssetBundle 资源

            if (CurLoadAssetRecord.AssetUrl == newAssetRecord.AssetUrl)
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToResourcesAsset Success!! 赋值相同的资源 直接返回 ", CurLoadAssetRecord.AssetUrl);
#endif
                return this;
            } //资源没有改变


            CurLoadAssetRecord.ReduceReference();

            CurLoadAssetRecord = newAssetRecord;
            getAssetFromRecordAction(component, newAssetRecord);
            CurLoadAssetRecord.AddReference();
#if UNITY_EDITOR
            Debug.LogInfor("AttachComponentReference 修改了资源引用 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return this;
        }
        /// <summary>
        /// 当使用到AssetBundle 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        /// <returns></returns>
        protected virtual BaseGameObjectAssetReference<T> ModifyToAssetBundleAsset(T component, ILoadAssetRecord newAssetRecord, System.Action<T, ILoadAssetRecord> getAssetFromRecordAction)
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
                getAssetFromRecordAction(component, modifyAssetBundleRecord);
                CurLoadAssetRecord.AddReference();
                return this;
            }//上一次使用的是AssetBundle 资源


            var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;

            if (CurAssetBundleLoadAssetRecord.IsReferenceSameAsset(modifyAssetBundleRecord))
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToAssetBundleAsset Success!! 赋值相同的资源AssetBundle{0}  直接返回 ", CurAssetBundleLoadAssetRecord.AssetUrl);
#endif
                return this;
            } //资源没有改变


            CurLoadAssetRecord.ReduceReference();
            CurLoadAssetRecord = newAssetRecord;
            getAssetFromRecordAction(component, newAssetRecord);
            CurLoadAssetRecord.AddReference();
#if UNITY_EDITOR
            Debug.LogInfor("ModifyToAssetBundleAsset 修改了资源引用! 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return this;
        }

    }
}