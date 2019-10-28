using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using GameFramePro;
using Newtonsoft.Json;

/// <summary>
/// 分析美术资源
/// </summary>
public class EffectResourceAnalysisEditor : EditorWindow
{
    [MenuItem("Tools/资源管理/获取BuildReport文本中美术资源列表")]
    private static void GetEffectResourcesAnalysis()
    {
        EffectResourceAnalysisEditor win = EditorWindow.GetWindow<EffectResourceAnalysisEditor>("获取BuildReport文本中美术资源列表");
        win.minSize = new Vector2(800, 600);
        win.Show();
    }

    #region data
    private string mBuildReportLogText = "";
    private List<string> mAllBuidReportLogInfors = new List<string>();
    private string mSearchKey = string.Empty;

    private static string mPrefabEffectDependencePath { get { return "Assets/Editor/Tool/ResourceAnalysis/PrefabEffectDependenceInfor.txt"; } }
    private TextAsset mPrefabEffectDependenceConfig = null;

    private Dictionary<string, List<string>> mAllEffectDependenceInfor = new Dictionary<string, List<string>>();
    private TextAsset mPrefabEffectDependenceLog = null;
    private const string EffectTag = "Effect_";
    private string searchEffectKey = string.Empty;

    //buildReport 中包含指定key 的内容
    private static string mBuildReportSpecialKeyInforPath { get { return "Assets/Editor/Tool/ResourceAnalysis/buidlReport_Analysis_Log.txt"; } }
    private TextAsset mBuildReportSpecialKeyLog = null;

    private static string mBuildReportAssetInforPath { get { return "Assets/Editor/Tool/ResourceAnalysis/buidlReport_Asset_Log.txt"; } }
    private TextAsset mBuildReportAssetLog = null;


    #endregion

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");

        #region 解析BuildReport log
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("解析完生成的BuildReport文件：", GUILayout.Width(200));
        if (mBuildReportAssetLog == null)
            mBuildReportAssetLog = AssetDatabase.LoadAssetAtPath<TextAsset>(mBuildReportAssetInforPath);
        EditorGUILayout.ObjectField(mBuildReportAssetLog, typeof(TextAsset), false);

        if (GUILayout.Button("解析整个打包后的日志文件"))
        {
            string filePath= EditorUtility.OpenFilePanel("打开Unity Editor.log 日志文件", Application.dataPath, "*.*");
            if (string.IsNullOrEmpty(filePath))
                return;

            mAllBuidReportLogInfors.Clear();
            StringBuilder builder = new StringBuilder();
            int  tagCount = 0;
            StreamReader reader = File.OpenText(filePath);
            string tag = "-------------------------------------------------------------------------------"; //Unity  BuildReport 开始和结束
            while (reader.EndOfStream==false)
            {
                string line = reader.ReadLine();
                if (tagCount == 1)
                {
                    builder.Append(line);
                    builder.Append(System.Environment.NewLine);
                    mAllBuidReportLogInfors.Add(line);
                }
               
                if (line== tag)
                {
                    ++tagCount;
                    if (tagCount >= 2)
                        break;
                }
            }

            string content = builder.ToString();
            string fileSavePath = $"{IOUtility.GetFilePathParentDirectory(Application.dataPath, 1)}/{mBuildReportAssetInforPath}";

            IOUtility.CreateOrSetFileContent(fileSavePath, content,false);
            Debug.Log("解析完Editor.log  日志文件中的Build Report文件"+ mBuildReportAssetInforPath);
        }

  //      GUILayout.Label(string.Format("一共包含{0}行数据", mAllBuidReportLogInfors.Count));
        EditorGUILayout.EndHorizontal();
        #endregion

        #region 加载Build Report文件

//        EditorGUILayout.BeginVertical("box");
//
//        GUILayout.Space(10);
//        EditorGUILayout.BeginHorizontal("box");
//        GUILayout.Label("Unity BuildReport Log日志绝对路径：",GUILayout.Width(200));
//        mBuildReportLogText = EditorGUILayout.TextField(mBuildReportLogText, GUILayout.ExpandWidth(true));
//        if (GUILayout.Button("加载BuildReport Log日志"))
//        {
//            string path = EditorUtility.OpenFilePanel("打开Build Report 日志", Application.dataPath, "*.*");
//            if (string.IsNullOrEmpty(path) == false)
//            {
//                mBuildReportLogText = path;
//                LoadBuildReportLog();
//            }
//        }
//        EditorGUILayout.EndHorizontal();
//        GUILayout.Label(string.Format("一共包含{0}行数据", mAllBuidReportLogInfors.Count));
//
//        EditorGUILayout.EndVertical();
        #endregion


        #region 查找某个特效

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("包含指定key的BuildReport文档：", GUILayout.Width(200));
        if (mBuildReportSpecialKeyLog == null)
            mBuildReportSpecialKeyLog = AssetDatabase.LoadAssetAtPath<TextAsset>(mBuildReportSpecialKeyInforPath);
        EditorGUILayout.ObjectField(mBuildReportSpecialKeyLog, typeof(TextAsset), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("要查找的关键字:", GUILayout.Width(100));
        mSearchKey = EditorGUILayout.TextField(mSearchKey, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("生成包含指定关键子的特效文档", GUILayout.Height(35)))
        {
            if (string.IsNullOrEmpty(mSearchKey))
            {
                Debug.LogError("请输入需要查询的key");
                return;
            }

            if (mAllBuidReportLogInfors.Count == 0)
            {
                Debug.LogError("请先加载BuildReport log日志");
                return;
            }

            GetAndSaveBuildReportLogOfSpecialKey(mSearchKey);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        #endregion


        #region 获取Resources下预制体引用的特效资源

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("预制体中包含的特效信息", GUILayout.Width(150));
        if (mPrefabEffectDependenceConfig == null)
            mPrefabEffectDependenceConfig = AssetDatabase.LoadAssetAtPath<TextAsset>(mPrefabEffectDependencePath);

        EditorGUILayout.ObjectField(mPrefabEffectDependenceConfig, typeof(TextAsset), false);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label(string.Format("特效的标示为以 [ \"{0}\" ] 开头命名", EffectTag));
        GUILayout.Label(string.Format("一共获取{0}个特效(被预制体)引用信息", mAllEffectDependenceInfor.Count));

        if(mPrefabEffectDependenceConfig!=null&&string.IsNullOrEmpty(mPrefabEffectDependenceConfig.text)==false && mAllEffectDependenceInfor.Count == 0)
        {
            mAllEffectDependenceInfor = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(mPrefabEffectDependenceConfig.text);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("检索Resource中预制体包含的特效", GUILayout.Height(35)))
        {
            mAllEffectDependenceInfor.Clear();
            GetAllResourcesEffectReferece();

            string conntent = JsonConvert.SerializeObject(mAllEffectDependenceInfor);
            string savePath = string.Format("{0}/{1}", IOUtility.GetFilePathParentDirectory(Application.dataPath, 1), mPrefabEffectDependencePath);

            IOUtility.CreateOrSetFileContent(savePath, conntent);
            Debug.Log(string.Format("获取脚本中特效依赖信息保存在{0}中,脚本中特效引用信息自行查看", mPrefabEffectDependencePath));
            AssetDatabase.Refresh();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        #endregion


        #region 特效引用
        GUILayout.Space(10);
        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();

        if (mPrefabEffectDependenceLog == null)
            mPrefabEffectDependenceLog = AssetDatabase.LoadAssetAtPath<TextAsset>(mPrefabEffectDependencePath);
        EditorGUILayout.ObjectField(mPrefabEffectDependenceLog, typeof(TextAsset), false);

        if (GUILayout.Button("加载特效被预制体依赖的日志信息"))
        {
            if (mPrefabEffectDependenceLog == null)
            {
                Debug.LogError("没有关于特效被预制体依赖的日志文件");
                return;
            }
            mAllEffectDependenceInfor = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(mPrefabEffectDependenceLog.text);
            Debug.Log("加载特效被预制体依赖的日志文件 成功");
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("输入要查找的特效名称(可以输入部分名称);", GUILayout.Width(250));
        searchEffectKey = EditorGUILayout.TextField(searchEffectKey, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("查找指定特效被预制体引用的信息", GUILayout.Height(35)))
        {
            if (string.IsNullOrEmpty(searchEffectKey))
            {
                Debug.Log("请输入要查找的特效Key");
                return;
            }
            if (mAllEffectDependenceInfor == null || mAllEffectDependenceInfor.Count == 0)
            {
                Debug.Log("请生成特效被预制体引用的信息");
                return;
            }
            foreach (var item in mAllEffectDependenceInfor)
            {
                if (item.Key.Contains(searchEffectKey))
                {
                    foreach (var prefab in item.Value)
                    {
                        Debug.Log(string.Format("特效被预制体  {0}  引用", prefab));
                    }
                }
            }
            Debug.Log(string.Format("查找特效  {0}  被引用次数信息完成", searchEffectKey));
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion


        EditorGUILayout.EndVertical();
    }



    #region 处理过程
    private void LoadBuildReportLog()
    {
        mAllBuidReportLogInfors.Clear();
        if (string.IsNullOrEmpty(mBuildReportLogText))
            return;

        if (System.IO.File.Exists(mBuildReportLogText) == false)
            return;
        StreamReader reader = null;
        try
        {
            mAllBuidReportLogInfors = new List<string>(200);
            reader = System.IO.File.OpenText(mBuildReportLogText);
            string line = string.Empty;
            while (reader.EndOfStream == false)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line) == false)
                    mAllBuidReportLogInfors.Add(line);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }

        Debug.Log(string.Format("一共加载了{0}条数据", mAllBuidReportLogInfors.Count));
    }

    /// <summary>
    /// 获取指定Key的特效可能包含哪些特效
    /// </summary>
    /// <param name="searchKey"></param>
    private void GetAndSaveBuildReportLogOfSpecialKey(string searchKey)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("key=" + searchKey);
        builder.Append(System.Environment.NewLine);

        foreach (var item in mAllBuidReportLogInfors)
        {
            if (item.Contains(searchKey))
            {
                builder.Append(item);
                builder.Append(System.Environment.NewLine);
            }
        }

        string content = builder.ToString();
        string savePath = string.Format("{0}/{1}", IOUtility.GetFilePathParentDirectory(Application.dataPath, 1), mBuildReportSpecialKeyInforPath);
        IOUtility.CreateOrSetFileContent(savePath, content, false);
        Debug.Log(string.Format("保存了关键子={0} 的日志文件在路径{1}", searchKey, savePath));
    }


    /// <summary>
    /// 获取Resources下所有预制体包含的特效
    /// </summary>
    /// <returns></returns>
    private void GetAllResourcesEffectReferece()
    {
        string[] assetGuidList = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });  //获取Resources下所有的预制体

        GameObject goRoot = new GameObject("Root");
        int count = 0;
        foreach (var assetguid in assetGuidList)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetguid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject go = GameObject.Instantiate(prefab, goRoot.transform);
            Transform[] childs = go.GetComponentsInChildren<Transform>(true);
            ++count;
            EditorUtility.DisplayProgressBar("解析预制体...", assetPath, 1f * count / assetGuidList.Length);

            foreach (var child in childs)
            {
                string gameName = child.gameObject.name;
                if (gameName.Contains(EffectTag))
                {
                    List<string> dependencesObjects = null;
                    if (mAllEffectDependenceInfor.TryGetValue(gameName, out dependencesObjects))
                    {
                        dependencesObjects.Add(prefab.name);
                    }
                    else
                    {
                        dependencesObjects = new List<string>();
                        dependencesObjects.Add(prefab.name);
                        mAllEffectDependenceInfor[gameName] = dependencesObjects;
                    }
                }
            }
            GameObject.DestroyImmediate(go);
        }
        EditorUtility.ClearProgressBar();
        GameObject.DestroyImmediate(goRoot);
    }

    /// <summary>
    /// 获取脚本中对特效的依赖
    /// </summary>
    private void GetSciptsEffectReference()
    {
        //string Pattern = "(\"@{1})([a-z]|[A-Z]|[0-9]|_|)+(\"{1})";   //多语言Key

        //var scripts = AssetDatabase.FindAssets("t:script", AllSearchScriptDirectory);  //获取Resources下所有的预制体    }
        //foreach (var item in scripts)
        //{
        //    string assetPath = AssetDatabase.GUIDToAssetPath(item);
        //    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        //    if (string.IsNullOrEmpty(asset.text))
        //        continue;

        //    MatchCollection result = Regex.Matches(asset.text, Pattern, RegexOptions.Multiline);
        //    if (result.Count == 0)
        //        continue;

        //    //if (AllSctiptsAssetInfor.Count < 10)
        //    //    Debug.Log("assetPath" + assetPath);
        //}

        //mAllScriptsLocalizationInfor.Clear();
        //foreach (var script in AllSctiptsAssetInfor)
        //{
        //    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(script);
        //    // Debug.Log("script=" + script);
        //    MatchCollection result = Regex.Matches(asset.text, Pattern, RegexOptions.Multiline);
        //    if (result.Count == 0)
        //        continue;

        //    List<LocalizationInfor_Script> localizationKeys = new List<LocalizationInfor_Script>(result.Count);
        //    for (int dex = 0; dex < result.Count; dex++)
        //    {
        //        var MatchInfor = result[dex];
        //        Group Groupinfor = MatchInfor.Groups[0];

        //        string Realkey = Groupinfor.Value.Replace("\"", "");  //去掉双引号
        //        if (Realkey.Length == 1)
        //            continue;  //过滤只有@的

        //        foreach (var item in localizationKeys)
        //        {
        //            if (item.mLocalizationKey == Realkey)
        //            {
        //                item.mReferenceCount++;
        //                break;
        //            }
        //        }
        //        LocalizationInfor_Script infor = new LocalizationInfor_Script();
        //        infor.mLocalizationKey = Realkey;
        //        infor.mReferenceCount = 1;
        //        infor.mScriptName = script;

        //        localizationKeys.Add(infor);
        //    }

        //    mAllScriptsLocalizationInfor[script] = localizationKeys;
        //}
    }

    #endregion




}
