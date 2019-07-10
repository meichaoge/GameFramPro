using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

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
            RecordAllBuildAssetBundleContainAssetInfor();

            //    AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
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
        /// 记录所有的AssetBundle 中包含的资源信息
        /// </summary>
        private static void RecordAllBuildAssetBundleContainAssetInfor()
        {
            string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();

        }

    }
}