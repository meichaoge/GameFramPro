using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    [System.Flags]
    //资源追踪状态
    public enum TraceNativeobjectStateEnum
    {
        Normal = 0,  //普通
        Singtion = 1, //单例
    }

    public partial class ResourcesTracker
    {
        [System.Serializable]
        //指定资源的信息
        private class TraceNativeobjectInfor
        {
            public int HashCode; //
            public TraceNativeobjectStateEnum NativeobjectState;
            public object Targetobject;  //追踪对象
        }


        #region Data 
        private static Dictionary<int, TraceNativeobjectInfor> mAllTraceNativeobjectsInfor = new Dictionary<int, TraceNativeobjectInfor>();
        #endregion

        #region 追踪 原生 object  对象

        /// <summary>
        /// 启动追踪资源的状态
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stateEnum">资源的状态类型 可以多个状态组合</param>
        /// <returns></returns>
        public static bool RegistTraceNativeobject(object obj, TraceNativeobjectStateEnum stateEnum)
        {
              if (Application.isPlaying == false||AppSetting.S_IsTraceResourceCreate==false)
                return false;
            if (obj == null)
            {
                Debug.LogError("RegistTraceNativeobject Fail,Target object is Null");
                return false;
            }

            int hashCode = obj.GetHashCode();
            TraceNativeobjectInfor infor = null;
            if (mAllTraceNativeobjectsInfor.TryGetValue(hashCode, out infor))
            {
                if (infor != null)
                {
                    if ((infor.NativeobjectState & stateEnum) != 0)  //判断是否已经包含了这一状态
                    {
                        Debug.LogEditorInfor(string.Format("RegistTraceNativeobject Fail,Already exit Target instanceid={0} ,state is {1}}", hashCode, stateEnum));
                        return false;
                    }
                    infor.NativeobjectState = infor.NativeobjectState | stateEnum; //新增这个状态

                    return true;
                }
            }
            infor = new TraceNativeobjectInfor();
            infor.HashCode = hashCode;
            infor.NativeobjectState = stateEnum;
            infor.Targetobject = obj;
            mAllTraceNativeobjectsInfor[hashCode] = infor;
            Debug.Log(string.Format("RegistTraceResources Success,instanceid={0}  target object name={1}", hashCode, obj.GetType().Name));
            return true;
        }

        /// <summary>
        /// 启动追踪资源的状态（状态是普通状态）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool RegistTraceNativeobject(object obj)
        {
              if (Application.isPlaying == false||AppSetting.S_IsTraceResourceCreate==false)
                return false;
            if (obj == null)
            {
                Debug.LogError("RegistTraceNativeobject Fail,Target object is Null");
                return false;
            }

            int hashCode = obj.GetHashCode();
            TraceNativeobjectInfor infor = null;
            if (mAllTraceNativeobjectsInfor.TryGetValue(hashCode, out infor))
            {
                if (infor != null)
                {
                    Debug.LogEditorInfor(string.Format("RegistTraceNativeobject Fail,Already exit Target instanceid={0} ,target object name is {1}}", hashCode, obj.GetType().Name));
                    return false;
                }
            }
            infor = new TraceNativeobjectInfor();
            infor.HashCode = hashCode;
            infor.NativeobjectState = TraceNativeobjectStateEnum.Normal;
            infor.Targetobject = obj;
            mAllTraceNativeobjectsInfor[hashCode] = infor;
            Debug.Log(string.Format("RegistTraceNativeobject Success,instanceid={0}  target object name={1}", hashCode, obj.GetType().Name));
            return true;
        }

        /// <summary>
        /// 取消追踪某个对象的某项属性(其他的状态保留)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stateEnum"></param>
        /// <returns></returns>
        public static bool UnRegistTraceNativeobject(object obj, TraceNativeobjectStateEnum stateEnum)
        {
              if (Application.isPlaying == false||AppSetting.S_IsTraceResourceCreate==false)
                return false;
            if (obj == null)
            {
                Debug.LogError("UnRegistTraceNativeobject Fail,Target object is Null");
                return false;
            }

            int hashCode = obj.GetHashCode();
            TraceNativeobjectInfor infor = null;
            if (mAllTraceNativeobjectsInfor.TryGetValue(hashCode, out infor))
            {
                if (infor == null)
                {
                    Debug.LogError("UnRegistTraceNativeobject Fail, Record ResouceInfor is Null");
                    return false;
                }

                if (object.ReferenceEquals(obj, infor.Targetobject) == false)
                {
                    Debug.LogError("UnRegistTraceNativeobject Fail, Record Resources is Not Equal Reference");
                    return false;
                }
                infor.NativeobjectState = infor.NativeobjectState & (~stateEnum);  //减去某个属性
                if (infor.NativeobjectState == TraceNativeobjectStateEnum.Normal)
                {
                    infor = null;
                    mAllTraceNativeobjectsInfor.Remove(hashCode);
                }
                return true;
            }

            Debug.LogErrorFormat("UnRegistTraceResources Fail, No Record of HashCode", hashCode.ToString());
            return false;
        }

        /// <summary>
        /// 取消追踪某个对象(不会进行状态判断)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool UnRegistTraceNativeobject(object obj)
        {
              if (Application.isPlaying == false||AppSetting.S_IsTraceResourceCreate==false)
                return false;
            if (obj == null)
            {
                Debug.LogError("UnRegistTraceNativeobject Fail,Target object is Null");
                return false;
            }

            int hashCode = obj.GetHashCode();
            TraceNativeobjectInfor infor = null;
            if (mAllTraceNativeobjectsInfor.TryGetValue(hashCode, out infor))
            {
                if (infor == null)
                {
                    Debug.LogError("UnRegistTraceNativeobject Fail, Record ResouceInfor is Null");
                    return false;
                }

                if (object.ReferenceEquals(obj, infor.Targetobject) == false)
                {
                    Debug.LogError("UnRegistTraceNativeobject Fail, Record Resources is Not Equal Reference");
                    return false;
                }

                infor = null;
                mAllTraceNativeobjectsInfor.Remove(hashCode);
                return true;
            }

            Debug.LogErrorFormat("UnRegistTraceNativeobject Fail, No Record of HashCode", hashCode.ToString());
            return false;
        }

        #endregion

    }
}