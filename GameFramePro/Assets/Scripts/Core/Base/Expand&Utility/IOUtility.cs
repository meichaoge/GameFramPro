using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;

public static class IOUtility
{
    /// <summary>
    /// 创建或者追加内容
    /// </summary>
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


            int buffersize = 1024;
            buffersize = Mathf.Max(buffersize, data.Length);  //防止content为null时候buffer=0报错

            if (File.Exists(filePath) && (isAppend == false))
                operateStream = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Read, buffersize);//截断
            else
                operateStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, buffersize);//打开或者创建

            if (isAppend)
                operateStream.Write(data, (int)operateStream.Length, data.Length);
            else
                operateStream.Write(data, 0, data.Length);

            operateStream.Flush();// 刷新
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

    /// <summary>
    /// byte 单位转换成B/KB/MB/GB单位
    /// </summary>
    /// <param name="byteSize"></param>
    /// <param name="isUptoConvert">=true  标示向上取整，=false 则四舍五入</param>
    /// <returns></returns>
    public static string ByteConversionOthers(int byteSize, bool isUptoConvert = false)
    {
        //转成Byte
        if (isUptoConvert)
            byteSize = Mathf.CeilToInt(byteSize / 8f);
        else
            byteSize = Mathf.FloorToInt(byteSize / 8f);

        string[] units = new string[] { "B", "KB", "MB", "GB", "TB" };
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

    /// <summary>
    /// 获取指定目录下的第N级别的父目录
    /// </summary>
    /// <param name="currentPath"></param>
    /// <param name="parentDeeep">父目录的层次</param>
    /// <returns></returns>
    public static string GetFilePathParentDirectory(this string currentPath, int parentDeeep)
    {
        if (parentDeeep == 0)
            return currentPath;

        int currentDeep = 0;
        string parentPath = string.Empty;
        string targetPath = currentPath;
        while (currentDeep < parentDeeep)
        {
            parentPath = Path.GetDirectoryName(targetPath);
            targetPath = parentPath;
            ++currentDeep;
        }
        return parentPath;
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

        string fileName = Path.GetFileNameWithoutExtension(currentPath);
        string filePath = Path.GetDirectoryName(currentPath);
        while (string.IsNullOrEmpty(fileName) == false)
        {
            if (fileName == searchDirectoryName)
                return true;

            filePath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(filePath))
                return false;
            fileName = Path.GetFileNameWithoutExtension(filePath);
        }
        return false;
    }

    /// <summary>
    /// 获取指定目录下的文件 不包含指定类型的扩展名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="exculdeExtension">当这个参数为null 或者长度为0 时候等同于Directory.GetFiles(path, searchPattern, searchOption)</param>
    /// <returns></returns>
    public static string[] GetFilesExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension)
    {
        if (System.IO.Directory.Exists(path) == false)
        {
            Debug.LogErrorFormat("GetFilesExculde Fail,Not Exit Directory :{0}", path);
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


    /// <summary>
    ///  获取指定目录下所有的文件和文件夹目录
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="searchOption"></param>
    /// <param name="exculdeExtension">需要过滤掉那些文件扩展名</param>
    /// <param name="isRelativePath">返回结果中是否是相对于path 的相对路径</param>
    /// <param name="directoryIndex">目录在返回结果中的结束索引(等于-1标示不存在，否则为目录总个数)</param>
    /// <returns></returns>
    public static string[] GetFiles_DirectoryExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension, out int directoryIndex, bool isRelativePath = false)
    {
        directoryIndex = -1;
        if (System.IO.Directory.Exists(path) == false)
        {
            Debug.LogErrorFormat("GetFilesExculde Fail,Not Exit Directory :{0}", path);
            return null;
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



    public static string[] GetFiles_DirectoryExculde(this string path, string searchPattern, SearchOption searchOption, string[] exculdeExtension, bool isRelativePath = false)
    {
        int directoryIndex = -1;
        return GetFiles_DirectoryExculde(path, searchPattern, searchOption, exculdeExtension, out directoryIndex, isRelativePath);
    }



}
