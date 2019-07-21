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
    public class XluaManager : Single<XluaManager>
    {
        public LuaEnv LuaEngine { get; private set; } //lua 虚拟机

        protected override void InitialSingleton()
        {
            if (LuaEngine == null)
            {
                LuaEngine = new LuaEnv();
                ResourcesTracker.RegistTraceNativeobject(LuaEngine, TraceNativeobjectStateEnum.Singtion);
                LuaEngine.AddLoader(CustomerLuaLoader);  //注册自定义的加载器

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

        /// <summary>
        /// 自定义的lua 加载器
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] CustomerLuaLoader(ref string filePath)
        {
            Debug.LogError("TODO Customer loader");
            string luaAssetInfor = ResourcesManager.LoadTextAssettSync(filePath);
            if (string.IsNullOrEmpty(luaAssetInfor) == false)
                return new byte[0];

            return Encoding.UTF8.GetBytes(luaAssetInfor);
        }


    }
}