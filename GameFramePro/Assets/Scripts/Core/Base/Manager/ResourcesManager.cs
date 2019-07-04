using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 负责整个项目的加载逻辑处理
    /// </summary>
    public partial class ResourcesManager : Singleton_Static<ResourcesManager>
    {

        #region Data 
        private HashSet<GameObject> m_AllNotDestroyOnLoadObjects = new HashSet<GameObject>(); // 所有不会在场景加载时候销毁的对象
        #endregion

    }
}