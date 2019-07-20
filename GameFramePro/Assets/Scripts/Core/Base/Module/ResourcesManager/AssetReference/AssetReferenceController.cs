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
        public static void CreateOrAddReference<T>(T targetComponent, ILoadAssetRecord referenceAssetRecord, System.Func<LinkedList<IAssetReference>, IAssetReference> getAssetReference,System.Action<T, ILoadAssetRecord> getAssetFromRecordAction) where T : Component
        {
            AssetReferenceController referenceController = null;
            if(mAllGameObjectReferenceController.TryGetValue(targetComponent.gameObject,out referenceController)==false)
            {
                referenceController = targetComponent.gameObject.AddComponent<AssetReferenceController>();
                mAllGameObjectReferenceController[targetComponent.gameObject] = referenceController;
            }

            LinkedList<IAssetReference> assetReferencesLinkedList = null;
            if(referenceController.mAllComponentReferencesRecord.TryGetValue(targetComponent,out assetReferencesLinkedList)==false)
            {
                assetReferencesLinkedList = new LinkedList<IAssetReference>();
                referenceController.mAllComponentReferencesRecord[targetComponent] = assetReferencesLinkedList;
            }

            IAssetReference gameObjectReference = getAssetReference(assetReferencesLinkedList);
            Action<Component, ILoadAssetRecord> getAssetFromRecordAct = TranslateAction<T>(getAssetFromRecordAction);
            (gameObjectReference as BaseGameObjectAssetReference<T>).AttachComponentReference(targetComponent, referenceAssetRecord, getAssetFromRecordAct);
            referenceController.ModifyReference(targetComponent, gameObjectReference);
        }



        /// <summary>
        /// 转换回调类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getAssetFromRecordAction"></param>
        /// <returns></returns>
        private static Action<Component, ILoadAssetRecord> TranslateAction<T>(System.Action<T, ILoadAssetRecord> getAssetFromRecordAction) where T : Component
        {
            Action<Component, ILoadAssetRecord> getAssetFromRecordAct = null;
            if (getAssetFromRecordAction != null)
            {
                getAssetFromRecordAct = (component, record) =>
                {
                    getAssetFromRecordAction(component as T, record);
                };
            }//转换回调
            return getAssetFromRecordAct;
        }
        #endregion


        /// <summary>
        /// 当某个组件上资源引用关系改变时候
        /// </summary>
        /// <param name="targetComponent"></param>
        /// <param name="reference"></param>
        public void ModifyReference(Component targetComponent, IAssetReference reference)
        {
            LinkedList<IAssetReference> referenceRecords = null;
            if(mAllComponentReferencesRecord.TryGetValue(targetComponent,out referenceRecords)==false|| referenceRecords.Count==0)
            {
                if (referenceRecords == null)
                    referenceRecords = new LinkedList<IAssetReference>();
            }
            if (referenceRecords.Contains(reference) == false)
                referenceRecords.AddLast(reference);
#if UNITY_EDITOR
            UpdateDebugView();
#endif
        }





#if UNITY_EDITOR

        [Serializable]
        public class ComponentReferenceInfor
        {
            public Component mTargetComponent;
            public List<ResourcesLoadAssetRecord> mAllCurrentResourcesRecord = new List<ResourcesLoadAssetRecord>();
            public List<AssetBundleSubAssetLoadRecord> mAllCurrentAssetBundleRecord = new List<AssetBundleSubAssetLoadRecord>();

            public ComponentReferenceInfor() { }

            public ComponentReferenceInfor(Component component, LinkedList<IAssetReference> allReferences)
            {
                mTargetComponent = component;
                if(allReferences != null|| allReferences.Count != 0)
                {
                    var node = allReferences.First;
                    while (node!=null)
                    {
                        if (node.Value.CurLoadAssetRecord != null)
                        {
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