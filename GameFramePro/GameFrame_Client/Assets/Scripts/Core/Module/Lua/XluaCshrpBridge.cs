using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using XLua;

namespace GameFramePro.Lua
{
    /// <summary>/// 提供 Xlua 访问C# 的常用通用接口/// </summary>
    public static class XluaCshrpBridge
    {
        /// <summary>/// xlua 直接访问的是底层判空接口，导致实际的结果与C# 判断不一致/// </summary>
        [LuaCallCSharp]
        public static bool IsNull(this UnityEngine.Object obj)
        {
            return obj == null;
        }

        #region Component 引用

        [LuaCallCSharp]
        public static Text GetTextComponent(this UnityEngine.GameObject obj)
        {
            if (obj == null) return null;
            return obj.GetComponent<Text>();
        }

        [LuaCallCSharp]
        public static Button GetButtonComponent(this UnityEngine.GameObject obj)
        {
            if (obj == null) return null;
            return obj.GetComponent<Button>();
        }

        [LuaCallCSharp]
        public static Image GetImageComponent(this UnityEngine.GameObject obj)
        {
            if (obj == null) return null;
            return obj.GetComponent<Image>();
        }

        #endregion
    }
}
