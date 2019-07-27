using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{

    /// <summary>
    /// 每一个使用ResouceManager 加载资源并使用的对象都会有一个这个管理器，管理当前对象使用的所有资源
    /// 后期可能没有这个脚本剔除对 MonoBehaviour 的依赖(TODO)
    /// </summary>
    [DisallowMultipleComponent]
    public class AssetReferenceController : MonoBehaviour
    {
        //记录了每个gameobject 引用的资源信息
        public Dictionary<Component, LinkedList<IAssetReference>> mAllComponentReferencesRecord = new Dictionary<Component, LinkedList<IAssetReference>>();

        #region 创建和新增引用关系

        //记录所有挂在了 AssetReferenceController 的对象
        public static Dictionary<GameObject, AssetReferenceController> mAllGameObjectReferenceController = new Dictionary<GameObject, AssetReferenceController>();

        /// <summary>
        /// 对指定的组件增加或者更新引用关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetComponent"></param>
        /// <param name="referenceAssetRecord"></param>
        /// <param name="assetReferenceAct"></param>
        public static void CreateOrAddReference<T>(T targetComponent, ILoadAssetRecord referenceAssetRecord, GetCurReferenceHandler<T> getAssetReference, GetAssetFromRecordHandler<T> getAssetFromRecordAction, System.Action<UnityEngine.Object> AfterReferenceAction = null) where T : Component
        {
            AssetReferenceController referenceController = null;
            if (mAllGameObjectReferenceController.TryGetValue(targetComponent.gameObject, out referenceController) == false)
            {
                referenceController = targetComponent.gameObject.AddComponent<AssetReferenceController>();
                mAllGameObjectReferenceController[targetComponent.gameObject] = referenceController;
            }

            LinkedList<IAssetReference> assetReferencesLinkedList = null;
            if (referenceController.mAllComponentReferencesRecord.TryGetValue(targetComponent, out assetReferencesLinkedList) == false)
            {
                assetReferencesLinkedList = new LinkedList<IAssetReference>();
                referenceController.mAllComponentReferencesRecord[targetComponent] = assetReferencesLinkedList;
            }

            IAssetReference assetReference = getAssetReference(targetComponent, assetReferencesLinkedList);
            if (assetReference == null)
                return;

            (assetReference as BaseAssetReference<T>).AttachComponentReference(targetComponent, referenceAssetRecord, getAssetFromRecordAction);
            //   referenceController.ModifyReference(targetComponent, gameObjectReference);
            if (assetReferencesLinkedList.Contains(assetReference) == false)
                assetReferencesLinkedList.AddLast(assetReference);

            if (AfterReferenceAction != null)
                AfterReferenceAction(assetReference.ReferenceAssetInfor.ReferenceAsset);

#if UNITY_EDITOR
            referenceController.UpdateDebugView();
#endif
        }

        /// <summary>
        /// 获取指定组件上满足条件的第一个 引用的资源信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetComponent"></param>
        /// <param name="getAssetReference"></param>
        /// <returns></returns>
        public static IAssetReference GetAssetReference<T>(T targetComponent, GetCurReferenceHandler<T> getAssetReference) where T : Component
        {
            if (getAssetReference == null)
            {
                Debug.LogError("GetAssetReference Fail,parameter getAssetReference is null  ");
                return null;
            }

            AssetReferenceController referenceController = null;
            if (mAllGameObjectReferenceController.TryGetValue(targetComponent.gameObject, out referenceController))
            {
                LinkedList<IAssetReference> allAssetReferences = null;
                if (referenceController.mAllComponentReferencesRecord.TryGetValue(targetComponent, out allAssetReferences))
                {
                    return getAssetReference(targetComponent, allAssetReferences);
                }
            }
            return null;
        }

        #endregion


        private void OnDestroy()
        {
            OnRemoveAllReference();
        }

        public void OnRemoveAllReference()
        {
            Debug.LogInfor("OnRemoveAllReference");
            foreach (var referenceInfor in mAllComponentReferencesRecord)
            {
                if (referenceInfor.Value.Count == 0)
                    continue;
                var node = referenceInfor.Value.First;
                while (node != null)
                {
                    if (node.Value.CurLoadAssetRecord != null)
                        node.Value.CurLoadAssetRecord.ReduceReference();
                    node = node.Next;
                }
            }
            mAllComponentReferencesRecord.Clear();
        }



#if UNITY_EDITOR

        [Serializable]
        public class ComponentReferenceInfor
        {
            public Component mTargetComponent;

            public List<BaseBeReferenceAssetInfor> Debug_ReferenceDetail = new List<BaseBeReferenceAssetInfor>();
            public List<ResourcesLoadAssetRecord> mAllCurrentResourcesRecord = new List<ResourcesLoadAssetRecord>();
            public List<AssetBundleSubAssetLoadRecord> mAllCurrentAssetBundleRecord = new List<AssetBundleSubAssetLoadRecord>();

            public ComponentReferenceInfor() { }

            public ComponentReferenceInfor(Component component, LinkedList<IAssetReference> allReferences)
            {
                mTargetComponent = component;
                if (allReferences != null || allReferences.Count != 0)
                {
                    var node = allReferences.First;
                    while (node != null)
                    {
                        if (node.Value.CurLoadAssetRecord != null)
                        {
                            if (node.Value.ReferenceAssetInfor.ReferenceInstanceID == 0)
                            {
                                Debug.LogErrorFormat("没有赋值的实例ID " + node.Value.CurLoadAssetRecord);
                            }

                            BaseBeReferenceAssetInfor detailInfor = new BaseBeReferenceAssetInfor();
                            detailInfor.ReferenceInstanceID = node.Value.ReferenceAssetInfor.ReferenceInstanceID;
                            detailInfor.ReferenceAsset = node.Value.ReferenceAssetInfor.ReferenceAsset;
                            if (node.Value.ReferenceAssetInfor.ReferenceAssetType != null)
                                detailInfor.ReferenceAssetType = node.Value.ReferenceAssetInfor.ReferenceAssetType;
                            Debug_ReferenceDetail.Add(detailInfor);



                            if (node.Value.CurLoadAssetRecord is ResourcesLoadAssetRecord)
                                mAllCurrentResourcesRecord.Add(node.Value.CurLoadAssetRecord as ResourcesLoadAssetRecord);
                            else
                                mAllCurrentAssetBundleRecord.Add(node.Value.CurLoadAssetRecord as AssetBundleSubAssetLoadRecord);
                        }
                        node = node.Next;
                    }
                }
            }
        }
        [SerializeField]
        private List<ComponentReferenceInfor> mAllComponentReferenceInfor = new List<ComponentReferenceInfor>();

        private void UpdateDebugView()
        {
            mAllComponentReferenceInfor.Clear();
            foreach (var item in mAllComponentReferencesRecord)
            {
                mAllComponentReferenceInfor.Add(new ComponentReferenceInfor(item.Key, item.Value));
            }
        }

#endif




    }
}