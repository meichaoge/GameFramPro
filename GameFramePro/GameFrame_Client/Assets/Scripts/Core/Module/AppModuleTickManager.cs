using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    [System.Flags]
    internal enum UpdateCallbackUsage
    {
        None = 0,
        Update = 1,
        LateUpdate = 2,
        FixedUpdate = 4,
    }

    /// <summary>
    /// 间接控制全局模块Update 更新逻辑
    /// </summary>
    public class AppModuleTickManager : SingleMono<AppModuleTickManager>
    {
        public static List<System.Action> mUpdateCallbackEvent { get; private set; } = new List<System.Action>();
        public static List<System.Action> mFixedUpdateCallbackEvent { get; private set; } = new List<System.Action>();
        public static List<System.Action> mLateUpdateCallbackEvent { get; private set; } = new List<System.Action>();



        public static void AddUpdateCallback(System.Action updateCallback)
        {
            AddUpdateCallback(updateCallback, mUpdateCallbackEvent);
        }
        public static void AddFixedUpdateCallback(System.Action fixedUpdateCallback)
        {
            AddUpdateCallback(fixedUpdateCallback, mFixedUpdateCallbackEvent);
        }
        public static void AddLateUpdateCallback(System.Action lateUpdateCallback)
        {
            AddUpdateCallback(lateUpdateCallback, mLateUpdateCallbackEvent);
        }


        private static void AddUpdateCallback(System.Action updateCallback, List<System.Action> dataSources)
        {
            if (updateCallback == null) return;
            if (dataSources.Contains(updateCallback))
                return;
            dataSources.Add(updateCallback);
        }


        public static void RemoveUpdateCallback(System.Action updateCallback)
        {
            RemoveUpdateCallback(updateCallback, mUpdateCallbackEvent);
        }
        public static void RemoveFixedUpdateCallback(System.Action fixedUpdateCallback)
        {
            RemoveUpdateCallback(fixedUpdateCallback, mFixedUpdateCallbackEvent);
        }
        public static void RemoveLateUpdateCallback(System.Action lateUpdateCallback)
        {
            RemoveUpdateCallback(lateUpdateCallback, mLateUpdateCallbackEvent);
        }

        private static void RemoveUpdateCallback(System.Action updateCallback, List<System.Action> dataSources)
        {
            if (updateCallback == null) return;
            for (int dex = dataSources.Count - 1; dex >= 0; dex--)
            {
                if (dataSources[dex] == updateCallback)
                {
                    dataSources.RemoveAt(dex);
                    return;
                }
            }
        }


        // Update is called once per frame
        private void Update()
        {
            TriggerUpdateCallback(mUpdateCallbackEvent);
        }

        private void FixedUpdate()
        {
            TriggerUpdateCallback(mFixedUpdateCallbackEvent);
        }

        private void LateUpdate()
        {
            TriggerUpdateCallback(mLateUpdateCallbackEvent);
        }

        private static void TriggerUpdateCallback(List<System.Action> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
                return;
            for (int dex = dataSources.Count - 1; dex >= 0; dex--)
            {
                var updateCallback = dataSources[dex];
                if (updateCallback == null)
                {
                    dataSources.RemoveAt(dex);
                    continue;
                }

                updateCallback.Invoke();
            }
        }

    }
}