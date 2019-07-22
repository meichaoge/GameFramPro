using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;
using System.Reflection;
using System.Linq;


/// <summary>
/// 参考 ExampleConfig 提供是示例配置 自行根据项目需求配置的
/// </summary>
public class XluaConfig 
{

    private const string CSharpAssembly = "Assembly-CSharp";//C# 程序集

    private static HashSet<string> s_HotFixInjectIgnoreNameSpace = new HashSet<string>() { "XLua" }; //Hotfix 标签需要过滤的命名空间
    private static HashSet<Type> s_DelegateIgnoreTypes = new HashSet<Type>() { }; //  需要过滤的委托类型



    [Hotfix]
    static IEnumerable<Type> HotfixInject
    {
        get
        {
            return (from type in Assembly.Load(CSharpAssembly).GetExportedTypes()
                    where type.Namespace == null || s_HotFixInjectIgnoreNameSpace.Contains(type.Namespace) == false
                    select type);
        }
    }
    //--------------begin 热补丁自动化配置-------------------------
    static bool hasGenericParameter(Type type)
    {
        if (type.IsGenericTypeDefinition) return true;
        if (type.IsGenericParameter) return true;
        if (type.IsByRef || type.IsArray)
        {
            return hasGenericParameter(type.GetElementType());
        }
        if (type.IsGenericType)
        {
            foreach (var typeArg in type.GetGenericArguments())
            {
                if (hasGenericParameter(typeArg))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 是否包含Editor 类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool typeHasEditorRef(Type type)
    {
        if (type.Namespace != null && (type.Namespace == "UnityEditor" || type.Namespace.StartsWith("UnityEditor.")))
        {
            return true;
        }
        if (type.IsNested) //嵌套类
        {
            return typeHasEditorRef(type.DeclaringType); //声明类
        }
        if (type.IsByRef || type.IsArray)
        {
            return typeHasEditorRef(type.GetElementType());
        }
        if (type.IsGenericType)
        {
            foreach (var typeArg in type.GetGenericArguments())
            {
                if (typeHasEditorRef(typeArg))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断委托中是否包含Editor 类型
    /// </summary>
    /// <param name="delegateType"></param>
    /// <returns></returns>
    static bool delegateHasEditorRef(Type delegateType)
    {
        if (typeHasEditorRef(delegateType)) return true;
        var method = delegateType.GetMethod("Invoke");
        if (method == null)
        {
            return false;
        }
        if (typeHasEditorRef(method.ReturnType)) return true;
        return method.GetParameters().Any(pinfo => typeHasEditorRef(pinfo.ParameterType));
    }

   // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
   [CSharpCallLua]
    static IEnumerable<Type> AllDelegate
    {
        get
        {
            BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            List<Type> allTypes = new List<Type>();
            var allAssemblys = new Assembly[]
            {
                Assembly.Load(CSharpAssembly)
            };
            foreach (var t in (from assembly in allAssemblys from type in assembly.GetTypes() select type))
            {
                var p = t;
                while (p != null)
                {
                    allTypes.Add(p);
                    p = p.BaseType;
                }
            }

            allTypes.RemoveAll((type) => { return s_DelegateIgnoreTypes.Contains(type); });  //过滤一些委托
            allTypes = allTypes.Distinct().ToList();


            var allMethods = from type in allTypes
                             from method in type.GetMethods(flag)
                             select method;
            var returnTypes = from method in allMethods
                              select method.ReturnType;
            var paramTypes = allMethods.SelectMany(m => m.GetParameters()).Select(pinfo => pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType() : pinfo.ParameterType);
            var fieldTypes = from type in allTypes
                             from field in type.GetFields(flag)
                             select field.FieldType;
            return (returnTypes.Concat(paramTypes).Concat(fieldTypes)).Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct();
        }
    }
    //--------------end 热补丁自动化配置-------------------------

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            };
}
