using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 资源引用标识 (只在编辑器下显示)
    /// </summary>
    public class AssetReferenceTag : MonoBehaviour
    {
#if UNITY_EDITOR
        [System.Serializable]
        internal class ReferenceAssetInfor
        {
            public string mReferenceAssetTypeStr;
            public string mReferenceAssetName;
            public Component mComponentReference;
            public Object mReferenceAsset;


            #region 对象池

            private static NativeObjectPool<ReferenceAssetInfor> s_ReferenceAssetInforrPoolMgr;

            private static void OnBeforeGeReferenceAssetInfor(ReferenceAssetInfor record)
            {
            }

            private static void OnBeforeRecycleReferenceAssetInfor(ReferenceAssetInfor record)
            {
                if (record == null) return;
                record.mComponentReference = null;
                record.mReferenceAsset = null;
            }

            /// <summary>
            /// 获取 ReferenceAssetInfor 实例对象
            /// </summary>
            /// <returns></returns>
            public static ReferenceAssetInfor GetReferenceAssetInfor()
            {
                return s_ReferenceAssetInforrPoolMgr.GetItemFromPool();
            }

            /// <summary>
            /// 获取 ReferenceAssetInfor 实例对象
            /// </summary>
            /// <param name="assetBundleUri"></param>
            /// <param name="assetBundle"></param>
            /// <returns></returns>
            public static ReferenceAssetInfor GetReferenceAssetInfor(Component component, Type type, Object asset)
            {
                var assetBundleRecordInfor = s_ReferenceAssetInforrPoolMgr.GetItemFromPool();
                assetBundleRecordInfor.mComponentReference = component;
                assetBundleRecordInfor.mReferenceAssetTypeStr = type.Name;
                assetBundleRecordInfor.mReferenceAsset = asset;
                assetBundleRecordInfor.mReferenceAssetName = asset.name;
                return assetBundleRecordInfor;
            }

            /// <summary>
            /// 释放 ReferenceAssetInfor 对象
            /// </summary>
            /// <param name="assetBundleRecordInfor"></param>
            public static void ReleaseReferenceAssetInfor(ReferenceAssetInfor assetBundleRecordInfor)
            {
                s_ReferenceAssetInforrPoolMgr.RecycleItemToPool(assetBundleRecordInfor);
            }

            #endregion


            static ReferenceAssetInfor()
            {
                s_ReferenceAssetInforrPoolMgr = new NativeObjectPool<ReferenceAssetInfor>(50, OnBeforeGeReferenceAssetInfor, OnBeforeRecycleReferenceAssetInfor);
            }

            public ReferenceAssetInfor()
            {
            }
        }

        [SerializeField] internal List<ReferenceAssetInfor> mAllReferenceInfors = new List<ReferenceAssetInfor>();

        public void RecordReference(Component component, Type type, Object asset)
        {
            ReferenceAssetInfor referenceAssetInfor = ReferenceAssetInfor.GetReferenceAssetInfor(component, type, asset);
            mAllReferenceInfors.Add(referenceAssetInfor);
        }

        public void RomoveReference(Component component, Type type, Object asset)
        {
            if (component == null)
                return;


            for (int dex = mAllReferenceInfors.Count - 1; dex >= 0; dex--)
            {
                var referenceInfor = mAllReferenceInfors[dex];
                if (referenceInfor.mComponentReference == component && referenceInfor.mReferenceAsset == asset)
                {
                    mAllReferenceInfors.RemoveAt(dex);
                    return;
                }
            }

            //   Debug.LogError($"移除引用记录失败{component} {asset.name}");
        }
#endif


        private void OnDestroy()
        {
#if UNITY_EDITOR
            mAllReferenceInfors.Clear();
            Debug.LogEditorInfor($"被标记的资源{gameObject.name}  被销毁，立刻释放引用计数，并计时等待加载的资源被回收");
#endif
            ResourcesManager.ReduceGameObjectReference(gameObject);
        }
    }
}