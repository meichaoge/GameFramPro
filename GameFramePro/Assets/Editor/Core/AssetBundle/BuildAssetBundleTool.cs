using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;
using Newtonsoft.Json;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 提供打包生成AssetBundel 的接口
    /// </summary>
    internal static class BuildAssetBundleTool
    {



        /// <summary>
        /// 保存生成的AssetBundel 信息
        /// </summary>
        public  static string S_BuildAssetBundleTotalRecordPath { get { return "Editor/Core/AssetBundle/BuildAssetBundleTotalRecord"; } }
        public  static string S_BuildAssetBundleTotalRecordRealPath { get { return Application.dataPath.CombinePathEx(S_BuildAssetBundleTotalRecordPath); } }  // 真是路径 （不带扩展名）
        //需要导出的配置文件的相对目录
        public static string S_AssetBundleExportConfigRelativePath { get { return ConstDefine. S_AssetBundleDirectoryName.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName); } } 



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
                outputPath = System.IO.Path.Combine(outputPath, AppPlatformManager.GetPlatformFolderName(platform)).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
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
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".initialed..", 0.1f);
            RemoveAllUnUseAssetBundleNames();
            RemoveAllEmptyFoldeAssetBundleNames();

            var containAsset = IOUtility.GetDirectoriesAndFilesExculde(outputPath, "*.*", System.IO.SearchOption.AllDirectories, null);
            if (containAsset.Length > 0)
            {
                if (EditorUtility.DisplayDialog("提示", string.Format("保存生成AssetBundle 目录中包含 {0} 个资源，是否先删除文件内容", containAsset.Length), "删除存在文件", "忽略"))
                {
                    IOUtility.ClearDirectory(outputPath);
                }
            }
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".Build..", 0.5f);
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
            Debug.Log(string.Format("生成 {0} 平台下AssetBundle 成功，保存在目录 {1}", targetPlatform, outputPath));
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理日志记录..", 0.5f);
            string assetBundleConfigPath = RecordAllBuildAssetBundleContainAssetInfor(outputPath, assetBundleManifest);
          //  ExportAssetBundleInforToCsv(assetBundleConfigPath, targetPlatform);  //目前不适用csv  避免自己解析 TODO
            ExportAssetBundleInforToJson(assetBundleConfigPath, targetPlatform);
            EditorUtility.ClearProgressBar();
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
        private static string  RecordAllBuildAssetBundleContainAssetInfor(string outputPath, AssetBundleManifest assetBundleManifest)
        {
            EditorTotalAssetBundleInfor totalAssetBundleInfor = new EditorTotalAssetBundleInfor();
            string[] allAssetBundlePackage = assetBundleManifest.GetAllAssetBundles();
            foreach (var assetBundlePackage in allAssetBundlePackage)
            {
                EditorAssetBundleInfor assetBundlePackInfor = new EditorAssetBundleInfor(); //一个包的信息
                assetBundlePackInfor.mAssetBundlePackageName = assetBundlePackage;

                string assetBundlePackagePath = outputPath.CombinePathEx(assetBundlePackage);
                assetBundlePackInfor.mMD5Code = MD5Helper.GetFileMD5(assetBundlePackagePath, ref assetBundlePackInfor.mPackageSize); //当前AssetBundle 包大小

                string[] allContainAssetInfor = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundlePackage);
                foreach (var containAsset in allContainAssetInfor)
                {
                    EditorAssetBundleAssetInfor assetInfor = new EditorAssetBundleAssetInfor();
                    assetInfor.mAssetRelativePath = containAsset.GetPathFromSpecialDirectoryName(ConstDefine.S_ResourcesName, false);
                    assetInfor.mMD5Code = MD5Helper.GetFileMD5(ConstDefine.S_ResourcesRealPath.CombinePathEx(assetInfor.mAssetRelativePath), ref assetInfor.mFileAssetSize);

                    assetBundlePackInfor.mAllContainAssetInfor.Add(assetInfor);
                    //Debug.LogEditorInfor("assetInfor=" + assetInfor);
                } //包含的资源

                string[] depdences = assetBundleManifest.GetAllDependencies(assetBundlePackage);
                foreach (var depInfor in depdences)
                {
                    Debug.Log("depInfor=" + depInfor);
                }

                string[] direcDepdences = assetBundleManifest.GetDirectDependencies(assetBundlePackage);
                foreach (var directDepInfor in direcDepdences)
                {
                    Debug.Log("directDepInfor=" + directDepInfor);
                }


                // Debug.Log(assetBundlePackInfor);
                totalAssetBundleInfor.mTotalSize += assetBundlePackInfor.mPackageSize;
                totalAssetBundleInfor.mTotalAssetBundleInfor[assetBundlePackInfor.mAssetBundlePackageName] = assetBundlePackInfor;
            }

            DateTime dataTimeRecord = DateTime.UtcNow.TruncatedDataTime_Minute();

            totalAssetBundleInfor.mConfigBuildTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            string content = SerilazeManager.SerializeObject(totalAssetBundleInfor);
            string realPath = S_BuildAssetBundleTotalRecordRealPath.ConcatPathEx(dataTimeRecord.ToString("_MM_dd_HH_mm")).ConcatPathEx(ConstDefine.S_TextAssetExtension);

            IOUtility.CreateOrSetFileContent(realPath, content);
            Debug.LogEditorInfor("生成AssetBundle 信息成功！！保存在目录 {0}", realPath);
            return realPath;

        }




        #region 读写生成的配置文件
        /// <summary>
        /// 将生成的AssetBundle 文件导出成 csv文件
        /// </summary>
        public static void ExportAssetBundleInforToCsv(string assetBundleConfigPath, BuildTarget targetPlatform)
        {
            string configContent;
            if (IOUtility.GetFileContent(assetBundleConfigPath, out configContent) == false)
                return;
            EditorTotalAssetBundleInfor totalAssetBundleInfor = SerilazeManager.DeserializeObject<EditorTotalAssetBundleInfor>(configContent);

            StringBuilder builder = StringUtility.GetStringBuilder();

            #region 开始几行数据
            CsvUtility.WriteCsv(totalAssetBundleInfor.mVersion, ref builder);  //版本号
            CsvUtility.WriteCsv(totalAssetBundleInfor.mTotalSize, ref builder);  //字节大小
            CsvUtility.WriteCsv(totalAssetBundleInfor.mConfigBuildTime, ref builder); //创建时间
            CsvUtility.WriteCsv_NewLine( ref builder); //换行
            #endregion

            #region 每个AB 信息
            foreach (var assetBundle in totalAssetBundleInfor.mTotalAssetBundleInfor.Values)
            {
                CsvUtility.WriteCsv(assetBundle.mAssetBundlePackageName, ref builder);  //AssetBundle Name
                CsvUtility.WriteCsv(assetBundle.mPackageSize, ref builder);  //AssetBundle 包大小 单位byte
                CsvUtility.WriteCsv(assetBundle.mAllContainAssetInfor.Count, ref builder);  //AssetBundle 包含的资源个数
                CsvUtility.WriteCsv_NewLine(ref builder); //换行

                foreach (var assetInfor in assetBundle.mAllContainAssetInfor)
                {
                    CsvUtility.WriteCsv(assetInfor.mAssetRelativePath, ref builder);  //Asset Path
                    CsvUtility.WriteCsv_NewLine(ref builder); //换行
                }
            }
            #endregion

            string csvContent = builder.ToString();
            string realPath = ConstDefine.S_ApplicationAssetParentRealPath.CombinePathEx(ConstDefine.S_ExportDirectoryName).
                CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(S_AssetBundleExportConfigRelativePath);
            IOUtility.CreateOrSetFileContent(realPath, csvContent,false);
            Debug.LogEditorInfor("ExportAssetBundleInforToCsv Success!! File Path is " + realPath);
            StringUtility.ReleaseStringBuilder(builder);
        }


        public static void ExportAssetBundleInforToJson(string assetBundleConfigPath, BuildTarget targetPlatform)
        {
            string configContent;
            if (IOUtility.GetFileContent(assetBundleConfigPath, out configContent) == false)
                return;
            EditorTotalAssetBundleInfor totalAssetBundleInfor = SerilazeManager.DeserializeObject<EditorTotalAssetBundleInfor>(configContent);

            LocalAssetBundleAssetTotalInfor localAssetBundleConfig = new LocalAssetBundleAssetTotalInfor();
            localAssetBundleConfig.mVersion = totalAssetBundleInfor.mVersion;
            localAssetBundleConfig.mConfigBuildTime = totalAssetBundleInfor.mConfigBuildTime;
            localAssetBundleConfig.mTotalSize = totalAssetBundleInfor.mTotalSize;
            foreach (var bundleInfor in totalAssetBundleInfor.mTotalAssetBundleInfor.Values)
            {
                LocalAssetBundleInfor assetBundleInfor = new LocalAssetBundleInfor();
                assetBundleInfor.mBundleName = bundleInfor.mAssetBundlePackageName;
                assetBundleInfor.mBundleSize = bundleInfor.mPackageSize;
                assetBundleInfor.mBundleAssetsCount = bundleInfor.mAllContainAssetInfor.Count;
                foreach (var assetInfor in bundleInfor.mAllContainAssetInfor)
                {
                    assetBundleInfor.mContainAssetPathInfor.Add(assetInfor.mAssetRelativePath);
                }

                localAssetBundleConfig.mTotalAssetBundleInfor[assetBundleInfor.mBundleName] = assetBundleInfor;
            }

            string content = SerilazeManager.SerializeObject(localAssetBundleConfig);
            string realPath = ConstDefine.S_ApplicationAssetParentRealPath.CombinePathEx(ConstDefine.S_ExportDirectoryName).
              CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(S_AssetBundleExportConfigRelativePath);
            IOUtility.CreateOrSetFileContent(realPath, content, false);
            Debug.LogEditorInfor("ExportAssetBundleInforToJson Success!! File Path is " + realPath);

        }

        #endregion

    }
}