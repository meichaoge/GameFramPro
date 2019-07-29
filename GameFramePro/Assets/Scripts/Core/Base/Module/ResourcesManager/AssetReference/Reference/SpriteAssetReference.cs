using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using UnityEngine.UI;

namespace GameFramePro
{
    /// <summary>
    /// Sprite 资源被引用
    /// </summary>
    public class SpriteAssetReference <T>: BaseAssetReference<T> where T: Image
    {

        public override BaseAssetReference<T> AttachComponentReference(T component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<T> getAssetFromRecordAction)
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
                ReferenceAssetInfor = getAssetFromRecordAction(component, newAssetRecord);
                return this;
            }


            if (CurLoadAssetRecord == null)
            {
                CurLoadAssetRecord = newAssetRecord;
                CurLoadAssetRecord.AddReference();
                ReferenceAssetInfor = getAssetFromRecordAction(component, newAssetRecord);
                return this;
            }//第一次赋值

            if(newAssetRecord is ResourcesLoadAssetRecord)
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
        /// <param name="allSpriteAssetReference"></param>
        /// <returns></returns>
        public static IAssetReference GetSpriteAssetReference(T component, LinkedList<IAssetReference> allSpriteAssetReference) 
        {
            if (allSpriteAssetReference == null || allSpriteAssetReference.Count == 0)
                return new SpriteAssetReference<T>();

            int curReferenceInstanceID = -1;
            if (component != null && component.sprite != null)
                curReferenceInstanceID = component.sprite.GetInstanceID();
            if(curReferenceInstanceID==-1)
                return new SpriteAssetReference<T>(); //说明之前没有引用资源

            var targetNode = allSpriteAssetReference.First;
            while (targetNode!=null)
            {
                if (targetNode.Value.ReferenceAssetInfor.ReferenceInstanceID == curReferenceInstanceID)
                    return targetNode.Value;
                targetNode = targetNode.Next;
            }

            return new SpriteAssetReference<T>(); //没有找到匹配的引用资源
        }

        /// <summary>
        /// 默认的从 SpriteRenderer 组件中获取 sprite
        /// </summary>
        /// <param name="component"></param>
        /// <param name="assetRecord"></param>
        /// <returns>返回组件的实例ID .-1表示没有获取到.0 表示可能有地方没有赋值</returns>
        public static BaseBeReferenceAssetInfor GetSpriteFromSpriteRender(T component, BaseLoadAssetRecord assetRecord) 
        {
            BaseBeReferenceAssetInfor referenceAssetInfor = new BaseBeReferenceAssetInfor();

            referenceAssetInfor.ReferenceAssetType = typeof(Sprite);
            if (assetRecord == null || assetRecord.LoadUnityObjectAssetInfor == null)
            {
                //  Debug.LogErrorFormat("GetSpriteFromSpriteRender Fail,Parameter is null or Asset is null");
                component.sprite = null;
                referenceAssetInfor.ReferenceInstanceID = -1;
                return referenceAssetInfor;
            }

          

            Sprite sp = assetRecord.LoadUnityObjectAssetInfor.LoadSpriteFromSpriteRender();
            if (sp == null)
            {
                Debug.LogError("GetSpriteFromSpriteRender Fail, Record {0}", assetRecord.AssetUrl);
                component.sprite = null;
                referenceAssetInfor.ReferenceInstanceID = -1;
                return referenceAssetInfor;
            }

            component.sprite = sp;
            referenceAssetInfor.ReferenceAsset = component.sprite;
            referenceAssetInfor.ReferenceInstanceID = sp.GetInstanceID();
            return referenceAssetInfor;
        }


    }
}