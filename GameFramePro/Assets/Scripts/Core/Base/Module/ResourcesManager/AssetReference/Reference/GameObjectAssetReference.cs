using GameFramePro.ResourcesEx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 生成预制体实例 资源引用
    /// </summary>
    public class GameObjectAssetReference : BaseAssetReference
    {
        /// <summary>
        /// 生成的资源的实例
        /// </summary>
        public GameObject mTargetAssetInstance { get; protected set; }

        public override BaseAssetReference AttachComponentReference<Transform>(Transform component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<Transform> getAssetFromRecordAction)
        {
            if (getAssetFromRecordAction == null)
            {
                Debug.LogError("AttachComponentReference Fail,Parameter getAssetFromRecordAction is null");
                return null;
            }
            if (newAssetRecord == null)
            {
                if (CurLoadAssetRecord != null)
                    CurLoadAssetRecord.ReduceReference();
                CurLoadAssetRecord = newAssetRecord;
                ReferenceAssetInfor =    getAssetFromRecordAction(component, newAssetRecord);
                return this;
            }


            if (CurLoadAssetRecord == null)
            {
                CurLoadAssetRecord = newAssetRecord;
                CurLoadAssetRecord.AddReference();
                ReferenceAssetInfor = getAssetFromRecordAction(component, newAssetRecord);
                return this;
            }//第一次赋值

            if (newAssetRecord is ResourcesLoadAssetRecord)
            {
                ModifyToResourcesAsset(component, newAssetRecord, getAssetFromRecordAction);
            }
            else if (newAssetRecord is AssetBundleSubAssetLoadRecord)
            {
                ModifyToAssetBundleAsset(component, newAssetRecord, getAssetFromRecordAction);
            }
            else
            {
                Debug.LogError("AttachComponentReference Fail,Not Define AssetType", newAssetRecord.GetType());
            }
            return this;
        }


        /// <summary>
        /// 如何从一堆的数据中找到自己需要的数据(目前根据引用的实例ID 获取)
        /// </summary>
        /// <param name="allTransformAssetReference"></param>
        /// <returns></returns>
        public static IAssetReference GetTransformAssetReference(Transform component, LinkedList<IAssetReference> allTransformAssetReference)
        {
            //if (allTransformAssetReference == null || allTransformAssetReference.Count == 0)
            //    return new GameObjectAssetReference();

            //int curReferenceInstanceID = -1;
            //if (component != null && component.sprite != null)
            //    curReferenceInstanceID = component.sprite.GetInstanceID();
            //   if (curReferenceInstanceID == -1)
            return new GameObjectAssetReference(); //说明之前没有引用资源

            //var targetNode = allTransformAssetReference.First;
            //while (targetNode != null)
            //{
            //    if (targetNode.Value.ReferenceInstanceID == curReferenceInstanceID)
            //        return targetNode.Value;
            //    targetNode = targetNode.Next;
            //}

            //return new GameObjectAssetReference(); //没有找到匹配的引用资源
        }



        /// <summary>
        /// 默认的从 SpriteRenderer 组件中获取 sprite
        /// </summary>
        /// <param name="component"></param>
        /// <param name="assetRecord"></param>
        /// <returns>返回组件的实例ID .-1表示没有获取到.0 表示可能有地方没有赋值</returns>
        public static BaseBeReferenceAssetInfor GetGameObjectInstance(Transform targetParent, BaseLoadAssetRecord assetRecord)
        {
            BaseBeReferenceAssetInfor referenceAssetInfor = new BaseBeReferenceAssetInfor();
            referenceAssetInfor.ReferenceAssetType = typeof(GameObject);
            if (assetRecord == null || assetRecord.LoadUnityObjectAssetInfor == null)
            {
                //  Debug.LogErrorFormat("GetSpriteFromSpriteRender Fail,Parameter is null or Asset is null");
                targetParent = null;
                referenceAssetInfor.ReferenceInstanceID = -1;
                return referenceAssetInfor;
            }

            GameObject go = assetRecord.LoadUnityObjectAssetInfor.InstantiateInstance(targetParent);
            if (go == null)
            {
                Debug.LogError("GetGameObjectInstance Fail,Not GameObject Asset {0}", assetRecord.AssetUrl);
                referenceAssetInfor.ReferenceInstanceID = -1;
                return referenceAssetInfor;
            }
            referenceAssetInfor.ReferenceAsset = go;
            referenceAssetInfor.ReferenceInstanceID = go.GetInstanceID();
            return referenceAssetInfor;
        }


        public static BaseAssetReference2 GetGameObjectFromAssetReference2(Component component, List<BaseAssetReference2> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences.Count == 0) return null;
            Transform targetTransformComponent = component as Transform;

            if (targetTransformComponent == null || targetTransformComponent.childCount == 0)
                return null;
            if(otherParameter==null|| otherParameter.Length == 0)
            {
                Debug.LogError("GetGameObjectFromAssetReference2 至少需要传一个额外的 AssetUrl 参数");
                return null;
            }

            string assetName = otherParameter[0].ToString().GetFileNameWithoutExtensionEx();
            if(string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("GetGameObjectFromAssetReference2 至少需要传一个额外的 AssetUrl 参数,且能获取到正确的格式 {0}", otherParameter[0]);
                return null;
            }

            List<Transform> childTrans = targetTransformComponent.GetAllChildsTrans(true);

            foreach (var reference in allComponentReferences)
            {
                if (reference.ReferenceAssetInfor == null) continue;
                if (reference.ReferenceAssetInfor.ReferenceAsset == null) continue;
                if (reference.ReferenceAssetInfor.ReferenceAssetType != typeof(GameObject))
                    continue;

                GameObject go = reference.ReferenceAssetInfor.ReferenceAsset as GameObject;
                if (go == null)
                {
                    Debug.LogError("GetGameObjectFromAssetReference2 Fail!,当前资源 {0} 不是 GameObject ", reference.CurLoadAssetRecord.AssetUrl);
                    continue;
                }
                if (go.name != assetName) continue;

                if (childTrans.Contains(go.transform))
                    return reference;

            }
            return null;
        }

    }
}