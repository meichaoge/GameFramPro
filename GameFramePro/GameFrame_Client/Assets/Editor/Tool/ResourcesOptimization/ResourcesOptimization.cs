using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using System.Text;
using System;
using GameFramePro;
using GameFramePro.EditorEx;
using GameFramePro.UI;

namespace GameFramePro.EditorEx
{
    //资源优化管理
    public class ResourcesOptimization
    {
        #region 编辑器菜单

        #region 图片格式优化

        [MenuItem("Assets/工具和扩展/资源优化/图片被引用预制体关联Image格式(九宫格 Slider)")]
        private static void AutoSetImgReferencePrefabType_Sliced()
        {
            AutoSetImgReferencePrefabType(Image.Type.Sliced);
        }

        [MenuItem("Assets/工具和扩展/资源优化/图片被引用预制体关联Image格式(九宫格 Tilied)")]
        private static void AutoSetImgReferencePrefabType_Tilied()
        {
            AutoSetImgReferencePrefabType(Image.Type.Tiled);
        }

        //设置所有引用当前图片的预制体上image 格式
        private static void AutoSetImgReferencePrefabType(Image.Type type)
        {
            var selectobjs = Selection.objects;
            if (selectobjs.Length == 0)
                return;

            if (selectobjs.Length > 1)
            {
                Debug.LogError("只能选择一个图片");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
            if (TextureAssetEditor.CheckIsTexture(assetPath))
            {
                List<string> result = EditorAssetReferenceManager.GetAssetReference(assetPath);
                foreach (var item in result)
                {
                    Debug.Log($"资源{assetPath,-30} 被预制体引用{item}");
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

            Debug.LogError("选择的资源不是图片 " + assetPath);
        }


        [MenuItem("Assets/工具和扩展/资源优化/图片被引用预制体关联Image格式(水平分成2个)")]
        private static void AutoSetImgReferencePrefab_Horizontial()
        {
            var selectobjs = Selection.objects;
            if (selectobjs.Length == 0)
                return;

            if (selectobjs.Length > 1)
            {
                Debug.LogError("只能选择一个图片");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
            if (TextureAssetEditor.CheckIsTexture(assetPath))
            {
                List<string> result = EditorAssetReferenceManager.GetAssetReference(assetPath);
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

            Debug.LogError("选择的资源不是图片 " + assetPath);
        }

        #endregion

        #region 图片替换

        [MenuItem("Assets/工具和扩展/资源优化/替换选中图片")]
        private static void AutoReplaceImgWithSelected()
        {
            var selectobjs = Selection.objects;
            if (selectobjs.Length == 0)
                return;

            if (selectobjs.Length != 2)
            {
                Debug.LogError("请选择两张图片");
                return;
            }

            string oldAssetPath = AssetDatabase.GetAssetPath(selectobjs[0]); //旧资源
            string newAssetPath = AssetDatabase.GetAssetPath(selectobjs[0]); //替换的新资源

            if (TextureAssetEditor.CheckIsTexture(oldAssetPath))
            {
                Debug.LogError($"选择的第一个资源(旧的需要替换的资源)不是图片格式");
                return;
            }

            if (TextureAssetEditor.CheckIsTexture(newAssetPath))
            {
                Debug.LogError($"选择的第二个资源(新的替换的资源)不是图片格式");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", $"是否确定使用{newAssetPath} 替换{oldAssetPath} 在所有预制体中的引用", "替换", "取消") == false)
                return;

            Sprite newTargetSprite = AssetDatabase.LoadAssetAtPath<Sprite>(newAssetPath);
            if (newTargetSprite == null)
            {
                Debug.LogError($"请先导入{newAssetPath} 这个新资源，并这是好相应的格式后重试");
                return;
            }

            List<string> result = EditorAssetReferenceManager.GetAssetReference(oldAssetPath);
            foreach (var item in result)
            {
                Debug.Log($"资源{oldAssetPath,-30} 被预制体引用{item}");
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
                    if (path == oldAssetPath)
                    {
                        img.sprite = newTargetSprite;
                        isNeedSave = true;
                        Debug.Log($"修改了节点{img.gameObject.name} image 格式为新的资源");
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
        }

        #endregion

        #endregion
    }
}