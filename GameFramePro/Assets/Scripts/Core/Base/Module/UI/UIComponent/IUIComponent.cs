using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace GameFramePro.UI
{
    /// <summary>
    /// 用于继承自UIBasePage 对象获取UI预制体上的组件
    /// </summary>
    public interface IUIComponent
    {
        /// <summary>
        /// 节点名称和路径的映射关系 用于根据名字查找对象
        /// </summary>
        Dictionary<string, string> NamePathMapInfor { get; }
        ///// <summary>
        ///// 关联的对象
        ///// </summary>
        //Transform ConnectTrans { get; }

        T GetComponentByName<T>(string gameObjectName) where T : Component;
        T GetComponentByPath<T>(string gameObjectName, string path) where T : Component;

 


        /// <summary>
        /// 被销毁时候释放引用
        /// </summary>
        void ReleaseReference(bool isReleaseNamePathMap);

    }
}