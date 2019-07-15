using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
/// <summary>
///  Mono的单例泛型类
/// </summary>
public class Single_Mono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object obj = new object();
    protected static T s_Instance = null;
    public static T S_Instance { get { return s_Instance; } }
    protected static T GetInstance(bool isIgnoreCheck = true)
    {
        if (s_Instance != null && isIgnoreCheck)
            return s_Instance;
        if (AppSetting.S_IsSingletonCreateSaftCheck)
        {
            try
            {
                lock (obj)
                {
                    var result = GameObject.FindObjectsOfType<T>();
                    if (result.Length == 0)
                    {
                        //   Debug.LogError("MonoSingleton ... Not Initialed :" + typeof(T));
                        s_Instance = ResourcesManager.Instantiate(typeof(T).Name).AddComponent<T>();  //测试发现当运行时如果报错，下一次运行会生成多个对象
                    }
                    else if (result.Length == 1)
                    {
                        s_Instance = result[0];
                    }
                    else
                    {
                        s_Instance = result[result.Length - 1];  //Keep the First Initialed one  Be The Avalable
                        Debug.LogError("There are " + result.Length + " " + typeof(T));
                        for (int dex = 0; dex < result.Length - 1; ++dex)
                            ResourcesManager.DestroyImmediate(result[dex]);
                    }
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError("GetInstance Exception : " + ex.ToString());
            }
        }

        return s_Instance;
    }


    /// <summary>
    /// 确保在运行时动态创建和添加组件依然保持只有一个对象
    /// </summary>
    protected virtual void Awake()
    {
        s_Instance = gameObject.GetAddComponent<T>();
        GetInstance(false);  //Make sure the other Component is destroyed
        ResourcesTracker.RegisterTraceResources(s_Instance,TraceResourcesStateEnum.Singtion);
    }


    protected virtual void OnDestroy()
    {
        ResourcesTracker.UnRegisterTraceResources(s_Instance);

        s_Instance = null;
        obj = null;
    }



#if UNITY_EDITOR
    /// <summary>
    /// 当在编辑器挂在脚本时调用
    /// </summary>
    protected virtual void Reset()
    {
        try
        {
            if (GameObject.FindObjectsOfType<T>().Length > 1)
            {
                ResourcesManager.DestroyImmediate(gameObject.GetComponent<T>());
                Debug.LogError("There are Already Exit " + typeof(T));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

#endif

}
