using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;
using GameFramePro.ResourcesEx;
using GameFramePro.Upgrade;
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
        public static string S_BuildAssetBundleTotalRecordPath
        {
            get { return "Editor/Core/AssetBundle/BuildAssetBundleTotalRecord"; }
        }

        public static string S_BuildAssetBundleTotalRecordRealPath
        {
            get { return Application.dataPath.CombinePathEx(S_BuildAssetBundleTotalRecordPath); }
        } // 真是路径 （不带扩展名）

        //需要导出的配置文件的相对目录
        public static string S_AssetBundleExportConfigRelativePath
        {
            get { return ConstDefine.S_AssetBundleDirectoryName.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName); }
        }

        private static EditorTotalAssetBundleInfor s_TotalAssetBundleInfor = null;


        /// <summary>
        /// 根据选择的不同平台打包生成AssetBundle  
        /// </summary>
        /// <param name="selectedOutputPath">打包AssetBundle 保存的目录(Assets下某个目录)</param>
        /// <param name="outputPath">打包成功后会从 Application.StreamPath 复制到这里</param>
        /// <param name="assetBundleOptions"></param>
        /// <param name="targetPlatforms"></param>
        public static void BeginBuildAssetBundle(string exportPath, BuildAssetBundleOptions assetBundleOptions, List<AppPlatformEnum> targetPlatformEnums)
        {
            foreach (var platform in targetPlatformEnums)
            {
                exportPath = System.IO.Path.Combine(exportPath, AppPlatformManager.GetPlatformFolderName(platform)).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
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

                string createAssetBundleSavePath = Application.streamingAssetsPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(platform)).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                IOUtility.ClearOrCreateDirectory(createAssetBundleSavePath);
                BuildAssetBundleOfTargetPlatform(createAssetBundleSavePath, exportPath, assetBundleOptions, targetPlatform);
            }
        }

        /// <summary>
        /// 根据参数平台打包生成AssetBundel 资源
        /// </summary>
        /// <param name="createAssetBundleSavePath"></param>
        /// <param name="exportPath"></param>
        /// <param name="assetBundleOptions"></param>
        /// <param name="targetPlatform"></param>
        private static void BuildAssetBundleOfTargetPlatform(string createAssetBundleSavePath, string exportPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            if (IOUtility.CheckOrCreateDirectory(exportPath) == false)
                return;
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".Prepare..", 0.1f);

            #region 打包前的准备工作

            RemoveAllUnUseAssetBundleNames();
            RemoveAllEmptyFoldeAssetBundleNames();

            var containAsset = IOUtility.GetDirectoriesAndFilesExculde(exportPath, "*.*", System.IO.SearchOption.AllDirectories, null);
            if (containAsset.Length > 0)
            {
                if (EditorUtility.DisplayDialog("提示", $"保存生成AssetBundle 目录中包含 {containAsset.Length} 个资源，是否先删除文件内容", "删除存在文件", "忽略"))
                {
                    IOUtility.ClearOrCreateDirectory(exportPath);
                }
            }

            #endregion

            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".Build..", 0.5f);
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(createAssetBundleSavePath, assetBundleOptions, targetPlatform);
            Debug.Log($"生成 {targetPlatform} 平台下AssetBundle 成功，保存在目录 {createAssetBundleSavePath}");

            #region 打包后的操作

            AssetDatabase.Refresh();

            #region 重命名主AssetBundle 避免移动文件时候报错

            #endregion

            Debug.Log($"生成 {targetPlatform} 平台下AssetBundle 成功，复制到输出目录 {exportPath}");

            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理日志记录..", 0.7f);
            ReImportMainAssetBundle(createAssetBundleSavePath); //修改没有扩展名的主AssetBundle 资源名
            string assetBundleConfigPath = RecordAllBuildAssetBundleContainAssetInfor(createAssetBundleSavePath, assetBundleManifest);
            //  ExportAssetBundleInforToCsv(assetBundleConfigPath, targetPlatform);  //目前不适用csv  避免自己解析 TODO
            AssetDatabase.Refresh();
            IOUtility.CopyDirectory(createAssetBundleSavePath, exportPath, new string[] {ConstDefine.S_MetaExtension, ConstDefine.S_AssetBundleManifestExtension});

            ExportAssetBundleInforToJson(assetBundleConfigPath, targetPlatform); //在复制完文件资源之后再生成配置，否则会被删除

            EditorUtility.ClearProgressBar();

            #endregion
        }

        #region 打包准备+打包后的文件处理

        /// <summary>
        /// 将打包后的主AssetBundle 文件重命名
        /// </summary>
        /// <param name="createAssetBundleSavePath"></param>
        private static void ReImportMainAssetBundle(string createAssetBundleSavePath)
        {
            string filePath = createAssetBundleSavePath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
            if (System.IO.File.Exists(filePath))
            {
                string newName = System.IO.Path.ChangeExtension(filePath, ConstDefine.S_AssetBundleExtension);
                System.IO.File.Move(filePath, newName);
                AssetDatabase.Refresh();
            }
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
                    Debug.LogEditorError($"RemoveAllUnUseAssetBundleNames Fail,unUseAssetBundleName={unUseAssetBundleName}");
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
        private static string RecordAllBuildAssetBundleContainAssetInfor(string saveAssetBundlePath, AssetBundleManifest assetBundleManifest)
        {
            s_TotalAssetBundleInfor = new EditorTotalAssetBundleInfor();

            RecordMainAssetBundelInfor(saveAssetBundlePath);


            string[] allAssetBundlePackage = assetBundleManifest.GetAllAssetBundles();
            foreach (var assetBundlePackage in allAssetBundlePackage)
            {
                EditorAssetBundleInfor assetBundlePackInfor = new EditorAssetBundleInfor(); //一个包的信息
                assetBundlePackInfor.mAssetBundlePackageName = assetBundlePackage;

                string assetBundlePackagePath = saveAssetBundlePath.CombinePathEx(assetBundlePackage);
                assetBundlePackInfor.mMD5Code = MD5Helper.GetFileMD5OutLength(assetBundlePackagePath, out assetBundlePackInfor.mPackageSize); //当前AssetBundle 包大小
                bool isSuccess = BuildPipeline.GetCRCForAssetBundle(assetBundlePackagePath, out assetBundlePackInfor.mCRCCode);
                if (isSuccess == false)
                    Debug.LogError("RecordAllBuildAssetBundleContainAssetInfor Error" + assetBundlePackagePath);


                string[] allContainAssetInfor = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundlePackage);
                foreach (var containAsset in allContainAssetInfor)
                {
                    EditorAssetBundleAssetInfor assetInfor = new EditorAssetBundleAssetInfor();
                    string assetRelativePath = containAsset.GetPathFromSpecialDirectoryName(ConstDefine.S_ResourcesName, false);
                    assetInfor.mAssetRelativePath = assetRelativePath.GetPathWithOutExtension(); //这里不需要任何扩展名
                    assetInfor.mMD5Code = MD5Helper.GetFileMD5OutLength(ConstDefine.S_ResourcesRealPath.CombinePathEx(assetRelativePath), out assetInfor.mFileAssetSize);

                    assetBundlePackInfor.mAllContainAssetInfor.Add(assetInfor);
                    //Debug.LogEditorInfor("assetInfor=" + assetInfor);
                } //包含的资源

                string[] dependences = assetBundleManifest.GetAllDependencies(assetBundlePackage);
                foreach (var depInfor in dependences)
                {
                    Debug.Log("depInfor=" + depInfor);
                }

                string[] direcDependences = assetBundleManifest.GetDirectDependencies(assetBundlePackage);
                foreach (var directDepInfor in direcDependences)
                {
                    Debug.Log("directDepInfor=" + directDepInfor);
                }

                // Debug.Log(assetBundlePackInfor);
                s_TotalAssetBundleInfor.mTotalSize += assetBundlePackInfor.mPackageSize;
                s_TotalAssetBundleInfor.mTotalAssetBundleInfor[assetBundlePackInfor.mAssetBundlePackageName] = assetBundlePackInfor;
            }


            s_TotalAssetBundleInfor.mConfigBuildTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            string content = SerializeManager.SerializeObject(s_TotalAssetBundleInfor);
            string realPath = S_BuildAssetBundleTotalRecordRealPath.ConcatPathEx(DateTime.Now.ToString("_yyyy_MM_dd_HH_mm")).ConcatPathEx(ConstDefine.S_TextAssetExtension);

            IOUtility.CreateOrSetFileContent(realPath, content);
            Debug.LogEditorInfor("生成AssetBundle 信息成功！！保存在目录 {0}", realPath);
            return realPath;
        }

        /// <summary>
        /// 记录主AssetBundel 和 AssetBundleManifest 资源的信息
        /// </summary>
        private static void RecordMainAssetBundelInfor(string saveAssetBundlePath)
        {
            //主AssetBundle
            string mainAssetBundlePath = saveAssetBundlePath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName); //.ConcatPathEx(ConstDefine.S_AssetBundleExtension);
            EditorAssetBundleInfor mainAssetBundleInfor = new EditorAssetBundleInfor
            {
                mAssetBundlePackageName = System.IO.Path.GetFileName(mainAssetBundlePath).ConcatPathEx(ConstDefine.S_AssetBundleExtension)
            };
            mainAssetBundleInfor.mMD5Code = MD5Helper.GetFileMD5OutLength(mainAssetBundlePath.ConcatPathEx(ConstDefine.S_AssetBundleExtension), out mainAssetBundleInfor.mPackageSize); //当前AssetBundle 包大小
            bool isSuccess = BuildPipeline.GetCRCForAssetBundle(mainAssetBundlePath, out mainAssetBundleInfor.mCRCCode);
            if (isSuccess == false)
                Debug.LogError("RecordAllBuildAssetBundleContainAssetInfor Error" + mainAssetBundlePath);

            s_TotalAssetBundleInfor.mTotalSize += mainAssetBundleInfor.mPackageSize;
            s_TotalAssetBundleInfor.mTotalAssetBundleInfor[mainAssetBundleInfor.mAssetBundlePackageName] = mainAssetBundleInfor;

            //.manifest 文件
            //string manifestAssetBundelPath = saveAssetBundlePath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);//.ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension);
            //EditorAssetBundleInfor manifestAssetBundleInfor = new EditorAssetBundleInfor();
            //manifestAssetBundleInfor.mAssetBundlePackageName = System.IO.Path.GetFileName(manifestAssetBundelPath).ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension);
            //manifestAssetBundleInfor.mMD5Code = MD5Helper.GetFileMD5(manifestAssetBundelPath.ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension), ref manifestAssetBundleInfor.mPackageSize); //当前AssetBundle 包大小
            //bool isSuccess2 = BuildPipeline.GetCRCForAssetBundle(manifestAssetBundelPath, out manifestAssetBundleInfor.mCRCCode);
            //if (isSuccess2 == false)
            //    Debug.LogError("RecordAllBuildAssetBundleContainAssetInfor Error" + manifestAssetBundelPath);

            //s_TotalAssetBundleInfor.mTotalSize += manifestAssetBundleInfor.mPackageSize;
            //s_TotalAssetBundleInfor.mTotalAssetBundleInfor[manifestAssetBundleInfor.mAssetBundlePackageName] = manifestAssetBundleInfor;
        }

        #endregion

        #region 读写生成的配置文件

        /// <summary>
        /// 将生成的AssetBundle 文件导出成 csv文件
        /// </summary>
        public static void ExportAssetBundleInforToCsv(string assetBundleConfigPath, BuildTarget targetPlatform)
        {
            string configContent;
            if (IOUtility.GetFileContent(assetBundleConfigPath, out configContent) == false)
                return;
            EditorTotalAssetBundleInfor totalAssetBundleInfor = SerializeManager.DeserializeObject<EditorTotalAssetBundleInfor>(configContent);

            StringBuilder builder = StringUtility.GetStringBuilder();

            #region 开始几行数据

            CsvUtility.WriteCsv(totalAssetBundleInfor.mVersion, ref builder); //版本号
            CsvUtility.WriteCsv(totalAssetBundleInfor.mTotalSize, ref builder); //字节大小
            CsvUtility.WriteCsv(totalAssetBundleInfor.mConfigBuildTime, ref builder); //创建时间
            CsvUtility.WriteCsv_NewLine(ref builder); //换行

            #endregion

            #region 每个AB 信息

            foreach (var assetBundle in totalAssetBundleInfor.mTotalAssetBundleInfor.Values)
            {
                CsvUtility.WriteCsv(assetBundle.mAssetBundlePackageName, ref builder); //AssetBundle Name
                CsvUtility.WriteCsv(assetBundle.mPackageSize, ref builder); //AssetBundle 包大小 单位byte
                CsvUtility.WriteCsv(assetBundle.mAllContainAssetInfor.Count, ref builder); //AssetBundle 包含的资源个数
                CsvUtility.WriteCsv_NewLine(ref builder); //换行

                foreach (var assetInfor in assetBundle.mAllContainAssetInfor)
                {
                    CsvUtility.WriteCsv(assetInfor.mAssetRelativePath, ref builder); //Asset Path
                    CsvUtility.WriteCsv_NewLine(ref builder); //换行
                }
            }

            #endregion

            string csvContent = builder.ToString();
            string realPath = ConstDefine.S_ExportRealPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(S_AssetBundleExportConfigRelativePath);
            IOUtility.CreateOrSetFileContent(realPath, csvContent, false);
            Debug.LogEditorInfor("ExportAssetBundleInforToCsv Success!! File Path is " + realPath);
            StringUtility.ReleaseStringBuilder(builder);
        }

        /// <summary>
        /// 导出AsetBundle 配置到Json文件中
        /// </summary>
        /// <param name="assetBundleConfigPath"></param>
        /// <param name="targetPlatform"></param>
        public static void ExportAssetBundleInforToJson(string assetBundleConfigPath, BuildTarget targetPlatform)
        {
            if (IOUtility.GetFileContent(assetBundleConfigPath, out var configContent) == false)
                return;
            EditorTotalAssetBundleInfor totalAssetBundleInfor = SerializeManager.DeserializeObject<EditorTotalAssetBundleInfor>(configContent);

            AssetBundleAssetTotalInfor localAssetBundleConfig = new AssetBundleAssetTotalInfor();
            localAssetBundleConfig.mVersion = totalAssetBundleInfor.mVersion;
            localAssetBundleConfig.mConfigBuildTime = totalAssetBundleInfor.mConfigBuildTime;
            localAssetBundleConfig.mTotalSize = totalAssetBundleInfor.mTotalSize;
            
            foreach (var bundleInfor in totalAssetBundleInfor.mTotalAssetBundleInfor.Values)
            {
                string assetBundleNameStr = bundleInfor.mAssetBundlePackageName.GetPathStringEx();
                
                AssetBundleInfor assetBundleInfor = new AssetBundleInfor();
                assetBundleInfor.mAssetBundleUri = bundleInfor.mAssetBundlePackageName;
                assetBundleInfor.mAssetRelativeAssetBundleUri = bundleInfor.mAssetRelativePath; //相对路径
                assetBundleInfor.mBundleSize = bundleInfor.mPackageSize;
                assetBundleInfor.mBundleAssetsCount = bundleInfor.mAllContainAssetInfor.Count;
                assetBundleInfor.mBundleMD5Code = bundleInfor.mMD5Code;
                assetBundleInfor.mBundleCRC = bundleInfor.mCRCCode;


                foreach (var assetInfor in bundleInfor.mAllContainAssetInfor)
                {
                    assetBundleInfor.mContainAssetPathInfor.Add(assetInfor.mAssetRelativePath);
                } //获取包含的资源信息

                assetBundleInfor.mDependenceAssetBundleInfor = new string[bundleInfor.mDependenceAssetBundle.Count];
                for (int dex = 0; dex < bundleInfor.mDependenceAssetBundle.Count; dex++)
                {
                    assetBundleInfor.mDependenceAssetBundleInfor[dex] = bundleInfor.mDependenceAssetBundle[dex].mAssetBundlePackageName;
                } //获取依赖信息 这里不是处理之后的AssetBundle Name

                localAssetBundleConfig.mTotalAssetBundleInfor[assetBundleNameStr] = assetBundleInfor; //这里的key 不带路径分隔符
            }

            string content = SerializeManager.SerializeObject(localAssetBundleConfig);
            string realPath = ConstDefine.S_ExportRealPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(S_AssetBundleExportConfigRelativePath);
            IOUtility.CreateOrSetFileContent(realPath, content, false);
            Debug.LogEditorInfor("ExportAssetBundleInforToJson Success!! File Path is " + realPath);
        }

        #endregion
    }
}
