using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
/// <summary>///  Mono的单例泛型类/// </summary>
public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>/// 标示这个Mono  实例是否不会跟随场景销毁  默认会一起销毁/// </summary>
    protected virtual bool IsNotDestroyedOnLoad { get; } = false;


    #region 初始化和实例化逻辑

    private static object obj = new object();
    private static T s_Instance = null;
    protected bool mIsRigsterTrance = false; //标示是否已经注册了监听

    public static T S_Instance
    {
        get
        {
            if (s_Instance == null) s_Instance = Instantiate();
            return s_Instance;
        }
    }

    protected static T Instantiate()
    {
        s_Instance = ResourcesManager.Instantiate(typeof(T).Name).AddComponent<T>(); //测试发现当运行时如果报错，下一次运行会生成多个对象
        return s_Instance;
    }

    protected static T CheckInstance(bool isIgnoreCheck = true)
    {
        if (s_Instance != null && isIgnoreCheck)
            return s_Instance;
        if (ApplicationManager.mIsSingletonCreateSaftCheck)
        {
            try
            {
                lock (obj)
                {
                    var result = GameObject.FindObjectsOfType<T>();
                    if (result.Length == 0)
                    {
                        //   Debug.LogError("MonoSingleton ... Not Initialed :" + typeof(T));
                        Instantiate();
                    }
                    else if (result.Length == 1)
                    {
                        s_Instance = result[0];
                    }
                    else
                    {
                        s_Instance = result[result.Length - 1]; //Keep the First Initialed one  Be The Available
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

    protected virtual void DestroyedInstance()
    {
        s_Instance = null;
        obj = null;
    }

    #endregion

    /// <summary>/// 确保在运行时动态创建和添加组件依然保持只有一个对象/// </summary>
    protected virtual void Awake()
    {
        s_Instance = gameObject.GetAddComponentEx<T>();
        if (mIsRigsterTrance == false)
        {
            ResourcesTracker.RegisterTraceResources(s_Instance, TraceResourcesStateEnum.Singtion);
            mIsRigsterTrance = true;
        }

        if (IsNotDestroyedOnLoad)
            ResourcesManager.MarkNotDestroyOnLoad(gameObject);
    }


    protected virtual void OnDestroy()
    {
        if (mIsRigsterTrance)
            ResourcesTracker.UnRegisterTraceResources(s_Instance);
        mIsRigsterTrance = false;
        DestroyedInstance();
    }


#if UNITY_EDITOR
    protected virtual void Reset()
    {
        try
        {
            T[] allInstances = GameObject.FindObjectsOfType<T>();

            if (allInstances.Length > 1)
            {
                foreach (var targetInstance in allInstances)
                    Debug.LogError($"当前组件{typeof(T)} 已经存在于对象{targetInstance.gameObject.name}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

#endif
}
