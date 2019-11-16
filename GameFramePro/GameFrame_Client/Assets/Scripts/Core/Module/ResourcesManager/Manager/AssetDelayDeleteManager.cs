using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx.Reference
{
    /// <summary>
    /// 管理当资源的引用为0时候,继续等待一段时间，如果再起被启用则不销毁
    /// </summary>
    public sealed class AssetDelayDeleteManager : Single<AssetDelayDeleteManager>, IUpdateTimeTick
    {
        //按照剩余存在时间排序
        private static readonly LinkedList<ComponentReferenceAssetInfor> s_AllDelayDeleteAssetInfor = new LinkedList<ComponentReferenceAssetInfor>();


        #region IUpdateTick Interface

        protected float lastRecordTime = 0; //上一次记录的时间
        public float TickPerTimeInterval { get; private set; } = 5; //约等于5秒检测一次 

        public bool CheckIfNeedUpdateTick(float curTime)
        {
            if (lastRecordTime == 0f)
            {
                lastRecordTime = curTime;
                return true;
            }

            if (curTime - lastRecordTime >= TickPerTimeInterval)
                return true;

            return false;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick(currentTime) == false)
                return;
            UnLoadNoReferenceAssets(currentTime);
        }


        #endregion

        /// <summary>
        /// 删除没有被引用的资源
        /// </summary>
        public void UnLoadNoReferenceAssets(float currentTime)
        {
            float timeSpane = currentTime - lastRecordTime;
            lastRecordTime = currentTime;

            List<ComponentReferenceAssetInfor> allNoReferenceAssets = ReferenceAssetManager.GetAllNoReferenceAssetsForDelete();
            foreach (var noReferenceAsset in allNoReferenceAssets)
                s_AllDelayDeleteAssetInfor.AddLast(noReferenceAsset);
            if (s_AllDelayDeleteAssetInfor.Count == 0)
                return;
            allNoReferenceAssets.Clear();

            var target = s_AllDelayDeleteAssetInfor.First;
            var temp = target;
            while (target != null)
            {
                bool isCanRelease = target.Value.CheckIfCanRealRelease();
                temp = target.Next;
                if (isCanRelease)
                {
#if UNITY_EDITOR
                    Debug.LogEditorInfor($"某个资源{target.Value.ReferenceAssetUri}   没有引用后超过可以存活的时间 被释放了！！");
#endif
                    allNoReferenceAssets.Add(target.Value);
                    s_AllDelayDeleteAssetInfor.Remove(target);
                }

                target = temp;
            }
            ReferenceAssetManager.DeleteAllNoReferenceAssets(allNoReferenceAssets);
        }

    }
}
