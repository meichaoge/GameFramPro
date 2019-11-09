using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;
using GameFramePro.ResourcesEx;
using GameFramePro.Upgrade;
using System.Threading.Tasks;

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

        ////需要导出的配置文件的相对目录
        //public static string S_AssetBundleExportConfigRelativePath
        //{
        //    get { return ConstDefine.S_AssetBundleDirectoryName.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName); }
        //}

        private static AssetBundleAssetTotalInfor s_AssetBundleAssetTotalInfor = null;


        /// <summary>
        /// 根据选择的不同平台打包生成AssetBundle  
        /// </summary>
        /// <param name="selectedOutputPath">打包AssetBundle 保存的目录(Assets下某个目录)</param>
        /// <param name="outputPath">打包成功后会从 Application.StreamPath 复制到这里</param>
        /// <param name="assetBundleOptions"></param>
        /// <param name="targetPlatforms"></param>
        public static async Task BeginBuildAssetBundleAsync(string saveAssetBundlePath, BuildAssetBundleOptions assetBundleOptions, List<AppPlatformEnum> targetPlatformEnums, System.Action OnSuccessBuildAct = null)
        {
            foreach (var platform in targetPlatformEnums)
            {
                saveAssetBundlePath = System.IO.Path.Combine(saveAssetBundlePath, AppPlatformManager.GetPlatformFolderName(platform)).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
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

                await BuildAssetBundleOfTargetPlatformAsync(saveAssetBundlePath, assetBundleOptions, targetPlatform, OnSuccessBuildAct);
            }
        }

        /// <summary>
        /// 根据参数平台打包生成AssetBundel 资源
        /// </summary>
        /// <param name="createAssetBundleSavePath"></param>
        /// <param name="exportPath"></param>
        /// <param name="assetBundleOptions"></param>
        /// <param name="targetPlatform"></param>
        private static async Task BuildAssetBundleOfTargetPlatformAsync(string saveAssetBundlePath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform, System.Action OnSuccessBuildAct = null)
        {
            if (IOUtility.CheckOrCreateDirectory(saveAssetBundlePath) == false)
                return;

            #region 打包前的准备工作

            RemoveAllUnUseAssetBundleNames();
            if (CheckAssetBundleNameValid() == false)
                return;
            RemoveAllEmptyFoldeAssetBundleNames();

            var containAsset = IOUtility.GetDirectoriesAndFilesExculde(saveAssetBundlePath, "*.*", System.IO.SearchOption.AllDirectories, null);
            if (containAsset.Length > 0)
            {
                if (EditorUtility.DisplayDialog("提示", $"保存生成AssetBundle 目录中包含 {containAsset.Length} 个资源，是否先删除文件内容", "删除存在文件", "忽略") == false)
                {
                    Debug.LogEditorInfor($"取消打包AssetBundle");
                    return;
                }

                IOUtility.ClearOrCreateDirectory(saveAssetBundlePath);
            }

            #endregion

            await Task.Delay(TimeSpan.FromSeconds(0.1f));

            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".Prepare..", 0.1f);



            AssetBundleManifest assetBundleManifest = null;
            try
            {
                assetBundleManifest = BuildPipeline.BuildAssetBundles(saveAssetBundlePath, assetBundleOptions, targetPlatform);
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($" 打包失败，异常{e}");
                return;
            }

            Debug.Log($"生成 {targetPlatform} 平台下AssetBundle 成功，保存在目录 {saveAssetBundlePath}");
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".生成AssetBundle.....", 0.5f);
            #region 打包后的操作

            AssetDatabase.Refresh();
            Debug.Log($"生成 {targetPlatform} 平台下AssetBundle 成功 ");

            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理日志记录..", 0.6f);
            //     ReImportMainAssetBundle(saveAssetBundlePath); //修改没有扩展名的主AssetBundle 资源名
            string assetBundleConfigPath = RecordAllBuildAssetBundleContainAssetInfor(saveAssetBundlePath, assetBundleManifest);
            //  ExportAssetBundleInforToCsv(assetBundleConfigPath, targetPlatform);  //目前不适用csv  避免自己解析 TODO
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理日志记录..", 0.8f);
            AssetDatabase.Refresh();
            //    IOUtility.CopyDirectory(createAssetBundleSavePath, exportPath, new string[] { ConstDefine.S_MetaExtension, ConstDefine.S_AssetBundleManifestExtension });
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理日志记录..", 0.85f);
            ExportAssetBundleInforToJson(assetBundleConfigPath, targetPlatform); //在复制完文件资源之后再生成配置，否则会被删除



            await Task.Delay(TimeSpan.FromSeconds(1));
            EditorUtility.DisplayProgressBar("Create AssetBundle ..", "....开始压缩资源", 0.9f);
            string zipOutputPath = saveAssetBundlePath.GetFilePathParentDirectory(1).CombinePathEx("TempZip.zip");
            ZipUtility.Zip(new string[] { saveAssetBundlePath }, zipOutputPath);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            Debug.Log($"完成压缩AssetBundle {zipOutputPath}");
            #endregion


            EditorUtility.ClearProgressBar();


            OnSuccessBuildAct?.Invoke();
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
        /// 检查是否有资源的AssetBundleName 没有扩展名
        /// </summary>
        /// <returns></returns>
        private static bool CheckAssetBundleNameValid()
        {
            string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            bool isEnable = true;
            foreach (var assetBundleName in allAssetBundleNames)
            {
                if (System.IO.Path.GetExtension(assetBundleName) == string.Empty)
                {
                    isEnable = false;
                    string[] allContainAssetPathInfors = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName); //当前这个 AssetBundle 中包含的资源
                    foreach (var assetInfor in allContainAssetPathInfors)
                    {
                        Debug.LogError($"AssetBundle Name({assetBundleName}) 没有设置扩展名：{assetInfor}");
                    }
                }
            }

            return isEnable;
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
            s_AssetBundleAssetTotalInfor = new AssetBundleAssetTotalInfor();
            //        RecordMainAssetBundelInfor(saveAssetBundlePath);

            //**修改AssetBundle 资源名称 并记录相关的资源引用关系
            string[] allAssetBundlePackage = assetBundleManifest.GetAllAssetBundles();
            int dex = 0;
            foreach (var assetBundlePackageUri in allAssetBundlePackage)
            {
                dex++;
                EditorUtility.DisplayProgressBar("Create AssetBundle ..", ".处理依赖关系....." + assetBundlePackageUri, 0.7f + (dex * 0.11f / allAssetBundlePackage.Length));

                #region AssetBundle 大小+MD5+CRC+改名处理

                AssetBundleInfor assetBundlePackInfor = new AssetBundleInfor(); //一个包的信息
                assetBundlePackInfor.RelDirctory = System.IO.Path.GetDirectoryName(assetBundlePackageUri);

                string assetBundlePackagePath = saveAssetBundlePath.CombinePathEx(assetBundlePackageUri);
                //    Debug.LogInfor($"assetBundlePackageUri={assetBundlePackageUri}         assetBundlePackagePath={assetBundlePackagePath}");
                bool isSuccess = BuildPipeline.GetCRCForAssetBundle(assetBundlePackagePath, out uint crcCode);
                if (isSuccess == false)
                {
                    Debug.LogError($"RecordAllBuildAssetBundleContainAssetInfor Error  {assetBundlePackagePath}");
                    return null;
                }

                string md5Code = MD5Helper.GetFileMD5OutLength(assetBundlePackagePath, out long fileSize); //当前AssetBundle 包大小]
                                                                                                           ////为了避免小概率MD5 相同 是文件长度 +分割字符串+ CRC+分割字符串+MD5
                string newFileName = $"{fileSize}{ConstDefine.S_AssetBundleAssetNameSeparatorChar}{ crcCode}{ConstDefine.S_AssetBundleAssetNameSeparatorChar}{md5Code}";
                string newFilePath = IOUtility.FileModifyName(assetBundlePackagePath, newFileName);
                if (string.IsNullOrEmpty(newFilePath))
                {
                    Debug.LogError($"生成AssetBundle 失败");
                    return string.Empty;
                }

                string newAssetBundlePackageUri = newFilePath.Substring(saveAssetBundlePath.Length + 1); //新的资源路径
                                                                                                         //  Debug.Log($" assetBundlePackageUri={assetBundlePackageUri}  ====>>>>> newAssetBundlePackageUri={newAssetBundlePackageUri}");

                #endregion


                #region AssetBundle 包含的资源处理

                string[] allContainAssetInfor = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundlePackageUri);
                foreach (var containAssetUri in allContainAssetInfor)
                {
                    string assetRelativeResourcesUri = containAssetUri.GetPathFromSpecialDirectoryName(ConstDefine.S_ResourcesName, false).GetPathWithOutExtension();
                    string relativeUri = assetRelativeResourcesUri.GetFileNameWithoutExtensionEx().ToLower(); //这里不需要任何扩展名
                                                                                                              //   Debug.LogEditorInfor($"assetInfor={containAssetUri}     assetRelativeResourcesUri={assetRelativeResourcesUri}   relativeUri={relativeUri} ");
                    if (assetBundlePackInfor.ContainAssetReUri.TryGetValue(assetRelativeResourcesUri, out var assetNameUri))
                    {
                        if (assetNameUri != relativeUri)
                        {
                            Debug.LogError($"{assetBundlePackageUri} AssetBundle 中 已经包含了相对路径为{relativeUri} 的资源");
                            return string.Empty;
                        }
                    }

                    assetBundlePackInfor.ContainAssetReUri[assetRelativeResourcesUri] = relativeUri;
                } //包含的资源

                #endregion

                #region 依赖关系处理

                string[] dependences = assetBundleManifest.GetAllDependencies(assetBundlePackageUri);
                foreach (var depInfor in dependences)
                    Debug.LogError("depInfor=" + depInfor);

                string[] direcDependences = assetBundleManifest.GetDirectDependencies(assetBundlePackageUri);
                foreach (var directDepInfor in direcDependences)
                    Debug.LogError("directDepInfor=" + directDepInfor);

                #endregion

                // Debug.Log(assetBundlePackInfor);
                s_AssetBundleAssetTotalInfor.mTotalSize += fileSize;
                if (s_AssetBundleAssetTotalInfor.mTotalAssetBundleInfor.ContainsKey(newFileName))
                {
                    Debug.LogError($"包含重复的AssetBundle 配置{newFileName} ::{assetBundlePackInfor.RelDirctory}");
                    return string.Empty;
                }

                s_AssetBundleAssetTotalInfor.mTotalAssetBundleInfor[newFileName] = assetBundlePackInfor;
            }


            DeleteAllUnUsingMainfestFiles(saveAssetBundlePath);

            s_AssetBundleAssetTotalInfor.mConfigBuildTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            string content = SerializeManager.SerializeObject(s_AssetBundleAssetTotalInfor);
            // string realPath = S_BuildAssetBundleTotalRecordRealPath.ConcatPathEx(DateTime.Now.ToString("_yyyy_MM_dd_HH_mm")).ConcatPathEx(ConstDefine.S_TextAssetExtension);
            string realPath = S_BuildAssetBundleTotalRecordRealPath.ConcatPathEx(ConstDefine.S_TextAssetExtension);

            IOUtility.CreateOrSetFileContent(realPath, content);
            Debug.LogEditorInfor("生成AssetBundle 信息成功！！保存在目录 {0}", realPath);

            return realPath;
        }


        ///// <summary>
        ///// 记录主AssetBundel 和 AssetBundleManifest 资源的信息
        ///// </summary>
        //private static void RecordMainAssetBundelInfor(string saveAssetBundlePath)
        //{
        //    //主AssetBundle
        //    string mainAssetBundlePath = saveAssetBundlePath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName); //.ConcatPathEx(ConstDefine.S_AssetBundleExtension);
        //    AssetBundleInfor mainAssetBundleInfor = new AssetBundleInfor
        //    {
        //        mABundleUri = System.IO.Path.GetFileName(mainAssetBundlePath).ConcatPathEx(ConstDefine.S_AssetBundleExtension)
        //    };
        //    string mainAssetBundleUri = mainAssetBundlePath.ConcatPathEx(ConstDefine.S_AssetBundleExtension);
        //    string md5Code = MD5Helper.GetFileMD5OutLength(mainAssetBundleUri, out mainAssetBundleInfor.Size); //当前AssetBundle 包大小

        //    //TODO 10/28
        //    //string newFileName = UnityEngine.Random.Range(S_MinRandomRange, S_MaxRandomRange) + md5Code; //为了避免小概率MD5 相同，前面随机5位数
        //    //string originalFileName = IOUtility.GetFileNameWithoutExtensionEx(mainAssetBundleUri);
        //    //if (IOUtility.FileModifyName(mainAssetBundleUri, newFileName) == false)
        //    //{
        //    //    Debug.LogError($"生成AssetBundle 失败");
        //    //    return ;
        //    //}
        //    //md5CodeNameToOriginalName[originalFileName] = newFileName;

        //    bool isSuccess = BuildPipeline.GetCRCForAssetBundle(mainAssetBundlePath, out mainAssetBundleInfor.mBundleCRC);
        //    if (isSuccess == false)
        //        Debug.LogError("RecordAllBuildAssetBundleContainAssetInfor Error" + mainAssetBundlePath);

        //    s_AssetBundleAssetTotalInfor.mTotalSize += mainAssetBundleInfor.Size;
        //    s_AssetBundleAssetTotalInfor.mTotalAssetBundleInfor[mainAssetBundleInfor.mABundleUri] = mainAssetBundleInfor;

        //    //.manifest 文件
        //    //string manifestAssetBundelPath = saveAssetBundlePath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);//.ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension);
        //    //EditorAssetBundleInfor manifestAssetBundleInfor = new EditorAssetBundleInfor();
        //    //manifestAssetBundleInfor.mAssetBundlePackageName = System.IO.Path.GetFileName(manifestAssetBundelPath).ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension);
        //    //manifestAssetBundleInfor.mMD5Code = MD5Helper.GetFileMD5(manifestAssetBundelPath.ConcatPathEx(ConstDefine.S_AssetBundleManifestExtension), ref manifestAssetBundleInfor.mPackageSize); //当前AssetBundle 包大小
        //    //bool isSuccess2 = BuildPipeline.GetCRCForAssetBundle(manifestAssetBundelPath, out manifestAssetBundleInfor.mCRCCode);
        //    //if (isSuccess2 == false)
        //    //    Debug.LogError("RecordAllBuildAssetBundleContainAssetInfor Error" + manifestAssetBundelPath);

        //    //s_TotalAssetBundleInfor.mTotalSize += manifestAssetBundleInfor.mPackageSize;
        //    //s_TotalAssetBundleInfor.mTotalAssetBundleInfor[manifestAssetBundleInfor.mAssetBundlePackageName] = manifestAssetBundleInfor;
        //}

        /// <summary>
        /// 删除生成的AssetBundle 的资源 .manifest 文件
        /// </summary>
        private static void DeleteAllUnUsingMainfestFiles(string saveAssetBundlePath)
        {
            //删除主AssetBundle 
            string[] allFiles = System.IO.Directory.GetFiles(saveAssetBundlePath, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var mainAssetBundleFile in allFiles)
            {
                string fileName = System.IO.Path.GetFileName(mainAssetBundleFile);
                if (fileName == ConstDefine.S_AssetBundleDirectoryName)
                {
                    System.IO.File.Delete(mainAssetBundleFile);
                    Debug.LogInfor($"删除无用的主AssetBundle 资源 {mainAssetBundleFile}");
                }
            }

            //删除 ,manifest 文件
            allFiles = System.IO.Directory.GetFiles(saveAssetBundlePath, string.Format("*{0}", ConstDefine.S_AssetBundleManifestExtension), System.IO.SearchOption.AllDirectories);
            foreach (var mainifestFile in allFiles)
            {
                System.IO.File.Delete(mainifestFile);
            }
        }

        #endregion

        #region 读写生成的配置文件

        /// <summary>
        /// 将生成的AssetBundle 文件导出成 csv文件
        /// </summary>
        public static void ExportAssetBundleInforToCsv(string assetBundleConfigPath, BuildTarget targetPlatform)
        {
            // string configContent;
            // if (IOUtility.GetFileContent(assetBundleConfigPath, out configContent) == false)
            //     return;
            // AssetBundleAssetTotalInfor totalAssetBundleInfor = SerializeManager.DeserializeObject<AssetBundleAssetTotalInfor>(configContent);

            // StringBuilder builder = StringUtility.GetStringBuilder();

            // #region 开始几行数据

            // CsvUtility.WriteCsv(totalAssetBundleInfor.mVersion, ref builder); //版本号
            // CsvUtility.WriteCsv(totalAssetBundleInfor.mTotalSize, ref builder); //字节大小
            // CsvUtility.WriteCsv(totalAssetBundleInfor.mConfigBuildTime, ref builder); //创建时间
            // CsvUtility.WriteCsv_NewLine(ref builder); //换行

            // #endregion

            // #region 每个AB 信息

            // foreach (var assetBundle in totalAssetBundleInfor.mTotalAssetBundleInfor)
            // {
            //     CsvUtility.WriteCsv(assetBundle.mABundleUri, ref builder); //AssetBundle Name
            ////     CsvUtility.WriteCsv(assetBundle.Size, ref builder); //AssetBundle 包大小 单位byte
            //     CsvUtility.WriteCsv(assetBundle.ContainAssetReUri.Count, ref builder); //AssetBundle 包含的资源个数
            //     CsvUtility.WriteCsv_NewLine(ref builder); //换行

            //     foreach (var assetInfor in assetBundle.ContainAssetReUri)
            //     {
            //         CsvUtility.WriteCsv(assetInfor.Key, ref builder); //Asset Path
            //         CsvUtility.WriteCsv(assetInfor.Value, ref builder); //Asset Path
            //         CsvUtility.WriteCsv_NewLine(ref builder); //换行
            //     }
            // }

            // #endregion

            // string csvContent = builder.ToString();
            // string realPath = ConstDefine.S_ExportRealPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(S_AssetBundleExportConfigRelativePath);
            // IOUtility.CreateOrSetFileContent(realPath, csvContent, false);
            // Debug.LogEditorInfor("ExportAssetBundleInforToCsv Success!! File Path is " + realPath);
            // StringUtility.ReleaseStringBuilder(builder);
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
            AssetBundleAssetTotalInfor totalAssetBundleInfor = SerializeManager.DeserializeObject<AssetBundleAssetTotalInfor>(configContent);

            string content = SerializeManager.SerializeObject(totalAssetBundleInfor);
            string realPath = ConstDefine.S_ExportRealPath.CombinePathEx(AppPlatformManager.GetPlatformFolderName(targetPlatform)).CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
            IOUtility.CreateOrSetFileContent(realPath, content, false);
            Debug.LogEditorInfor("ExportAssetBundleInforToJson Success!! File Path is " + realPath);
        }

        #endregion
    }
}