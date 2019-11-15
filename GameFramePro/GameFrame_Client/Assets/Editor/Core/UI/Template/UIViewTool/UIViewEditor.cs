using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GameFramePro.UI;
using UnityEditor;
using UnityEngine;

//生成继承自Mono 的脚本
public class UIViewEditor : Editor
{
    public enum EmObjType
    {
        Transform,
        Gameobject,
        Component,
        RectTransform,
    }

    public struct TagObj
    {
        public EmObjType objType;
        public string objName;
        public string prefix;
        public string tplText;


        public TagObj(EmObjType objType, string objName, string prefix)
        {
            this.objType = objType;
            this.objName = objName;
            this.prefix = prefix;
            this.tplText = "";

            if (objType == EmObjType.Transform)
                this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\");\n";
            else if (objType == EmObjType.Gameobject)
                this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\").gameObject;\n";
            else if (objType == EmObjType.Component)
                this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\").gameObject.GetComponent<{0}>();\n";
            else if (objType == EmObjType.RectTransform)
                this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\") as RectTransform;\n";
        }
    }


    //key =tag name 
    public static Dictionary<string, TagObj> TagObjs = new Dictionary<string, TagObj>()
    {
        {UGUIComponentTypeEnum.GameObject.ToString(), new TagObj(EmObjType.Gameobject, "GameObject", "go")},
        {UGUIComponentTypeEnum.Transform.ToString(), new TagObj(EmObjType.Transform, "Transform", "tf")},
        {UGUIComponentTypeEnum.RectTransform.ToString(), new TagObj(EmObjType.RectTransform, "RectTransform", "rtf")},


        {UGUIComponentTypeEnum.UGUIText.ToString(), new TagObj(EmObjType.Component, "Text", "txt")},
        {UGUIComponentTypeEnum.UGUIImage.ToString(), new TagObj(EmObjType.Component, "Image", "img")},
        {UGUIComponentTypeEnum.UGUIButton.ToString(), new TagObj(EmObjType.Component, "Button", "btn")},
        {UGUIComponentTypeEnum.UGUIDropDown.ToString(), new TagObj(EmObjType.Component, "Dropdown", "drop")},
        {UGUIComponentTypeEnum.UGUIInputField.ToString(), new TagObj(EmObjType.Component, "InputField", "input")},

              {UGUIComponentTypeEnum.CanvasGroup.ToString(), new TagObj(EmObjType.Component, "CanvasGroup", "canvasGroup")},
           {UGUIComponentTypeEnum.UILoopScrollRect_Vertical.ToString(), new TagObj(EmObjType.Component, "LoopVerticalScrollRect", "loopVertical")},
           {UGUIComponentTypeEnum.UILoopScrollRect_Horizontial.ToString(), new TagObj(EmObjType.Component, "LoopHorizontalScrollRect", "loopHorizontial")},

    };

    public static string TemplatePath = Application.dataPath + "/Editor/Core/UI/Template"; //模本文件


    [MenuItem("Assets/工具/辅助/选中UI预制体，生成View代码")]
    public static void GenViewCode()
    {
        GameObject goSelected = Selection.activeGameObject;
        if (goSelected == null)
        {
            Debug.LogError("没有选中Prefab！");
            return;
        }

        string strTplPath = TemplatePath + "/" + "View.tpl.txt";
        if (!File.Exists(strTplPath))
        {
            Debug.LogError("模板文件不存在！");
            return;
        }

        string className = goSelected.name;
        string filePath = EditorUtility.SaveFilePanel("生成代码", Application.dataPath, className + ".cs", "cs");
        if (string.IsNullOrEmpty(filePath))
            return;

        className = Path.GetFileNameWithoutExtension(filePath);

        List<GameObject> goChilds = new List<GameObject>();
        List<Transform> goChildsTrans = goSelected.GetComponentsInChildren<Transform>(true).ToList();
        foreach (var item in goChildsTrans)
        {
//            if (item.gameObject != goSelected) ;  //2019/10/17 这个判断条件一直成立！！ 原因未知
            if (item.gameObject.GetInstanceID() != goSelected.GetInstanceID())
                goChilds.Add(item.gameObject);
        }

        goChilds.Insert(0, goSelected);

        StringBuilder uiParameter = new StringBuilder();
        StringBuilder viewTouiParameter = new StringBuilder();
        StringBuilder sbInitView = new StringBuilder();
        Dictionary<string, int> propertyNames = new Dictionary<string, int>();
        foreach (GameObject go in goChilds)
        {
            if (TagObjs.ContainsKey(go.tag))
            {
                TagObj tagObj = TagObjs[go.tag];
                string objName = tagObj.objName;
                string propertyName = tagObj.prefix + UnityExtentionMethod.FormatName(go.name);
                UnityExtentionMethod.UniqueName(ref propertyNames, ref propertyName);
                string path = go.transform.GetPath(goSelected.transform);
                string value = string.Format(tagObj.tplText, objName, propertyName, path);
                sbInitView.Append(value);
                uiParameter.Append("private " + objName + " m_" + propertyName + " ;\n");
                viewTouiParameter.Append("m_" + propertyName + "=" + propertyName + ";\n");
            }
        }

        string strInitView = sbInitView.ToString();
        StreamReader sr = new StreamReader(strTplPath, Encoding.UTF8);
        string strTpl = sr.ReadToEnd();
        sr.Close();


        strTpl = Regex.Replace(strTpl, " #UIPARAMETER#", uiParameter.ToString());
        strTpl = Regex.Replace(strTpl, " #INITVIEWTOPARAMETER#", viewTouiParameter.ToString());

        strTpl = Regex.Replace(strTpl, "#CLASSNAME#", className);
        Debug.Log(className);
        strTpl = Regex.Replace(strTpl, "#INITVIEW#", strInitView);
        Debug.Log(strInitView);

        //拷贝到剪贴板
        TextEditor te = new TextEditor();
        te.text = strInitView;
        te.SelectAll();
        te.Copy();

        StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
        sw.Write(strTpl);
        sw.Close();

        AssetDatabase.Refresh();
        string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(assetRelativePath));
        Debug.Log("生成View代码完成！");
    }

    [MenuItem("Assets/工具/辅助/选中UI预制体，拷贝View代码到剪贴板")]
    public static void CopyViewCodeToClipbord()
    {
        GameObject goSelected = Selection.activeGameObject;
        if (goSelected == null)
        {
            Debug.LogError("没有选中Prefab！");
            return;
        }

        string strTplPath = TemplatePath + "/" + "View.tpl.txt";
        if (!File.Exists(strTplPath))
        {
            Debug.LogError("模板文件不存在！");
            return;
        }

        List<GameObject> goChilds = goSelected.GetChildCollectionRecursive();
        goChilds.Insert(0, goSelected);

        StringBuilder sbInitView = new StringBuilder();
        Dictionary<string, int> propertyNames = new Dictionary<string, int>();
        foreach (GameObject go in goChilds)
        {
            if (TagObjs.ContainsKey(go.tag))
            {
                TagObj tagObj = TagObjs[go.tag];
                string objName = tagObj.objName;
                string propertyName = tagObj.prefix + UnityExtentionMethod.FormatName(go.name);
                UnityExtentionMethod.UniqueName(ref propertyNames, ref propertyName);
                string path = go.transform.GetPath(goSelected.transform);
                string value = string.Format(tagObj.tplText, objName, propertyName, path);
                sbInitView.Append(value);
            }
        }

        string strInitView = sbInitView.ToString();
        StreamReader sr = new StreamReader(strTplPath, Encoding.UTF8);
        string strTpl = sr.ReadToEnd();
        sr.Close();

        strTpl = Regex.Replace(strTpl, "#INITVIEW#", strInitView);
        Debug.Log(strInitView);

        //拷贝到剪贴板
        TextEditor te = new TextEditor();
        te.text = strInitView;
        te.SelectAll();
        te.Copy();

        Debug.Log("拷贝View代码到剪贴板完成！");
    }

//    [MenuItem("Assets/工具/UI 视图/获取本地化配置")]
//    public static string GetLocalizationTextInfor()
//    {
//        GameObject goSelected = Selection.activeGameObject;
//        if (goSelected == null)
//        {
//            Debug.LogError("没有选中Prefab！");
//            return null;
//        }
//
//        string strTplPath = TemplatePath + "/" + "View.tpl.txt";
//        if (!File.Exists(strTplPath))
//        {
//            Debug.LogError("模板文件不存在！");
//            return null;
//        }
//
//        List<GameObject> goChilds = new List<GameObject>();
//        List<Transform> goChildsTrans = goSelected.GetComponentsInChildren<Transform>(true).ToList();
//        foreach (var item in goChildsTrans)
//        {
//            if (item.gameObject != goSelected) ;
//            goChilds.Add(item.gameObject);
//        }
//        goChilds.Insert(0, goSelected);
//        StringBuilder sbInitView = new StringBuilder();
//        Dictionary<string, int> propertyNames = new Dictionary<string, int>();
//        foreach (GameObject go in goChilds)
//        {
//            //***2019/5/6 只获取需要修改多语言配置的项
//            if (go.tag != "UI.Text")
//                continue;
//            Text goText = go.GetComponent<Text>();
//            if (string.IsNullOrEmpty(goText.text) || goText.text.StartsWith("@") == false)
//                continue;
//
//
//            if (TagObjs.ContainsKey(go.tag))
//            {
//                TagObj tagObj = TagObjs[go.tag];
//                string objName = tagObj.objName;
//                string propertyName = tagObj.prefix + Util.FormatName(go.name);
//                string path = go.transform.GetPath(goSelected.transform);
//                string localizationStr_Left = string.Format(tagObj.tplText, objName, propertyName, path);
//
//                sbInitView.Append(localizationStr_Left);
//                string value = string.Format("{0}.text=LanguageController.GetString(\"{1}\");", propertyName, goText.text);
//                sbInitView.Append(value);
//                sbInitView.Append(Environment.NewLine);
//                sbInitView.Append(Environment.NewLine);
//
//                string localizate = LanguageController.GetString(goText.text);
//                goText.text = localizate;
//            }
//        }
//
//        string strInitView = sbInitView.ToString();
//        GUIUtility.systemCopyBuffer = strInitView;  //复制到剪切板
//
//        Debug.Log("获取和替换本地化完成！"+ strInitView);
//        return strInitView;
//    }
}