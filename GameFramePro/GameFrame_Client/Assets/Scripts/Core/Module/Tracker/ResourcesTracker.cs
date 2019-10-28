using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    [System.Flags]
    //资源追踪状态
    public enum TraceResourcesStateEnum
    {
        Normal = 0, //普通
        Singtion = 1, //单例
        NotDestroyGameObject = 2, //不会销毁的Unity GameObject
    }

    /// <summary>
    /// 追踪资源加载情况 (AppSetting中 IsTraceResourceCreate可以控制是否启用这个选项)
    /// </summary>
    public partial class ResourcesTracker
    {
        [System.Serializable]
        //指定资源的信息
        private class TraceReSourcesInfor
        {
            public int InstanceID; //这里没有使用hashcode
            public TraceResourcesStateEnum ResourcesStateEnum;
            public Object TargetResources; //追踪对象
        }

        private static ResourcesTracker s_instance = null;

        public static ResourcesTracker S_Instance
        {
            get
            {
                if (s_instance == null) s_instance = new ResourcesTracker();
                return s_instance;
            }
        }


        #region Data 

        private static Dictionary<int, TraceReSourcesInfor> mAllTraceResourcesInfor = new Dictionary<int, TraceReSourcesInfor>();

        #endregion

        #region 追踪 UnityEngine.Object  对象

        /// <summary>
        /// 启动追踪资源的状态
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stateEnum">资源的状态类型 可以多个状态组合</param>
        /// <returns></returns>
        public static bool RegisterTraceResources(UnityEngine.Object obj, TraceResourcesStateEnum stateEnum)
        {
            if (Application.isPlaying == false || mIsTraceRecourceCreate == false)
                return false;

            if (obj == null)
            {
                Debug.LogError("RegistTraceResources Fail,Target object is Null");
                return false;
            }

            int instanceId = obj.GetInstanceID();
            TraceReSourcesInfor infor = null;
            if (mAllTraceResourcesInfor.TryGetValue(instanceId, out infor))
            {
                if (infor != null)
                {
                    if ((infor.ResourcesStateEnum & stateEnum) != 0) //判断是否已经包含了这一状态
                    {
                        Debug.LogEditorInfor(string.Format("RegistTraceResources Fail,Already exit Target instanceid={0} ,state is {1}", instanceId, stateEnum));
                        return false;
                    }

                    infor.ResourcesStateEnum = infor.ResourcesStateEnum | stateEnum; //新增这个状态

                    return true;
                }
            }

            infor = new TraceReSourcesInfor();
            infor.InstanceID = instanceId;
            infor.ResourcesStateEnum = stateEnum;
            infor.TargetResources = obj;
            mAllTraceResourcesInfor[instanceId] = infor;
#if UNITY_EDITOR
            Debug.LogEditorInfor($"RegistTraceResources Success,instanceid={instanceId}  target object name={obj.name} type={obj.GetType()}");
#endif
            return true;
        }

        ///// <summary>
        ///// 启动追踪资源的状态（状态是普通状态）
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static bool RegisterTraceResources(UnityEngine.Object obj)
        //{
        //    if (Application.isPlaying == false|| AppSetting.S_IsTraceResourceCreate == false)
        //        return false;
        //    if (obj == null)
        //    {
        //        Debug.LogError("RegistTraceResources Fail,Target object is Null");
        //        return false;
        //    }

        //    int instanceId = obj.GetInstanceID();
        //    TraceReSourcesInfor infor = null;
        //    if (mAllTraceResourcesInfor.TryGetValue(instanceId, out infor))
        //    {
        //        if (infor != null)
        //        {
        //            Debug.LogEditorInfor(string.Format("RegistTraceResources Fail,Already exit Target instanceid={0} ,target object name is {1}}", instanceId, obj.name));
        //            return false;
        //        }
        //    }
        //    infor = new TraceReSourcesInfor();
        //    infor.InstanceID = instanceId;
        //    infor.ResourcesStateEnum = TraceResourcesStateEnum.Normal;
        //    infor.TargetResources = obj;
        //    mAllTraceResourcesInfor[instanceId] = infor;
        //    Debug.Log(string.Format("RegistTraceResources Success,instanceid={0}  target object name={1}", instanceId, obj.name));
        //    return true;
        //}

        ///// <summary>
        ///// 取消追踪某个对象的某项属性(其他的状态保留)
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="stateEnum"></param>
        ///// <returns></returns>
        //public static bool UnRegisterTraceResources(UnityEngine.Object obj, TraceResourcesStateEnum stateEnum)
        //{
        //      if (Application.isPlaying == false||AppSetting.S_IsTraceResourceCreate==false)
        //        return false;
        //    if (obj == null)
        //    {
        //        Debug.LogError("UnRegisterTraceResources Fail,Target object is Null");
        //        return false;
        //    }

        //    int instanceId = obj.GetInstanceID();
        //    TraceReSourcesInfor infor = null;
        //    if (mAllTraceResourcesInfor.TryGetValue(instanceId, out infor))
        //    {
        //        if (infor == null)
        //        {
        //            Debug.LogError("UnRegisterTraceResources Fail, Record ResouceInfor is Null");
        //            return false;
        //        }

        //        if (object.ReferenceEquals(obj, infor.TargetResources) == false)
        //        {
        //            Debug.LogError("UnRegisterTraceResources Fail, Record Resources is Not Equal Reference");
        //            return false;
        //        }
        //        infor.ResourcesStateEnum = infor.ResourcesStateEnum & (~stateEnum);  //减去某个属性
        //        if (infor.ResourcesStateEnum == TraceResourcesStateEnum.Normal)
        //        {
        //            infor = null;
        //            mAllTraceResourcesInfor.Remove(instanceId);
        //        }
        //        return true;
        //    }

        //    Debug.LogError("UnRegisterTraceResources Fail, No Record of InstanceID", instanceId.ToString());
        //    return false;
        //}

        /// <summary>
        /// 取消追踪某个对象(不会进行状态判断)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool UnRegisterTraceResources(UnityEngine.Object obj)
        {
            if (Application.isPlaying == false || mIsTraceRecourceCreate == false)
                return false;
            if (obj == null)
            {
                Debug.LogError("UnRegisterTraceResources Fail,Target object is Null");
                return false;
            }

            int instanceId = obj.GetInstanceID();
            TraceReSourcesInfor infor = null;
            if (mAllTraceResourcesInfor.TryGetValue(instanceId, out infor))
            {
                if (infor == null)
                {
                    Debug.LogError("UnRegisterTraceResources Fail, Record ResouceInfor is Null");
                    return false;
                }

                if (object.ReferenceEquals(obj, infor.TargetResources) == false)
                {
                    Debug.LogError("UnRegisterTraceResources Fail, Record Resources is Not Equal Reference");
                    return false;
                }

                infor = null;
                mAllTraceResourcesInfor.Remove(instanceId);
#if UNITY_EDITOR
                Debug.LogEditorInfor(string.Format("UnRegisterTraceResources Success,instanceid={0}  target object name={1} type={2}", instanceId, obj.name, obj.GetType()));
#endif
                return true;
            }

            Debug.LogError(string.Format("UnRegisterTraceResources Fail, No Record of InstanceID {0} of object {1} type={1}", instanceId.ToString(), obj.name, obj.GetType()));
            return false;
        }

        #endregion
    }
}