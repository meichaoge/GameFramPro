using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameFramePro
{
    /// <summary>/// 对 System.IO 名称空间下类进行扩展/// </summary>
    public static class IOUtility
    {
        #region 文件创建、追加

        /// <summary>/// 创建或者追加内容/// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="content">文本内容</param>
        /// <param name="isAppend">是否是追加模式</param>
        public static void CreateOrSetFileContent(string filePath, string content, bool isAppend = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("CreateOrSetFileContent Path is Null  ");
                return;
            }

            byte[] data = Encoding.UTF8.GetBytes(content);
            CreateOrSetFileContent(filePath, data, isAppend);
        }

        public static void CreateOrSetFileContent(string filePath, byte[] byteData, bool isAppend = false)
        {
            FileStream operateStream = null;
            try
            {
                string directionaryPath = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(directionaryPath))
                {
                    Debug.LogError("无法解析路径的目录" + filePath);
                    return;
                }

                if (Directory.Exists(directionaryPath) == false)
                    Directory.CreateDirectory(directionaryPath);

                if (File.Exists(filePath) && (isAppend == false))
                    operateStream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read, byteData.Length); //截断
                else
                    operateStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, byteData.Length); //打开或者创建

                if (isAppend)
                    operateStream.Write(byteData, (int) operateStream.Length, byteData.Length);
                else
                    operateStream.Write(byteData, 0, byteData.Length);

                operateStream.Flush(); // 刷新
            }
            catch (System.Exception e)
            {
                Debug.LogError("CreateOrSetFileContent  " + e);
            }
            finally
            {
                if (operateStream != null)
                    operateStream.Close();

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }

        /// <summary>/// 读取指定路径上文件的全部信息/// </summary>
        /// <returns>返回值标示是否读取生成</returns>
        public static bool GetFileContent(string filePath, out string content)
        {
            content = string.Empty;
            if (System.IO.File.Exists(filePath) == false)
            {
                Debug.LogError("GetFileContent Fail,Not Exit File Path " + filePath);
                return false;
            }

            content = System.IO.File.ReadAllText(filePath);
            return true;
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public static bool FileModifyName(string filePath, string newFileName)
        {
            if (System.IO.File.Exists(filePath) == false)
                return false;

            string newFilePath = System.IO.Path.GetDirectoryName(filePath).CombinePathEx(newFileName);
            if (System.IO.File.Exists(newFilePath))
            {
                Debug.LogError("存在同名的文件 ，无法修改文件名 " + newFilePath);
                return false;
            }

            System.IO.File.Move(filePath, newFileName);
            return true;
        }

        /// <summary>/// 删除指定的文件/// </summary>
        public static bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("DeleteFile Fail,Parameter is null");
                return false;
            }

            if (System.IO.File.Exists(filePath) == false)
            {
                Debug.LogError("DeleteFile Fail,File Not Exit " + filePath);
                return false;
            }

            System.IO.File.Delete(filePath);
            return true;
        }

        #endregion

        #region 目录操作 (获取父目录、判断是否包含子目录、创建和销毁目录)

        /// <summary>
        /// 获取指定目录下的第N级别的父目录
        /// </summary>
        /// <param name="currentPath"></param>
        /// <param name="parentDeeep">父目录的层次,=0时候返回自身，=1返回上一级目录，以此类推</param>
        /// <returns></returns>
        public static string GetFilePathParentDirectory(this string currentPath, int parentDeeep)
        {
            if (parentDeeep == 0)
                return currentPath;

            int currentDeep = 0;
            string parentPath = string.Empty;
            string tempPath = currentPath; //缓存需要上一次得到的父级目录地址
            while (currentDeep < parentDeeep)
            {
                tempPath = parentPath = Path.GetDirectoryName(tempPath);
                if (string.IsNullOrEmpty(tempPath))
                {
                    Debug.LogError("GetFilePathParentDirectory ,parentDeeep is much more than directory deep");
                    return string.Empty;
                } //层级太深了 无法获取

                ++currentDeep;
            }

            return parentPath;
        }


        /// <summary>
        /// 获取路径的第 parentDeep 个父目录的目录名 
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="parentDeep">默认=0时候表示是当前目录名</param>
        /// <returns></returns>
        public static string GetPathDirectoryNameByDeep(this string targetPath, uint parentDeep = 0)
        {
            if (string.IsNullOrEmpty(targetPath))
                return string.Empty;

            string currentPath = targetPath;
            string directoryName = string.Empty;
            uint deep = 0;
            bool isFirstLoop = true;
            while (deep <= parentDeep) //为了==0时候也能处理
            {
                if (string.IsNullOrEmpty(currentPath))
                    return string.Empty;

                if (isFirstLoop)
                {
                    isFirstLoop = false;
                    if (Path.HasExtension(currentPath)) //文件路径
                    {
                        currentPath = GetPathWithOutExtension(currentPath); //去掉所有的扩展名
                        currentPath = Path.GetDirectoryName(currentPath); //第一个处理文件路径需要取目录后再处理
                    }
                }

                directoryName = Path.GetFileName(currentPath);
                currentPath = Path.GetDirectoryName(currentPath);
                ++deep;
            }

            return directoryName;
        }


        /// <summary>
        ///  判断是否包含指定的目录
        /// </summary>
        /// <param name="currentPath">需要判断的路径</param>
        /// <param name="searchDirectoryName">需要查找的路径</param>
        /// <returns></returns>
        public static bool IsContainDirectory(this string currentPath, string searchDirectoryName)
        {
            if (string.IsNullOrEmpty(currentPath) || string.IsNullOrEmpty(searchDirectoryName))
                return false;


            string targetPath = currentPath;
            string directotyName = GetFileNameWithoutExtensionEx(System.IO.Path.GetDirectoryName(targetPath));
            while (directotyName != searchDirectoryName)
            {
                if (string.IsNullOrEmpty(targetPath))
                    return false;

                targetPath = System.IO.Path.GetDirectoryName(targetPath);
                if (string.IsNullOrEmpty(targetPath))
                    return false;
                directotyName = GetFileNameWithoutExtensionEx(System.IO.Path.GetDirectoryName(targetPath));
            }

            return true;
        }

        /// <summary>
        /// 判断指定目录是否存在 如果不存在则创建
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool CheckOrCreateDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("CheckOrCreateDirectory Fail,Parameter directoryPath is null or empty ");
                return false;
            }

            bool isDirectoryExit = System.IO.Directory.Exists(directoryPath);
            if (isDirectoryExit == false)
                Directory.CreateDirectory(directoryPath);

            return true;
        }

        /// <summary>
        /// / 清空目录中所有的资源
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="isAutoCreate">如果不存在时候自动创建</param>
        /// <returns></returns>
        public static bool ClearOrCreateDirectory(string directoryPath, bool isAutoCreate = true)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("ClearDirectory Fail,Parameter directoryPath is null or empty ");
                return false;
            }

            if (System.IO.Directory.Exists(directoryPath) == false)
            {
                if (isAutoCreate)
                    System.IO.Directory.CreateDirectory(directoryPath);
                //  Debug.LogError(string.Format("ClearDirectory Fail, directoryPath[ {0} ] is not exit ", directoryPath));
                return false;
            }

            System.IO.Directory.Delete(directoryPath, true);
            System.IO.Directory.CreateDirectory(directoryPath);
            return true;
        }


        /// <summary>
        ///  复制文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        /// <param name="exculdeExtension">需要过滤哪些扩展名文件</param>
        public static void CopyDirectory(string sourcePath, string destinationPath, string[] exculdeExtension)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            //Directory.CreateDirectory(destinationPath);
            ClearOrCreateDirectory(destinationPath);

            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = Path.Combine(destinationPath, fsi.Name);
                if (exculdeExtension != null && exculdeExtension.Length != 0)
                {
                    string extensionName = Path.GetExtension(destName);
                    if (exculdeExtension.Contains(extensionName))
                        continue;
                }

                if (fsi is System.IO.FileInfo) //如果是文件，复制文件
                {
                    if (System.IO.File.Exists(destName))
                        System.IO.File.Delete(destName);
                    File.Copy(fsi.FullName, destName);
                }
                else //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName, exculdeExtension);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>

        #endregion

        #region 获取目录下的文件和子目录(可以过滤部分扩展名)

        /// <summary>/// 获取指定目录下的文件 不包含指定类型的扩展名/// </summary>
        /// <param name="exculdeExtension">当这个参数为null 或者长度为0 时候等同于Directory.GetFiles(path, searchPattern, searchOption)</param>
        public static string[] GetFilesExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension)
        {
            if (System.IO.Directory.Exists(path) == false)
            {
                Debug.LogError($"GetFilesExculde Fail,Not Exit Directory :{path}");
                return null;
            }

            string[] allFiles = Directory.GetFiles(path, searchPattern, searchOption);
            if (exculdeExtension == null || exculdeExtension.Length == 0)
                return allFiles;
            List<string> allFilesList = allFiles.ToList();
            List<string> extensions = exculdeExtension.ToList();
            allFilesList.RemoveAll(item => { return extensions.Contains(Path.GetExtension(item)); });
            return allFilesList.ToArray();
        }

        /// <summary>///  获取指定目录下所有的文件和文件夹目录/// </summary>
        /// <param name="exculdeExtension">需要过滤掉那些文件扩展名</param>
        /// <param name="isRelativePath">返回结果中是否是相对于path 的相对路径</param>
        /// <param name="directoryIndex">目录在返回结果中的结束索引(等于-1标示不存在，否则为目录总个数)</param>
        public static string[] GetDirectoriesAndFilesExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension, out int directoryIndex, bool isRelativePath = false)
        {
            directoryIndex = -1;
            if (System.IO.Directory.Exists(path) == false)
            {
                Debug.LogError($"GetFilesExculde Fail,Not Exit Directory :{path}");
                return new string[0];
            }
            //  Debug.LogEditorInfor("GetFiles_DirectoryExculde  path=" + path);

            string[] allFiles = Directory.GetFiles(path, searchPattern, searchOption);
            string[] allDirectorys = Directory.GetDirectories(path, searchPattern, searchOption);

            int pathLength = path.Length; //为了获取相对路径
            if (path.EndsWith(@"\") || path.EndsWith(@"\\") || path.EndsWith(@"/") || path.EndsWith(@"//") == false)
            {
                ++pathLength;
            } // 处理path路径结尾没有带目录结束符, 预留一个目录符号位置

            #region 没有需要过滤的扩展名

            if (exculdeExtension == null || exculdeExtension.Length == 0)
            {
                string[] allFilesAndDirectorys = new string[allFiles.Length + allDirectorys.Length];
                if (isRelativePath)
                {
                    for (int dex = 0; dex < allFiles.Length; dex++)
                        allFiles[dex] = allFiles[dex].Substring(pathLength);

                    for (int dex = 0; dex < allDirectorys.Length; dex++)
                        allDirectorys[dex] = allDirectorys[dex].Substring(pathLength);
                }

                System.Array.Copy(allDirectorys, 0, allFilesAndDirectorys, 0, allDirectorys.Length);
                System.Array.Copy(allFiles, 0, allFilesAndDirectorys, allDirectorys.Length, allFiles.Length);
                directoryIndex = allDirectorys.Length; //记录目录结束索引
                return allFilesAndDirectorys;
            }

            #endregion

            #region 按照指定的扩展名过滤结果

            List<string> allExtension = new List<string>(exculdeExtension);
            List<string> tempFilesAndDirectorys = new List<string>(allDirectorys.Length + allFiles.Length);
            for (int dex = 0; dex < allDirectorys.Length; dex++)
            {
                var directory = allDirectorys[dex];
                if (allExtension.Contains(System.IO.Path.GetExtension(directory)))
                    continue;
                if (isRelativePath == false)
                    tempFilesAndDirectorys.Add(directory);
                else
                    tempFilesAndDirectorys.Add(directory.Substring(pathLength));
            }

            if (tempFilesAndDirectorys.Count > 0)
                directoryIndex = tempFilesAndDirectorys.Count;

            for (int dex = 0; dex < allFiles.Length; dex++)
            {
                var directory = allFiles[dex];
                if (allExtension.Contains(System.IO.Path.GetExtension(directory)))
                    continue;
                if (isRelativePath == false)
                    tempFilesAndDirectorys.Add(directory);
                else
                    tempFilesAndDirectorys.Add(directory.Substring(pathLength));
            }

            return tempFilesAndDirectorys.ToArray();

            #endregion
        }

        /// <summary>
        ///  获取指定目录下所有的文件和文件夹目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="exculdeExtension">需要过滤掉那些文件扩展名</param>
        /// <param name="isRelativePath">返回结果中是否是相对于path 的相对路径</param>
        /// <returns></returns>
        public static string[] GetDirectoriesAndFilesExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension, bool isRelativePath = false)
        {
            int directoryIndex = -1;
            return GetDirectoriesAndFilesExculde(path, searchPattern, searchOption, exculdeExtension, out directoryIndex, isRelativePath);
        }


        /// <summary>
        /// 指定目录下目录结构个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exculdeExtension"></param>
        /// <returns></returns>
        public static int GetContainDirectoryExculde(this string path, string[] exculdeExtension)
        {
            if (System.IO.Directory.Exists(path) == false)
            {
                Debug.LogErrorFormat("GetContainDirectoryExculde Fail,Not Exit Directory :{0}", path);
                return 0;
            }

            string[] allDirectorys = Directory.GetDirectories(path);
            if (exculdeExtension == null || exculdeExtension.Length == 0)
                return allDirectorys.Length;

            int directoryCount = 0;
            List<string> allExculdeEstension = new List<string>(exculdeExtension);

            foreach (var item in allDirectorys)
            {
                if (allExculdeEstension.Contains(System.IO.Path.GetExtension(item)))
                    continue;
                ++directoryCount;
            }

            return directoryCount;
        }

        #endregion


        #region 路径处理

        /// <summary>/// 获取从某个指定的文件夹名开始的目录/// </summary>
        public static string GetPathFromSpecialDirectoryName(this string targetPath, string directoryName, bool isIncludeDirectoryName = true)
        {
            if (string.IsNullOrEmpty(targetPath)) return string.Empty;
            if (string.IsNullOrEmpty(directoryName)) return targetPath;

            List<string> allDirectoryNames = new List<string>();
            string currentPath = targetPath; // System.IO.Path.GetDirectoryName(targetPath);
            string currentDirectoryName = Path.GetFileName(currentPath);
            bool isFirstLoop = true;
            while (true)
            {
                if (isFirstLoop)
                {
                    isFirstLoop = false;
                    if (GetFileNameWithoutExtensionEx(currentDirectoryName) != directoryName)
                    {
                        allDirectoryNames.Add(currentDirectoryName);
                    }
                    else
                    {
                        if (isIncludeDirectoryName)
                            allDirectoryNames.Add(currentDirectoryName);
                        break;
                    }
                }
                else
                {
                    if (currentDirectoryName != directoryName)
                    {
                        allDirectoryNames.Add(currentDirectoryName);
                    }
                    else
                    {
                        if (isIncludeDirectoryName)
                            allDirectoryNames.Add(currentDirectoryName);
                        break;
                    }
                }

                currentPath = Path.GetDirectoryName(currentPath);
                currentDirectoryName = Path.GetFileName(currentPath);

                if (string.IsNullOrEmpty(currentPath))
                    return string.Empty;
            }

            allDirectoryNames.Reverse();
            StringBuilder builder = StringUtility.GetStringBuilder();
            for (int dex = 0; dex < allDirectoryNames.Count; dex++)
            {
                builder.Append(allDirectoryNames[dex]);
                if (dex != allDirectoryNames.Count - 1)
                    builder.Append(System.IO.Path.AltDirectorySeparatorChar);
            }

            string resultPath = builder.ToString();
            StringUtility.ReleaseStringBuilder(builder);
            return resultPath;
        }

        /// <summary>
        /// 扩展自 System.IO.Path.Combine(path1,path2) 实现路径组合
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string CombinePathEx(this string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }

        /// <summary>
        /// 直接组合两个字符串
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string ConcatPathEx(this string path1, string path2)
        {
            string resultStr = string.Empty;
            StringBuilder builder = StringUtility.GetStringBuilder();
            builder.Append(path1);
            builder.Append(path2);
            resultStr = builder.ToString();
            StringUtility.ReleaseStringBuilder(builder);
            return resultStr;
        }

        /// <summary>
        /// 获取指定路径不带扩展名的路径(从第一个.开始后面全部截断)
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static string GetPathWithOutExtension(this string targetPath, char extensionTag = '.')
        {
            if (string.IsNullOrEmpty(targetPath))
                return targetPath;
            int index = targetPath.IndexOf(extensionTag);
            if (index == -1)
                return targetPath;
            return targetPath.Substring(0, index);
        }

        /// <summary>
        /// 判断指定的路径是否是目录(如果这个路径不存在会返回false)
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static bool IsDirectoryPath(this string targetPath)
        {
            FileInfo fileInfor = new FileInfo(targetPath);
            if (fileInfor == null)
            {
                Debug.LogError("IsDirectoryPath Fail,Not Exit path " + targetPath);
                return false;
            }

            if ((fileInfor.Attributes & FileAttributes.Directory) != 0)
                return true;
            return false;
        }

        /// <summary>
        /// 扩展 Path.GetFileNameWithoutExtension 对于路径中包含多个扩展的情况处理
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="isRecursive">是否需要递归来得到不带任何扩展名的文件名，当值为false 时候与 Path.GetFileNameWithoutExtension() 结果相同</param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtensionEx(this string targetPath, bool isRecursive = true)
        {
            if (string.IsNullOrEmpty(targetPath))
                return targetPath;
            string pathInfor = Path.GetFileNameWithoutExtension(targetPath);
            if (isRecursive == false)
                return pathInfor;
            while (Path.HasExtension(pathInfor))
                pathInfor = Path.GetFileNameWithoutExtension(pathInfor);
            return pathInfor;
        }


        /// <summary>
        /// 忽略掉路径中的转移字符和目录字符
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="otherPath"></param>
        /// <returns></returns>
        public static bool ComparePathEx(this string targetPath, string otherPath)
        {
            string path1 = GetPathStringEx(targetPath);
            string path2 = GetPathStringEx(otherPath);
            return path1 == path2;
        }

        /// <summary>
        /// 获取不带路径分隔符(/or\or空白符)的字符串
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static string GetPathStringEx(this string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath) == false)
                return Regex.Replace(targetPath, @"[\\/\s]", string.Empty);
            return string.Empty;
        }

        #endregion


        /// <summary>/// byte 单位转换成B/KB/MB/GB单位/// </summary>
        /// <param name="isUptoConvert">=true  标示向上取整，=false 则四舍五入</param>
        public static string ByteConversionOthers(int byteSize, bool isUptoConvert = false)
        {
            //转成Byte
            if (isUptoConvert)
                byteSize = Mathf.CeilToInt(byteSize / 8f);
            else
                byteSize = Mathf.FloorToInt(byteSize / 8f);

            string[] units = new string[] {"B", "KB", "MB", "GB", "TB"};
            int count = 0;
            while (byteSize >= 1024)
            {
                if (isUptoConvert)
                {
                    byteSize = Mathf.CeilToInt(byteSize / 1024f);
                }
                else
                {
                    byteSize = Mathf.FloorToInt(byteSize / 1024f);
                }

                count++;
            }

            return string.Format("{0}{1}", byteSize, units[count]);
        }
    }
}