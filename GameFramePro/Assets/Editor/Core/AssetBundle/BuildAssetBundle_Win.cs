using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>/// 打包生成AssetBundle/// </summary>
    public class BuildAssetBundle_Win : EditorWindow
    {
        [MenuItem("工具和扩展/AssetBundle/创建 AssetBundle 资源")]
        private static void CreateAssetBundleWin()
        {
            BuildAssetBundle_Win win = EditorWindow.GetWindow<BuildAssetBundle_Win>("Create AssetBundle Window");
            win.minSize = new Vector2(600, 800);
            win.maxSize = new Vector2(800, 800);
            win.Show();
            win.Initialed();
        }


        #region 数据

        private AppPlatformEnum mSelectAppPlatform = AppPlatformEnum.Windows64; //选择的平台
        private BuildAssetBundleOptions mSelectBuildAssetBundleOptions = BuildAssetBundleOptions.None; // 打包AssetBundle 设置
        private string mSelectAssetBundleSavePath = string.Empty; //保存生成的AssetBundle 目录

        private AssetDirectoryTreeView mAssetDirectoryTreeView = null;
        private Vector2 mResourcesTreeViewScrollPos = Vector2.zero;

        private TextAsset mTotalAssetBundleInforConfig = null;

        private string outPutPath
        {
            get { return Application.streamingAssetsPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(mSelectAppPlatform)).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName); }
        }

        #endregion

        //初始化
        private void Initialed()
        {
            mSelectAssetBundleSavePath = ConstDefine.S_ExportRealPath;
            mAssetDirectoryTreeView = new AssetDirectoryTreeView();

            InitialedTreeView();
        }


        private void OnGUI()
        {
            GUILayout.BeginVertical("box");

            #region 打包设置

            GUILayout.BeginVertical("box");
            GUILayout.Space(10);

            #region 打包平台设置

            GUILayout.BeginHorizontal();
            GUILayout.Label("选择需要生成AssetBundel 的平台:", GUILayout.Width(200));
            mSelectAppPlatform = (AppPlatformEnum) EditorGUILayout.EnumPopup(mSelectAppPlatform, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            #endregion

            #region 打包AssetBundle 参数配置

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("生成AssetBundel 的配置:", GUILayout.Width(200));
            mSelectBuildAssetBundleOptions = (BuildAssetBundleOptions) EditorGUILayout.EnumPopup(mSelectBuildAssetBundleOptions, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            #endregion

            #region 打包AssetBundle 参数配置

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("保存生成的AssetBundel 的路径:", GUILayout.Width(200));
            mSelectAssetBundleSavePath = EditorGUILayout.TextField(mSelectAssetBundleSavePath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择目录", GUILayout.Width(50)))
            {
                string savePath = EditorUtility.SaveFolderPanel("选择保存生成AssetBundle 的路径", mSelectAssetBundleSavePath, string.Empty);
                if (string.IsNullOrEmpty(savePath) == false)
                {
                    mSelectAssetBundleSavePath = savePath;
                    Debug.LogEditorInfor(string.Format("选择保存生成AssetBundel 路径为 {0}", mSelectAssetBundleSavePath));
                }
            }

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("生成AssetBunde 资保存在Assets相对路径", GUILayout.Width(180));
            GUILayout.Label(outPutPath.GetPathFromSpecialDirectoryName(ConstDefine.S_AssetsName), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(10);
            GUILayout.EndVertical();

            #endregion

            #region 配置信息

            GUILayout.Space(8);
            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("上一次生成AssetBundle 配置", GUILayout.Width(180));
            mTotalAssetBundleInforConfig = (TextAsset) EditorGUILayout.ObjectField("", mTotalAssetBundleInforConfig, typeof(TextAsset), false);
            GUILayout.EndHorizontal();


            GUILayout.Space(8);
            GUILayout.EndVertical();

            #endregion

            #region 显示Resources 树形目录

            if (mAssetDirectoryTreeView != null)
            {
                mResourcesTreeViewScrollPos = GUILayout.BeginScrollView(mResourcesTreeViewScrollPos);

                mAssetDirectoryTreeView.DrawTreeView(-1, true);
                GUILayout.EndScrollView();
            }

            #endregion

            #region 操作

            GUILayout.Space(10);
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("生成AssetBundle", GUILayout.Width(150), GUILayout.Height(35)))
            {
                BuildAssetBundleTool.BeginBuildAssetBundle(mSelectAssetBundleSavePath, mSelectBuildAssetBundleOptions, new List<AppPlatformEnum> {mSelectAppPlatform});
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            #endregion

            GUILayout.EndVertical();
        }


        #region 辅助

        private void InitialedTreeView()
        {
            int directoryIndex = -1;
            string rootPath = ConstDefine.S_ResourcesRealPath; //根路径
            string[] allResourcesRootFilesAndDirectory = IOUtility.GetDirectoriesAndFilesExculde(rootPath, "*.*", System.IO.SearchOption.TopDirectoryOnly, new string[] {ConstDefine.S_MetaExtension}, out directoryIndex, true);
            mAssetDirectoryTreeView.InitialTreeView(null);

            for (int dex = 0; dex < allResourcesRootFilesAndDirectory.Length; dex++)
            {
                var treeNodePath = allResourcesRootFilesAndDirectory[dex];
                AssetStateNodeInfor node = mAssetDirectoryTreeView.GetTreeNode(treeNodePath, treeNodePath, null, true) as AssetStateNodeInfor;
                mAssetDirectoryTreeView.AddRootTreeNode(node);
            } //构建初始的根节点

            if (directoryIndex > 0)
            {
                TreeNodePathRecursiveHandler<AssetStateNodeInfor> handler = (in AssetStateNodeInfor treeNode, out IEnumerable<string> allSubTreeNode, out int recursiveCount) =>
                {
                    string realPath = System.IO.Path.Combine(ConstDefine.S_ResourcesRealPath, treeNode.TreeNodePathRelativeRoot); // string.Format("{0}/{1}", ConstDefine.S_ResourcesRealPath, treeNode.TreeNodePathRelativeRoot);
                    allSubTreeNode = IOUtility.GetDirectoriesAndFilesExculde(realPath, "*.*", System.IO.SearchOption.TopDirectoryOnly, new string[] {ConstDefine.S_MetaExtension}, out recursiveCount, true);
                };

                for (int dex = 0; dex < directoryIndex; dex++)
                {
                    AssetStateNodeInfor parentRootTreeNode = mAssetDirectoryTreeView.AllRootTreeNodes[dex];

                    TreeViewUtility<AssetStateNodeInfor>.CreateTreeNodeByPath(parentRootTreeNode, handler, (path) => { return mAssetDirectoryTreeView.GetTreeNode(path, path, parentRootTreeNode, true) as AssetStateNodeInfor; }, true);
                }
            }

            //       mAssetDirectoryTreeView.ShowTreeViewNodes();
        }

        #endregion
    }
}
