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
    public class SpriteAssetReference : BaseGameObjectAssetReference<UnityEngine.UI.Image>
    {

        public override BaseGameObjectAssetReference<Image> AttachComponentReference(Image component, ILoadAssetRecord newAssetRecord, System.Action<Image, ILoadAssetRecord> getAssetFromRecordAction)
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
                getAssetFromRecordAction(component, newAssetRecord);
                return this;
            }




            if (CurLoadAssetRecord == null)
            {
                CurLoadAssetRecord = newAssetRecord;
                CurLoadAssetRecord.AddReference();
                getAssetFromRecordAction(component, newAssetRecord);
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
        /// 如何从一堆的数据中找到自己需要的数据
        /// </summary>
        /// <param name="allSpriteAssetReference"></param>
        /// <returns></returns>
        public static IAssetReference GetSpriteAssetReference(LinkedList<IAssetReference> allSpriteAssetReference)
        {
            if (allSpriteAssetReference == null || allSpriteAssetReference.Count == 0)
                return new SpriteAssetReference();
            return allSpriteAssetReference.First.Value ;
        }

        /// <summary>
        /// 默认的从 SpriteRenderer 组件中获取 sprite
        /// </summary>
        /// <param name="component"></param>
        /// <param name="assetRecord"></param>
        public static void GetSpriteFromSpriteRender(Image component, ILoadAssetRecord assetRecord)
        {
            if (assetRecord == null || assetRecord.TargetAsset == null)
            {
              //  Debug.LogErrorFormat("GetSpriteFromSpriteRender Fail,Parameter is null or Asset is null");
                component.sprite = null;
                return;
            }

            SpriteRenderer spriteRender = (assetRecord.TargetAsset as GameObject).GetComponent<SpriteRenderer>();
            if (spriteRender == null)
            {
                Debug.LogErrorFormat("GetSpriteFromSpriteRender Fail,Not Contain SpriteRenderer Component of Record {0}", assetRecord.AssetUrl);
                component.sprite = null;
                return;
            }
            component.sprite = spriteRender.sprite;
        }


    }
}