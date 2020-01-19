using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{


    [Flags]
    internal enum UpdateCallbackUsage
    {
        None = 0,
        Update = 1,
        LateUpdate = 2,
        FixedUpdate = 4,
    }

    //用于不挂载脚本的对象获取Unity 内置的Update 回调
    internal class UpdateCallbackComponent : MonoBehaviour
    {
        public System.Action mUpdateCallbackEvent { get; private set; } = null;
        public System.Action mFixedUpdateCallbackEvent { get; private set; } = null;
        public System.Action mLateUpdateCallbackEvent { get; private set; } = null;


        private void OnDestroy()
        {
            RemoveAllUpdateCallback();
        }

        public void AddUpdateCallback(System.Action updateCallback)
        {
            mUpdateCallbackEvent = updateCallback;
            StateChangeController();
        }
        public void AddFixedUpdateCallback(System.Action fixedUpdateCallback)
        {
            mFixedUpdateCallbackEvent = fixedUpdateCallback;
            StateChangeController();
        }
        public void AddLateUpdateCallback(System.Action lateUpdateCallback)
        {
            mLateUpdateCallbackEvent = lateUpdateCallback;
            StateChangeController();
        }


        public void RemoveUpdateCallback()
        {
            mUpdateCallbackEvent = null;
            StateChangeController();
        }
        public void RemoveFixedUpdateCallback()
        {
            mFixedUpdateCallbackEvent = null;
            StateChangeController();
        }
        public void RemoveLateUpdateCallback()
        {
            mLateUpdateCallbackEvent = null;
            StateChangeController();
        }


        public void RemoveAllUpdateCallback()
        {
            mUpdateCallbackEvent = null;
            mFixedUpdateCallbackEvent = null;
            mLateUpdateCallbackEvent = null;
            StateChangeController();
        }


        private void StateChangeController()
        {
            if (mUpdateCallbackEvent == null && mFixedUpdateCallbackEvent == null && mLateUpdateCallbackEvent == null)
                this.enabled = false;
            else
                this.enabled = true;
        }



        // Update is called once per frame
        private void Update()
        {
            mUpdateCallbackEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            mFixedUpdateCallbackEvent?.Invoke();
        }

        private void LateUpdate()
        {
            mLateUpdateCallbackEvent?.Invoke();
        }

    }
}