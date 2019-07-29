using System.Collections;
using System.Collections.Generic;
using GameFramePro.ResourcesEx;
using UnityEngine;



namespace GameFramePro
{
    /// <summary>
    /// 声音资源加载器
    /// </summary>
    public class AudioClipAssetReference : BaseAssetReference
    {
        public override BaseAssetReference AttachComponentReference<AudioSource>(AudioSource component, BaseLoadAssetRecord newAssetRecord, GetAssetFromRecordHandler<AudioSource> getAssetFromRecordAction)
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
        /// <param name="allClipAssetReference"></param>
        /// <returns></returns>
        public static IAssetReference GetAudioClipAssetReference(AudioSource component, LinkedList<IAssetReference> allClipAssetReference)
        {
            if (allClipAssetReference == null || allClipAssetReference.Count == 0)
                return new AudioClipAssetReference();

            int curReferenceInstanceID = -1;
            if (component != null && component.clip != null)
                curReferenceInstanceID = component.clip.GetInstanceID();
            if (curReferenceInstanceID == -1)
                return new AudioClipAssetReference(); //说明之前没有引用资源

            var targetNode = allClipAssetReference.First;
            while (targetNode != null)
            {
                if (targetNode.Value.ReferenceAssetInfor.ReferenceInstanceID == curReferenceInstanceID)
                    return targetNode.Value;
                targetNode = targetNode.Next;
            }

            return new AudioClipAssetReference(); //没有找到匹配的引用资源
        }

        /// <summary>
        ///获取声音资源
        /// </summary>
        /// <param name="component"></param>
        /// <param name="assetRecord"></param>
        /// <returns>返回组件的实例ID .-1表示没有获取到.0 表示可能有地方没有赋值</returns>
        public static BaseBeReferenceAssetInfor GetAudioClipFromAsset(AudioSource component, BaseLoadAssetRecord assetRecord)
        {
            BaseBeReferenceAssetInfor referenceAssetInfor = new BaseBeReferenceAssetInfor();

            //referenceAssetInfor.ReferenceAssetType = typeof(AudioClip);
            //if (assetRecord == null || assetRecord.TargetAsset == null)
            //{
            //    //  Debug.LogErrorFormat("GetSpriteFromSpriteRender Fail,Parameter is null or Asset is null");
            //    component.clip = null;
            //    referenceAssetInfor.ReferenceInstanceID = -1;
            //    return referenceAssetInfor;
            //}

            //AudioClip audioClip = (assetRecord.TargetAsset as AudioClip);
            //if (audioClip == null)
            //{
            //    Debug.LogError("GetAudioClipFromAsset Fail,Not  AudioClip Record {0}", assetRecord.AssetUrl);
            //    component.clip = null;
            //    referenceAssetInfor.ReferenceInstanceID = -1;
            //    return referenceAssetInfor;
            //}
            //component.clip = audioClip;
            //referenceAssetInfor.ReferenceAsset = component.clip;
            //referenceAssetInfor.ReferenceInstanceID = audioClip.GetInstanceID();
            return referenceAssetInfor;
        }




    }
}