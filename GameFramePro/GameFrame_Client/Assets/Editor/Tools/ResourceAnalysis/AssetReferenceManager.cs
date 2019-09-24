﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using System.Text;
using System;
using GameFramePro;
using GameFramePro.UI;
using Newtonsoft.Json;

/// <summary>/// 编辑器下UI资源管理
/// 要求所有的图片都是Resources下预制体加载，而不是直接加载
/// UI资源和特效资源单独放置/// </summary>
public class AssetReferenceManager
{
    [MenuItem("Tools/删除所有PlayerPrefs 的数据")]
    private static void DelateAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("删除所有PlayerPrefs 的数据成功!!!");
    }

    /// <summary>
    /// 某一个资源的信息
    /// </summary>
    private class AssetInfor
    {
        public string mAssetRelativePath;
        public string mAssetMd5;
        public List<PrefabAssetNodeDetailInfor> mPrefabAssetNodesInfor = new List<PrefabAssetNodeDetailInfor>();
        public List<string> mDepdenceInfor = new List<string>();
    }

    /// <summary>
    /// 预制体中节点的信息
    /// </summary>
    public class PrefabAssetNodeDetailInfor
    {
        public string mNodeName;
        public string mNodeToRootPath;
        public string mNodeLayerName;
    }


    /// <summary>
    /// 缓存的资源依赖日志记录
    /// </summary>
    private static string mAssetsDepdenceInforLog { get; } = "Assets/Editor/Tool/ResourceAnalysis/AssetDepdenceLog.txt";

    private static Dictionary<string, AssetInfor> mAllPrefabsDepdence = new Dictionary<string, AssetInfor>(); //所有记录的Prefab资源的引用

    #region 编辑器菜单

    #region 一键设置Hierachy状态

    [MenuItem("GameObject/UI/设置选中预制体所有节点可见", false, 100)]
    private static void SetSeletcedObjectState()
    {
        if (Selection.gameObjects.Length == 0)
            return;
        Transform[] allChilds = Selection.gameObjects[0].GetComponentsInChildren<Transform>();
        foreach (var item in allChilds)
        {
            item.gameObject.SetActive(true);
        }
    }

    #endregion


    #region 获取依赖

    //[MenuItem("Tools/资源管理/获取Resources下Prefabs依赖信息")]
    //private static void GeAllPrefabsAssetInfor()
    //{
    //    GetAllPrefabRelatveInfor();
    //}

    //[MenuItem("Tools/资源管理/获取Assets下所有Prefabs依赖信息")]
    //private static void GeAllPrefabsAssetInfor_Assets()
    //{
    //    GetAllPrefabRelatveInfor();
    //}

    [MenuItem("工具和扩展/资源管理/获取Assets下所有资源依赖信息")]
    private static void GetAllAssetsDepdenceInfor()
    {
        GetAllPrefabRelatveInfor();
    }


    [MenuItem("工具和扩展/资源管理/获取选择资源依赖信息")]
    private static void GeSelectAssetDepdenceInfor()
    {
        if (Selection.objects.Length > 1)
        {
            Debug.LogError("选择一个资源后重试");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        GetDepdenceAssetInfor(assetPath);
    }

    [MenuItem("Assets/工具和扩展/引用/获取选择资源依赖信息")]
    private static void GeSelectAssetDepdenceInfor_Asseta()
    {
        GeSelectAssetDepdenceInfor();
    }


    [MenuItem("Assets/工具和扩展/引用/查找引用图集")]
    private static void GeSelectAssetDepdenceInfor_ReferencePrefab()
    {
        GetImgAssetReferencePrefab_All();
    }

    [MenuItem("Assets/工具和扩展/引用/查找多个图片引用图集")]
    private static void GeSelectAssetDepdenceInfor_ReferencePrefab_multi()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;


        for (int i = 0; i < selectobjs.Length; i++)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectobjs[i]);
            if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
            {
                ShowAllImageReference(assetPath);
            }
            else
            {
                Debug.LogError("选择的资源不是图片资源 " + assetPath);
            }
        }
    }


    [MenuItem("Assets/工具和扩展/引用/查看图片被引用的具体信息")]
    private static void GetImgReferenceDetail()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);

        if (assetPath.EndsWith(".png") == false && assetPath.EndsWith(".jpg") == false)
        {
            Debug.LogError("请选择图片资源" + assetPath);
            return;
        }

        List<string> result = GetAssetReference(assetPath);
        if (result.Count == 0)
        {
            Debug.Log($"获取资源{assetPath}  被预制体引用信息为0");
            return;
        }

        Debug.Log(string.Format("Begin 获取图片资源{0}被引用信息详情 ", assetPath));
        foreach (var item in result)
        {
            if (item.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                if (prefab == null)
                {
                    Debug.LogError(string.Format("资源{0} 已经不存在", item));
                    continue;
                }

                GameObject go = GameObject.Instantiate(prefab);
                Image[] allImags = go.GetComponentsInChildren<Image>(true);
                foreach (var img in allImags)
                {
                    //if(img.sprite==null)
                    //{
                    //    Debug.Log(string.Format("{0}上有一个没有关联图片的节点{1} 导致无法识别资源", item, img.gameObject.name));
                    //    continue;
                    //}

                    string path = AssetDatabase.GetAssetPath(img.sprite);
                    if (path == assetPath)
                        Debug.Log($"图片资源被预制体{item,30} 子节点:{img.gameObject.name}  引用 ");
                }

                //**UI下的UguiAtlas
                SpriteRenderer[] allUguiAtlas = go.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var uiAtlas in allUguiAtlas)
                {
                    string path = AssetDatabase.GetAssetPath(uiAtlas.sprite);
                    if (path == assetPath)
                        Debug.Log($"图片资源被 UguiAtlas 预制体{item,30} 子节点:{uiAtlas.gameObject.name}  引用 ");
                }

                GameObject.DestroyImmediate(go);
            }
            else
            {
                Debug.Log($"资源{assetPath}被非预制体引用{item}");
            }
        }

        Debug.Log($"End 获取图片资源{assetPath}被引用信息详情 ");
    }


    [MenuItem("Assets/工具和扩展/引用/查看资源被引用的具体信息")]
    private static void GeAssetReferenceDetail()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);

        List<string> result = GetAssetReference(assetPath);
        Debug.Log(string.Format("Begin 获取资源{0}被引用信息详情 ", assetPath));
        foreach (var item in result)
        {
            if (item != assetPath)
                Debug.Log(string.Format("资源被 {0} 引用", item));
        }

        Debug.Log(string.Format("End 获取资源{0}被引用信息详情 ", assetPath));
    }

    #endregion

    #region 文件

    [MenuItem("工具和扩展/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_全部")]
    private static void GetImgAssetReferencePrefab_All()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            ShowAllImageReference(assetPath);
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_无引用")]
    private static void GetImgAssetReferencePrefab_NoReference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            ShowAllImageReference_NoReference(assetPath);
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("工具和扩展/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_引用")]
    private static void GetImgAssetReferencePrefab_Reference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            ShowAllImageReference_Reference(assetPath);
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("工具和扩展/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_自动设置无用资源格式")]
    private static void GetImgAssetReferencePrefab_AutoSet()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            ShowAndAutoSetAllImageReference_NoReference(assetPath);
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("Assets/工具和扩展/资源优化/设置图片被引用Prefabs关联Image格式(九宫格 Slider)")]
    private static void AutoSetImgReferencePrefabType_Sliced()
    {
        AutoSetImgReferencePrefabType(Image.Type.Sliced);
    }

    [MenuItem("Assets/工具和扩展/资源优化/设置图片被引用Prefabs关联Image格式(九宫格 Tilied)")]
    private static void AutoSetImgReferencePrefabType_Tilied()
    {
        AutoSetImgReferencePrefabType(Image.Type.Tiled);
    }


    private static void AutoSetImgReferencePrefabType(Image.Type type)
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            List<string> result = GetAssetReference(assetPath);
            foreach (var item in result)
            {
                Debug.Log(string.Format("资源{0,-30} 被预制体引用{1}", assetPath, item));
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                GameObject go = GameObject.Instantiate(prefab);

                Image[] childImg = go.GetComponentsInChildren<Image>(true);
                if (childImg.Length == 0)
                {
                    GameObject.DestroyImmediate(go);
                    continue;
                }

                bool isNeedSave = false;
                foreach (var img in childImg)
                {
                    if (img.sprite == null) continue;
                    string path = AssetDatabase.GetAssetPath(img.sprite);
                    if (path == assetPath && img.type != type)
                    {
                        img.type = type;
                        isNeedSave = true;
                        Debug.Log($"修改了节点{img.gameObject.name} image 格式为九宫格拉伸 {type}");
                    }
                }

                if (isNeedSave)
                {
                    PrefabUtility.CreatePrefab(item, go);
                    Debug.Log($"设置了预制体{item}图片格式");
                }

                GameObject.DestroyImmediate(go);
            }

            AssetDatabase.Refresh();
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("Assets/工具和扩展/资源优化/设置图片被引用Prefabs关联Image格式(水平分成2个)")]
    private static void AutoSetImgReferencePrefab_Horizontial()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            List<string> result = GetAssetReference(assetPath);
            foreach (var item in result)
            {
                Debug.Log(string.Format("资源{0,-30} 被预制体引用{1}", assetPath, item));
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                GameObject go = GameObject.Instantiate(prefab);

                Image[] childImg = go.GetComponentsInChildren<Image>(true);
                if (childImg.Length == 0)
                {
                    GameObject.DestroyImmediate(go);
                    continue;
                }

                foreach (var img in childImg)
                {
                    if (img.sprite == null) continue;
                    string path = AssetDatabase.GetAssetPath(img.sprite);
                    if (path == assetPath && img.type != Image.Type.Sliced)
                    {
                        Image[] ChildImgs = new Image[2];
                        RectTransform parent = img.transform as RectTransform;

                        #region 创建两个子Image

                        GameObject go2 = new GameObject("ChildImage_01");
                        go2.transform.SetParent(img.transform);
                        ChildImgs[1] = go2.GetAddComponentEx<Image>();
                        go2.transform.SetAsFirstSibling();

                        GameObject go1 = new GameObject("ChildImage_00");

                        go1.transform.SetParent(img.transform);
                        ChildImgs[0] = go1.GetAddComponentEx<Image>();
                        go1.transform.SetAsFirstSibling();

                        for (int dex = 0; dex < 2; dex++)
                        {
                            ChildImgs[dex].raycastTarget = img.raycastTarget;
                        }

                        #endregion

                        for (int dex = 0; dex < 2; dex++)
                        {
                            ChildImgs[dex].sprite = img.sprite;
                            ChildImgs[dex].raycastTarget = false;

                            RectTransform target = ChildImgs[dex].transform as RectTransform;
                            target.pivot = Vector2.one * 0.5f;
                            target.anchorMin = Vector2.zero;
                            target.anchorMax = Vector2.one;

                            target.anchoredPosition3D = Vector3.zero;

                            if (dex == 0)
                            {
                                target.localScale = Vector3.one;
                                target.offsetMax = new Vector2(-1 * parent.rect.width * 0.5f, 0);
                                target.offsetMin = Vector2.zero;
                            }
                            else
                            {
                                target.localScale = new Vector3(-1, 1, 1);
                                target.offsetMax = Vector2.zero;
                                target.offsetMin = new Vector2(parent.rect.width * 0.5f, 0);
                            }
                        }

                        Button connectButton = img.transform.GetComponent<Button>();

                        string imgName = img.gameObject.name;
                        img.sprite = null;
                        GameObject.DestroyImmediate(img);
                        if (connectButton == null)
                            continue;

                        connectButton.transform.gameObject.AddComponent<NoDrawingRayCast>();
                        connectButton.transition = Selectable.Transition.None;


                        Debug.Log("水平方向上分解了节点 " + imgName);
                    }
                }

                PrefabUtility.CreatePrefab(item, go);
                //    Debug.Log(string.Format("设置了预制体{0}图片格式", item));
                GameObject.DestroyImmediate(go);
            }

            AssetDatabase.Refresh();
            //Debug.Log("一共设置了" + result.Count + "个资源的格式");
            return;
        }

        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }

    #endregion

    #region 文件夹

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_全部")]
    private static void GetImgsAssetReferencePrefab_All()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        List<string> allImageAsset = new List<string>();
        foreach (var fileFolder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(fileFolder);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] {assetPath});

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

        ShowAllImageReference(allImageAsset);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_没引用")]
    private static void GetImgsAssetReferencePrefab_NoReference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        List<string> allImageAsset = new List<string>();
        foreach (var fileFolder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(fileFolder);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] {assetPath});

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

        ShowAllImageReference_NoReference(allImageAsset);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_引用")]
    private static void GetImgsAssetReferencePrefab_Reference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        List<string> allImageAsset = new List<string>();
        foreach (var fileFolder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(fileFolder);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] {assetPath});

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

        ShowAllImageReference_Reference(allImageAsset);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_自动设置无用资源格式")]
    private static void GetImgsAssetReferencePrefab_AutoSet()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        List<string> allImageAsset = new List<string>(20);
        foreach (var fileFolder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(fileFolder);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] {assetPath});

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

        ShowAndAutoSetAllImageReference_NoReference(allImageAsset);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_自动删除")]
    private static void GetImgsAssetReferencePrefab_AutoDelete()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        List<string> allImageAsset = new List<string>(20);
        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] {assetPath});

        foreach (var spriteAsset in containSprites)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
            allImageAsset.Add(spritePath);
        }

        int totalCount = ShowAndAutoDeleteAllImageReference_NoReference(allImageAsset);
    }

    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/自动删除DefaultType Image")]
    private static void DeleteDefaultTypeImage()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请选择一个文件夹");
            return;
        }

        int defaultTypeAssetCount = 0;
        foreach (var fileFolder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(fileFolder);
            string realPath = string.Format("{0}/{1}", IOUtility.GetFilePathParentDirectory(Application.dataPath, 1), assetPath);
            //Debug.Log("realPath=" + realPath);
            string[] allPngAssets = System.IO.Directory.GetFiles(realPath, "*.png", System.IO.SearchOption.AllDirectories);
            foreach (var pngAsset in allPngAssets)
            {
                string unityAssetPath = pngAsset.Substring((Application.dataPath.Length - "Assets".Length));
                //   Debug.Log("unityAssetPath=" + unityAssetPath);
                TextureImporter impoter = AssetImporter.GetAtPath(unityAssetPath) as TextureImporter;
                if (impoter.textureType != TextureImporterType.Default)
                    continue;

                Debug.Log(string.Format("删除Default格式的png 资源 : {0}", unityAssetPath));
                AssetDatabase.DeleteAsset(unityAssetPath);
                ++defaultTypeAssetCount;
            }
        }

        if (defaultTypeAssetCount > 0)
            AssetDatabase.Refresh();
        Debug.Log(string.Format("一共删除了{0}个Default 格式的png资源 ", defaultTypeAssetCount));
    }


    [MenuItem("工具和扩展/资源管理/图片管理(文件夹)/自动删除DefaultType和无引用的图片")]
    private static void DeleteDefaultTypeAndNoRefenceImage()
    {
        GetImgsAssetReferencePrefab_AutoDelete();
        DeleteDefaultTypeImage();
    }

    [MenuItem("Assets/工具和扩展/引用/查看文件夹中图片被引用的具体信息")]
    private static void GetAllImageReferenceInfor()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
        {
            Debug.LogError("请先选择一个文件夹");
            return;
        }

        //Key=SpritePath Value=ReferenceInfor
        SortedDictionary<string, List<string>> AllImagsReferenceInfor = new SortedDictionary<string, List<string>>();
        foreach (var folder in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(folder);
            string[] spriteAssets = AssetDatabase.FindAssets("t:sprite", new string[] {assetPath});
            foreach (var sprite in spriteAssets)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(sprite);
                //Debug.Log("spritePath=" + spritePath);

                List<string> result = GetAssetReference(spritePath);
                if (result == null || result.Count == 0)
                    continue;
                AllImagsReferenceInfor[spritePath] = result;
            }
        }

        StringBuilder builder = new StringBuilder();
        foreach (var item in AllImagsReferenceInfor)
        {
            if (item.Value.Count == 0)
                continue;

            builder.Append(string.Format("资源{0}被引用{1}次:", item.Key, item.Value.Count));
            builder.Append(System.Environment.NewLine);
            foreach (var reference in item.Value)
            {
                builder.Append(string.Format("\t\t {0}", reference));
                builder.Append(System.Environment.NewLine);
            }

            builder.Append(System.Environment.NewLine);
            builder.Append(System.Environment.NewLine);
        }


        string content = builder.ToString();
        string resultPath = string.Format("{0}/ResourcesAnalysis/FolderImgReferenceInfor.txt", IOUtility.GetFilePathParentDirectory(Application.dataPath, 1));
        IOUtility.CreateOrSetFileContent(resultPath, content, false);
        GUIUtility.systemCopyBuffer = content; //复制到剪切板
        Debug.Log(string.Format("所有文件中图片被引用的本地化信息已经输出到日志{0}中，并且已经复制到剪切板中", resultPath));
    }

    #endregion

    #endregion


    #region 处理接口

    /// <summary>
    /// 获取指定预制体依赖的资源信息
    /// </summary>
    /// <param name="prefabAsset"></param>
    private static void GetDepdenceAssetInfor(string assetPath)
    {
        List<string> depdence = AssetDatabase.GetDependencies(assetPath).ToList(); //获取当前资源的依赖
        depdence.Sort();
        Debug.Log(string.Format("***********显示当前资源{0} 依赖的资源个数:{1}  Start ************", assetPath, depdence.Count));
        foreach (var item in depdence)
        {
            if (item == assetPath)
                continue; //过滤自己
            Debug.Log(item);
        }

        Debug.Log("***************** End *****************");
    }

    private static void GetAllPrefabRelatveInfor()
    {
        TextAsset logAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(mAssetsDepdenceInforLog);
        if (logAsset != null && string.IsNullOrEmpty(logAsset.text) == false)
        {
            if (EditorUtility.DisplayDialog("提示", $"{mAssetsDepdenceInforLog}已经缓存了资源的依赖信息，是否使用缓存的信息", "使用缓存", "重新生成数据"))
            {
                mAllPrefabsDepdence.Clear();
                mAllPrefabsDepdence = JsonConvert.DeserializeObject<Dictionary<string, AssetInfor>>(logAsset.text);
                return;
            }
        }

        GameObject root = new GameObject("root");
        try
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int totalCount = 0;
            int index = 0;
            mAllPrefabsDepdence.Clear();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            foreach (var asset in allAssets)
            {
                ++index;
                EditorUtility.DisplayProgressBar("进度", asset, 1f * index / allAssets.Length);
                if (asset.StartsWith("Assets/") == false)
                    continue;

                Debug.Log($"asset={asset}");
                
                if (asset.IsContainDirectory("Plugins"))
                    continue; //过滤   Plugins 目录
                ++totalCount;

                if (asset.EndsWith(".cs"))
                    continue;


                AssetInfor infor = new AssetInfor();
                infor.mAssetRelativePath = asset;
                //infor.mAssetMd5 = md5;

                string fileRealPath = $"{IOUtility.GetFilePathParentDirectory(Application.dataPath, 1)}/{asset}";
                if (System.IO.File.Exists(fileRealPath))
                    infor.mAssetMd5 =MD5Helper.GetFileMD5(fileRealPath);
                else
                    continue;


                string[] depdences = AssetDatabase.GetDependencies(asset);
                if (depdences != null && depdences.Length > 1)
                {
                    foreach (var item in depdences)
                    {
                        if (item != asset && item.EndsWith(".cs") == false)
                            infor.mDepdenceInfor.Add(item);
                    }
                }

                if (asset.EndsWith(".prefab"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                    if (prefab != null)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(prefab, root.transform);
                        Transform[] childs = go.GetComponentsInChildren<Transform>();
                        foreach (var child in childs)
                        {
                            PrefabAssetNodeDetailInfor nodeInfor = new PrefabAssetNodeDetailInfor();
                            nodeInfor.mNodeToRootPath = child.GetPathToRoot(go.transform);
                            nodeInfor.mNodeName = child.gameObject.name;
                            nodeInfor.mNodeLayerName = LayerMask.LayerToName(child.gameObject.layer);

                            infor.mPrefabAssetNodesInfor.Add(nodeInfor);
                        }

                        GameObject.DestroyImmediate(go);
                    }
                }

                mAllPrefabsDepdence[asset] = infor;
            }

            GameObject.DestroyImmediate(root);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间

            string content = JsonConvert.SerializeObject(mAllPrefabsDepdence);
            string fileSavePath = $"{IOUtility.GetFilePathParentDirectory(Application.dataPath, 1)}/{mAssetsDepdenceInforLog}";
            IOUtility.CreateOrSetFileContent(fileSavePath, content);
            Debug.Log("fileSavePath=" + fileSavePath);
            Debug.Log("缓存资源依赖信息到目录 " + mAssetsDepdenceInforLog);
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            Debug.Log($"一共获取了{totalCount}个资源,其中包含的依赖信息大于1个的详细信息的有{mAllPrefabsDepdence.Count}个,耗时{$"{timespan.Hours}H{timespan.Minutes}M{timespan.Seconds}s"}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error " + e.ToString());
            EditorUtility.ClearProgressBar();
            GameObject.DestroyImmediate(root);
        }

        //mAllPrefabsDepdence.Clear();
        //string[] assetGuidList = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });  //获取Resources下所有的预制体
        //foreach (var assetguid in assetGuidList)
        //{
        //    string assetPath = AssetDatabase.GUIDToAssetPath(assetguid);
        //    string[] depdence = AssetDatabase.GetDependencies(assetPath);  //获取当前资源的依赖
        //    mAllPrefabsDepdence[assetPath] = new List<string>(depdence);
        //}
        //Debug.Log(string.Format("获取{0}下{1}个预制体的依赖信息", paths[0], mAllPrefabsDepdence.Count));
    }

    /// <summary>
    /// 获取参数图片资源被哪些资源引用
    /// </summary>
    /// <param name="imagAsset">key 资源路径  Value 哪些资源引用</param>
    /// <returns></returns>
    private static Dictionary<string, List<string>> GetImageAssetReference(List<string> imagAsset)
    {
        if (mAllPrefabsDepdence.Count == 0)
        {
            GetAllPrefabRelatveInfor();
            // GetAllPrefabRelatveInfor(new string[] { "Assets/Resources" });
        }


        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>(imagAsset.Count);

        foreach (var imgItem in imagAsset)
        {
            List<string> reference = new List<string>(10); //那些预制体资源引用
            foreach (var prefabsAsset in mAllPrefabsDepdence)
            {
                if (prefabsAsset.Value.mDepdenceInfor.Contains(imgItem))
                {
                    reference.Add(prefabsAsset.Key);
                }
            }

            result[imgItem] = reference;
        }

        return result;
    }


    /// <summary>
    /// 获取参数资源被哪些资源引用
    /// </summary>
    /// <param name="imagAsset">key 资源路径  Value 哪些资源引用</param>
    /// <returns></returns>
    private static List<string> GetAssetReference(string assetPath)
    {
        if (mAllPrefabsDepdence.Count == 0)
        {
            GetAllPrefabRelatveInfor();

            //   GetAllPrefabRelatveInfor(new string[] { "Assets/Resources" });
        }

        List<string> reference = new List<string>(10); //那些预制体资源引用
        foreach (var prefabsAsset in mAllPrefabsDepdence)
        {
            if (prefabsAsset.Value.mDepdenceInfor.Contains(assetPath))
            {
                reference.Add(prefabsAsset.Key);
            }
        }

        return reference;
    }


    /// <summary>
    /// 显示所有的结果
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }

            Debug.Log("End---------------------Asset;" + item.Key);
        }
    }

    /// <summary>
    /// 显示所有的结果
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference(string assetPath)
    {
        List<string> result = GetAssetReference(assetPath);
        foreach (var item in result)
        {
            Debug.Log(string.Format("资源{0,30} 被引用资源{1}", assetPath, item));
        }
    }

    /// <summary>
    /// 只显示无引用的
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference_NoReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        int count = 0;
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;
            ++count;
            Debug.Log(string.Format("没有被引用的资源 AssetName={0,-30}   Path={1,-100};", System.IO.Path.GetFileName(item.Key), item.Key));
        }

        Debug.Log("完成显示无引用的图片信息 总共=" + count);
    }

    /// <summary>
    /// 只显示无引用的
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference_NoReference(string assetPath)
    {
        List<string> result = GetAssetReference(assetPath);
        foreach (var item in result)
        {
            Debug.Log(string.Format("没有被引用的资源 AssetName={0,-30}   Path={1,-100};", System.IO.Path.GetFileName(assetPath), assetPath));
        }

        Debug.Log("完成显示无引用的图片信息 总共=" + result.Count);
    }

    /// <summary>
    /// 只显示有引用的
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference_Reference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count == 0)
                continue;

            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }

            Debug.Log("End---------------------Asset;" + item.Key);
        }

        Debug.Log("完成显示有引用的图片信息");
    }

    /// <summary>
    /// 只显示有引用的
    /// </summary>
    /// <param name="assetPath"></param>
    private static void ShowAllImageReference_Reference(string assetPath)
    {
        List<string> result = GetAssetReference(assetPath);
        foreach (var item in result)
        {
            Debug.Log(string.Format("资源{0,-30} 被引用{1}", assetPath, item));
        }

        Debug.Log("完成显示有引用的图片信息");
    }

    /// <summary>
    /// 显示所欲没有被引用的资源，并且自动设置格式
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAndAutoSetAllImageReference_NoReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;

            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }

            Debug.Log("End---------------------Asset;" + item.Key);
        }


        int count = 0;
        //***设置导入格式
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;
            ++count;
            TextureImporter impoter = AssetImporter.GetAtPath(item.Key) as TextureImporter;
            if (impoter.textureType != TextureImporterType.Default)
            {
                impoter.textureType = TextureImporterType.Default;
                impoter.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("一共设置了" + count + "个资源的格式");
    }

    /// <summary>
    /// 显示所有没有被引用的资源，并且自动删除
    /// </summary>
    /// <param name="imagAsset"></param>
    private static int ShowAndAutoDeleteAllImageReference_NoReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;

            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }

            Debug.Log("End---------------------Asset;" + item.Key);
        }


        int count = 0;
        //***删除无用资源
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;
            ++count;
            AssetDatabase.DeleteAsset(item.Key);
        }

        AssetDatabase.Refresh();
        Debug.Log("格式为Spriet删除了" + count + "个资源");
        return count;
    }

    /// <summary>
    /// 显示所欲没有被引用的资源，并且自动设置格式
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAndAutoSetAllImageReference_NoReference(string assetPath)
    {
        List<string> result = GetAssetReference(assetPath);
        foreach (var item in result)
        {
            Debug.Log(string.Format("资源{0,-30} 被引用{1}", assetPath, item));
        }


        //***设置导入格式
        foreach (var item in result)
        {
            TextureImporter impoter = AssetImporter.GetAtPath(item) as TextureImporter;
            if (impoter.textureType != TextureImporterType.Default)
            {
                impoter.textureType = TextureImporterType.Default;
                impoter.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("一共设置了" + result.Count + "个资源的格式");
    }

    #endregion
}