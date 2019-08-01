﻿using System.Collections;
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
    public class ReferenceAssetAndRecord
    {
        public BaseLoadAssetRecord CurLoadAssetRecord { get; protected set; }

        //被真正引用的资源
        public ReferenceAssetInfor ReferenceAsset;

        #region 构造函数

        public ReferenceAssetAndRecord()
        {
        }

        public ReferenceAssetAndRecord(BaseLoadAssetRecord record, ReferenceAssetInfor referenceAsset)
        {
            CurLoadAssetRecord = record;
            ReferenceAsset = referenceAsset;
        }

        #endregion


        #region   编辑器下显示数据

#if UNITY_EDITOR
        [SerializeField] private ResourcesLoadAssetRecord Debug_ResourcesLoadAssetRecord_Current;
        [SerializeField] private AssetBundleSubAssetLoadRecord Debug_AssetBundleSubAssetLoadRecord_Current;

        public void UpdateView()
        {
            if (CurLoadAssetRecord != null)
            {
                if (CurLoadAssetRecord is ResourcesLoadAssetRecord)
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


        #region 引用计数接口

        /// <summary>
        /// 标示当前资源是否有效
        /// </summary>
        public bool IsReferenceEnable
        {
            get
            {
                if (CurLoadAssetRecord == null || ReferenceAsset == null)
                    return false;
                return CurLoadAssetRecord.IsReferenceEnable && ReferenceAsset.IsReferenceAssetEnable;
            }
        }

        public void AddReference()
        {
            if (IsReferenceEnable == false) return;
            CurLoadAssetRecord.AddReference();
            ReferenceAsset.AddReference();
        }

        public void ReduceReference(bool isforceDelete = false)
        {
            if (IsReferenceEnable == false) return;
            CurLoadAssetRecord.ReduceReference(isforceDelete);
            ReferenceAsset.ReduceReference(isforceDelete);
        }

        #endregion


        public virtual ReferenceAssetAndRecord ModifyComponentReference<T>(T component, ReferenceAssetAndRecord newAssetRecord, bool isAuotAddReference = true) where T : Component
        {
            if (newAssetRecord == null)
            {
                ReduceReference(); //释放当前引用的资源
                CurLoadAssetRecord = null;
                ReferenceAsset = null;
                return this;
            } //重定向引用空

            if (CurLoadAssetRecord == null)
            {
                CurLoadAssetRecord = newAssetRecord.CurLoadAssetRecord;
                ReferenceAsset = newAssetRecord.ReferenceAsset;
                if (isAuotAddReference)
                    AddReference();
                return this;
            } //第一次赋值

            if (newAssetRecord.CurLoadAssetRecord is ResourcesLoadAssetRecord)
                ModifyToResourcesAsset(component, newAssetRecord, isAuotAddReference);
            else if (newAssetRecord.CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
                ModifyToAssetBundleAsset(component, newAssetRecord, isAuotAddReference);
            else
                Debug.LogError("AttachComponentReference Fail,Not Define AssetType", newAssetRecord.GetType());

            return this;
        }

        /// <summary>
        /// 当切使用Resources 资源时候的操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newAssetRecord"></param>
        /// <param name="getAssetFromRecordAction"></param>
        protected virtual void ModifyToResourcesAsset<T>(T component, ReferenceAssetAndRecord newAssetRecord, bool isAuotAddReference = true) where T : Component
        {
            var modifyResourcesRecord = newAssetRecord.CurLoadAssetRecord as ResourcesLoadAssetRecord;

            if (CurLoadAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从AssetBundle 资源{2}到Resouces资源{3}转换", component.gameObject.name, component.GetType(),
                    CurAssetBundleLoadAssetRecord.AssetUrl, modifyResourcesRecord.AssetUrl));
#endif
                ReduceReference(); //释放当前引用资源

                CurLoadAssetRecord = modifyResourcesRecord;
                ReferenceAsset = newAssetRecord.ReferenceAsset;
                if (isAuotAddReference)
                    AddReference();
                return;
            } //上一次使用的是AssetBundle 资源

            if (CurLoadAssetRecord.AssetUrl == modifyResourcesRecord.AssetUrl)
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToResourcesAsset Success!! 赋值相同的资源 直接返回 ", CurLoadAssetRecord.AssetUrl);
#endif
                return;
            } //资源没有改变

            ReduceReference(); //释放当前引用资源
            CurLoadAssetRecord = modifyResourcesRecord;

            ReferenceAsset = newAssetRecord.ReferenceAsset;
            if (isAuotAddReference)
                AddReference();
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
        protected virtual void ModifyToAssetBundleAsset<T>(T component, ReferenceAssetAndRecord newAssetRecord, bool isAuotAddReference = true) where T : Component
        {
            var modifyAssetBundleRecord = newAssetRecord.CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;
            if (CurLoadAssetRecord is ResourcesLoadAssetRecord)
            {
                var CurResourcesLoadAssetRecord = CurLoadAssetRecord as ResourcesLoadAssetRecord;
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format(" 物体{0} 上 组件 {1} 从Resouces资源{2}到AssetBundle 资源{3}转换", component.gameObject.name, component.GetType(),
                    CurResourcesLoadAssetRecord.AssetUrl, modifyAssetBundleRecord.AssetUrl));
#endif
                ReduceReference(); //释放当前引用资源
                CurLoadAssetRecord = modifyAssetBundleRecord;
                ReferenceAsset = newAssetRecord.ReferenceAsset;
                if (isAuotAddReference)
                    AddReference();
                return;
            } //上一次使用的是AssetBundle 资源


            var CurAssetBundleLoadAssetRecord = CurLoadAssetRecord as AssetBundleSubAssetLoadRecord;

            if (CurAssetBundleLoadAssetRecord.IsReferenceSameAsset(modifyAssetBundleRecord))
            {
#if UNITY_EDITOR
                Debug.LogInfor("ModifyToAssetBundleAsset Success!! 赋值相同的资源AssetBundle{0}  直接返回 ", CurAssetBundleLoadAssetRecord.AssetUrl);
#endif
                return;
            } //资源没有改变

            ReduceReference(); //释放当前引用资源
            CurLoadAssetRecord = modifyAssetBundleRecord;
            ReferenceAsset = newAssetRecord.ReferenceAsset;
            if (isAuotAddReference)
                AddReference();

#if UNITY_EDITOR
            Debug.LogInfor("ModifyToAssetBundleAsset 修改了资源引用! 新的{0}", CurLoadAssetRecord.AssetUrl);
#endif
            return;
        }
    }
}