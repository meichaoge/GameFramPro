using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>/// 被引用的资源是实例化的GameObject 对象/// </summary>
    [System.Serializable]
    public class ReferenceGameObjectAssetInfor : ReferenceAssetInfor
    {

        protected GameObject mReferenceGameObjectAsset = null;

        public override bool IsReferenceAssetEnable { get { return mReferenceGameObjectAsset != null ? base.IsReferenceAssetEnable : false; } }
      
        //标示当前物体使用激活状态
        public bool IsActivity { get { return IsReferenceAssetEnable && mReferenceGameObjectAsset.activeSelf; } }

        protected Transform transform { get { return IsReferenceAssetEnable ? mReferenceGameObjectAsset.transform : null; } }

        #region 构造函数

        public ReferenceGameObjectAssetInfor() { }
        // 被真正引用的资源 可能是CurLoadAssetRecord 中资源的某个组件或者实例化的对象
        public ReferenceGameObjectAssetInfor(UnityEngine.Object asset, Type referenceType) : base(asset, referenceType)
        {
            if (referenceType != typeof(GameObject))
            {
                mReferenceGameObjectAsset = null;
                Debug.LogError("当前资源不是 GameObject");
            }
            else
            {
                mReferenceGameObjectAsset = asset as GameObject;
            }
        }

        #endregion

        #region 基类重写
        public override void ReleaseReference()
        {
            if (IsReferenceAssetEnable == false) return;
          //  ResourcesManager.UnLoadAsset(mReferenceAsset);  //GameObject  不能调用Unload
            ResourcesManager.Destroy(mReferenceGameObjectAsset);
            mReferenceGameObjectAsset = null;
        }
        #endregion


        #region 封装GameObject 各种需要的接口 (最好不要直接返回对象的引用)

        public T GetComponent<T>() where T : Component
        {
            if (IsReferenceAssetEnable == false) return null;
            return mReferenceGameObjectAsset.GetComponent<T>();
        }

        public T GetAddComponent<T>() where T : Component
        {
            if (IsReferenceAssetEnable == false) return null;
            return mReferenceGameObjectAsset.GetAddComponentEx<T>();
        }
        public T[] GetComponentsInChildren<T>(bool includeInactive)
        {
            if (IsReferenceAssetEnable == false) return null;
            return mReferenceGameObjectAsset.GetComponentsInChildren<T>();
        }

        //根据路径找到子节点然后返回指定的组件
        public T FindChildComponentByPath<T>(string path) where T : Component
        {
            if (IsReferenceAssetEnable == false) return null;
            Transform child = transform.Find(path);
            if (child == null) return null;
            return child.GetComponent<T>();
        }

        public string GetTransRelativePathToThis(Transform childTrans)
        {
            if (childTrans == null)
            {
                Debug.LogError("GetTransRelativePathToThis Fail,Parameter is null");
                return string.Empty;
            }
            if (IsReferenceAssetEnable == false) return null;
            return childTrans.GetTransRelativePathToRoot(transform);
        }

        //是否是子节点
        public bool IsChild(Transform parent)
        {
            if (IsReferenceAssetEnable == false) return false;
            return transform.parent == parent;
        }


        //设置显示或者隐藏
        public void SetActive(bool isActivity)
        {
            if (IsReferenceAssetEnable == false) return;
            mReferenceGameObjectAsset.SetActive(isActivity);
        }

        #endregion

        #region 设置相关的Transform 属性

        public void SetLocalPosition(Vector3 localPos)
        {
            if (IsReferenceAssetEnable == false) return;
            transform.localPosition = localPos;
        }

        #endregion

    }
}