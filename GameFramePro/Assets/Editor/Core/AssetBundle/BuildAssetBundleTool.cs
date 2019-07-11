using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 提供打包生成AssetBundel 的接口
    /// </summary>
    internal static class BuildAssetBundleTool
    {

        /// <summary>
        /// 根据选择的不同平台打包生成AssetBundle  
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="assetBundleOptions"></param>
        /// <param name="targetPlatforms"></param>
        public static void BeginBuildAssetBundle(string outputPath, BuildAssetBundleOptions assetBundleOptions, List<AppPlatformEnum> targetPlatformEnums)
        {
            foreach (var platform in targetPlatformEnums)
            {
                outputPath = System.IO.Path.Combine(outputPath, AppPlatformManager.GetPlatformFolderName(platform));
                BuildTarget targetPlatform = AppPlatformManager.GetBuildTargetFromAppPlatformEnum(platform);
                BuildTargetGroup targetGroup = AppPlatformManager.GetBuildTargetGroupFromAppPlatformEnum(platform);
                if (BuildPipeline.IsBuildTargetSupported(targetGroup, targetPlatform) == false)
                {
                    Debug.LogEditorError("BeginBuildAssetBundle ,Not Support Platform:" + targetPlatform);
                    continue;
                }

                if (EditorUserBuildSettings.activeBuildTarget != targetPlatform)
                {
                    if (EditorUtility.DisplayDialog("切换平台提示", string.Format("生成AssetBundle ,目标平台:{0}  当前平台：{1} ，是否需要切换到对应的平台",
                        targetPlatform, EditorUserBuildSettings.activeBuildTarget), "切换", "取消") == false)
                    {
                        continue;
                    } //取消切换平台
                    EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, targetPlatform);
                }
                BuildAssetBundleOfTargetPlatform(outputPath, assetBundleOptions, targetPlatform);
            }
        }


        private static void BuildAssetBundleOfTargetPlatform(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            if (IOUtility.CheckOrCreateDirectory(outputPath) == false)
                return;
            RemoveAllUnUseAssetBundleNames();
            RemoveAllEmptyFoldeAssetBundleNames();

            var containAsset = IOUtility.GetDirectoriesAndFilesExculde(outputPath, "*.*", System.IO.SearchOption.AllDirectories, null);
            if (containAsset.Length > 0)
            {
                if(EditorUtility.DisplayDialog("提示",string.Format("保存生成AssetBundle 目录中包含 {0} 个资源，是否先删除文件内容", containAsset.Length), "删除存在文件", "忽略"))
                {
                    IOUtility.ClearDirectory(outputPath);
                }
            }

            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
            Debug.Log(string.Format("生成 {0} 平台下AssetBundle 成功，保存在目录 {1}", targetPlatform, outputPath));
            RecordAllBuildAssetBundleContainAssetInfor(outputPath, assetBundleManifest);
        }

        /// <summary>
        /// 删除无效的AssetBundleNames
        /// </summary>
        private static void RemoveAllUnUseAssetBundleNames()
        {
            string[] allUnUsetAssetBundleNames = AssetDatabase.GetUnusedAssetBundleNames();
            if (allUnUsetAssetBundleNames == null || allUnUsetAssetBundleNames.Length == 0)
                return;
            foreach (var unUseAssetBundleName in allUnUsetAssetBundleNames)
            {
                if (AssetDatabase.RemoveAssetBundleName(unUseAssetBundleName, false) == false)
                    Debug.LogEditorError(string.Format("RemoveAllUnUseAssetBundleNames Fail,unUseAssetBundleName={0}", unUseAssetBundleName));
            }
        }
        /// <summary>
        /// 移除空的AssetBundle 
        /// </summary>
        private static void RemoveAllEmptyFoldeAssetBundleNames()
        {
            string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var assetBundleName in allAssetBundleNames)
            {
                string[] allContainAssetPathInfors = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName); //当前这个 AssetBundle 中包含的资源
                if (allContainAssetPathInfors == null || allContainAssetPathInfors.Length == 0)
                {
                    Debug.LogEditorInfor("RemoveAllEmptyFoldeAssetBundleNames EmptyAssetBundle " + assetBundleName);
                    AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
                }
            }
        }

        /// <summary>
        /// 记录所有的AssetBundle 中包含的资源信息
        /// </summary>
        private static void RecordAllBuildAssetBundleContainAssetInfor(string outputPath, AssetBundleManifest assetBundleManifest)
        {
            EditorTotalAssetBundleInfor taotalAssetBundleInfor = new EditorTotalAssetBundleInfor();
            string[] allAssetBundlePackage = assetBundleManifest.GetAllAssetBundles();
            foreach (var assetBundlePackage in allAssetBundlePackage)
            {
                EditorAssetBundleInfor assetBundlePackInfor = new EditorAssetBundleInfor(); //一个包的信息
                assetBundlePackInfor.mAssetBundlePackageName = assetBundlePackage;

                string assetBundlePackagePath = outputPath.CombinePathEx(assetBundlePackage);
                assetBundlePackInfor.mMD5Code = MD5Helper.GetFileMD5(assetBundlePackagePath,ref assetBundlePackInfor.mPackageSize); //当前AssetBundle 包大小

                string[] allContainAssetInfor = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundlePackage);
                foreach (var containAsset in allContainAssetInfor)
                {
                    EditorAssetBundleAssetInfor assetInfor = new EditorAssetBundleAssetInfor();
                    assetInfor.mAssetRelativePath = containAsset.GetPathFromSpecialDirectoryName(ConstDefine.S_ResourcesName,false);
                    assetInfor.mMD5Code = MD5Helper.GetFileMD5(ConstDefine.S_ResourcesRealPath.CombinePathEx(assetInfor.mAssetRelativePath),ref assetInfor.mFileAssetSize);

                    assetBundlePackInfor.mAllContainAssetInfor.Add(assetInfor);
                    Debug.LogEditorInfor("assetInfor=" + assetInfor);
                } //包含的资源

                string[] depdences = assetBundleManifest.GetAllDependencies(assetBundlePackage);
                foreach (var depInfor in depdences)
                {
                    Debug.Log("depInfor=" + depInfor);
                }

                string[] direcDepdences = assetBundleManifest.GetAllDependencies(assetBundlePackage);
                foreach (var directDepInfor in direcDepdences)
                {
                    Debug.Log("directDepInfor=" + directDepInfor);
                }



                Debug.Log(assetBundlePackInfor);

            }
        }

    }
}