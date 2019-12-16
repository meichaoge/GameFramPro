using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

namespace GameFramePro
{
    /// <summary>
    /// 管理Xlua ，提供bug修复的入口
    /// </summary>
    public class XluaManager : Single<XluaManager>, IUpdateCountTick
    {
        public LuaEnv LuaEngine { get; private set; } //lua 虚拟机

        protected override void InitialSingleton()
        {
            if (LuaEngine == null)
            {
                LuaEngine = new LuaEnv();
                ResourcesTracker.RegistTraceNativeobject(LuaEngine, TraceNativeobjectStateEnum.Singtion);
                LuaEngine.AddLoader(CustomerLuaLoader);  //注册自定义的加载器
                AppEntryManager.RegisterUpdateTick(this);
            }
            base.InitialSingleton();
        }

        public override void DisposeInstance()
        {
            if (LuaEngine != null)
            {
                ResourcesTracker.UnRegistTraceNativeobject(LuaEngine);
                LuaEngine.Dispose();
            }
            base.DisposeInstance();
        }


        #region IUpdateTick 接口
        protected int curUpdateCount = 0; //当前的帧基数
        public uint TickPerUpdateCount { get; protected set; } = 10;

        public bool CheckIfNeedUpdateTick()
        {
            ++curUpdateCount;
            if (curUpdateCount == 1)
                return true;  //确保第一次被调用

            if (curUpdateCount < TickPerUpdateCount)
                return false;

            curUpdateCount = 0;
            return true;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick() == false) return;

            if (LuaEngine != null)
                LuaEngine.Tick();
        }
        #endregion




        /// <summary>
        /// 自定义的lua 加载器
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] CustomerLuaLoader(ref string filePath)
        {
            byte[] result = null;
            ResourcesManagerUtility.LoadAssetSync<TextAsset>(filePath, null, (isSuccess, textAsset) =>
            {
                if (isSuccess == false || textAsset == null || string.IsNullOrEmpty(textAsset.text))
                {
                    result = new byte[0];
                    return;
                }
                result = Encoding.UTF8.GetBytes(textAsset.text);
            });
            return result;
        }


    }
}