using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 缓存UI 组件名到路径的映射关系
    /// </summary>
    public static class ComponentUtility
    {
        //key =Unity 对象实例名 value={ key=节点名,value=path to root }
        private static Dictionary<string, Dictionary<string, string>> mAllGameObjectNamePathMap = new Dictionary<string, Dictionary<string, string>>();//每个对象上节点的相对根节点路径映射


        /// <summary>
        /// 获取指定对象的名称和路径的映射
        /// </summary>
        /// <param name="go"></param>
        public static Dictionary<string, string> GetGameObjectNamePathMap(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("GetGameObjectNamePathMap Fail,parameter is null");
                return null;
            }
            Dictionary<string, string> namePathMap = null;
            if (mAllGameObjectNamePathMap.TryGetValue(go.name,out namePathMap))
                return GetNamePathMap(namePathMap);
            namePathMap = new Dictionary<string, string>();
            Transform[] allChildTrans = go.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildTrans)
            {
                if (namePathMap.ContainsKey(child.name))
                {
                    Debug.Log("GetGameObjectNamePathMap 包含重复的对象名 " + child.name);
                    continue;
                }
                namePathMap[child.name] = child.GetTransRelativePathToRoot(go.transform);
            }
            mAllGameObjectNamePathMap[go.name] = namePathMap;
            return GetNamePathMap(namePathMap); 
        }

        /// <summary>
        /// 避免直接修改了缓存的原始数据
        /// </summary>
        /// <param name="namePathMap"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetNamePathMap(Dictionary<string, string> namePathMap)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>(namePathMap);
            //if (namePathMap != null)
            //{

            //}
            //foreach (var item in namePathMap)
            //    temp[item.Key] = item.Value;

            return temp;
        }

        /// <summary>
        /// 获取指定对象关联的映射
        /// </summary>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetGameObjectPathByName(string gameObjectName)
        {
            Dictionary<string, string> nameMap = null;

            if (mAllGameObjectNamePathMap.TryGetValue(gameObjectName, out nameMap))
                return nameMap;
            return null;
        }

    }
}