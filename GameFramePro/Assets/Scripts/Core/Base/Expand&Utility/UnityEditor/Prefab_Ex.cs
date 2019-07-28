﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFramePro
{
    /// <summary>
    /// 扩展预制体
    /// </summary>
    public static class Prefab_Ex
    {
       /// <summary>
       /// 判断选择的对象是否是场景中的对象还是资源对象
       /// </summary>
       /// <param name="gameObject"></param>
       /// <returns></returns>
        public static bool CheckIsSceneObject(UnityEngine.Object gameObject)
        {
            if (gameObject = null)
                return false;
            return string.IsNullOrEmpty(AssetDatabase.GetAssetPath(gameObject));
        }

    }
}