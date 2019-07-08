using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 负责辅助生成 UnityMonoObjectPool 
    /// </summary>
    public class UnityMonoObjectPoolHelper : Single_Mono_AutoCreateNotDestroy<UnityMonoObjectPoolHelper>
    {
        private readonly string R_UnityPoolManagerPrefix = "UnityPoolManger_"; //Unity 对象池父节点名称前缀
        private Dictionary<string, Transform> mAllPoolMonoObjects = new Dictionary<string, Transform>();


        /// <summary>
        /// 获取Unity 对象池管理器所属的Hierachy 父节点
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public Transform GetUnityPoolManagerTransParent(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("GetUnityPoolManagerTransParent Fail,Parameter poolName is null");
                return null;
            }
            poolName = string.Format("{0}{1}", R_UnityPoolManagerPrefix, poolName);
            Transform parent = null;
            if (mAllPoolMonoObjects.TryGetValue(poolName, out parent))
            {
                if (parent != null)
                    return parent;
            }

            parent = ResourcesManager.Instantiate(poolName).transform;
            parent.SetParent(transform, false);
            mAllPoolMonoObjects[poolName] = parent;
            return parent;
        }

        /// <summary>
        /// 销毁对象池时候回收父节点
        /// </summary>
        /// <param name="poolName"></param>

        public void RecycleUnityPoolManagerTransParent(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("RecycleUnityPoolManagerTransParents Fail,Parameter poolName is null");
                return;
            }
            poolName = string.Format("{0}{1}", R_UnityPoolManagerPrefix, poolName);
            Transform parent = null;
            if (mAllPoolMonoObjects.TryGetValue(poolName, out parent))
            {
                if (parent != null)
                {
                    mAllPoolMonoObjects.Remove(poolName);
                    ResourcesManager.Destroy(parent.gameObject);
                    return;
                }
            }

            Debug.LogError(string.Format("RecycleUnityPoolManagerTransParent Fail,the poolName={0} connect Transform Not Record Or Aleady Destroyed", poolName));
            return;
        }

    }
}