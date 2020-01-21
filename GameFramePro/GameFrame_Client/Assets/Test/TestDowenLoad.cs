using GameFramePro;
using GameFramePro.NetWorkEx;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TestDowenLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
          //  string urlInfor = "http://superxu3d.tcmapi.cn/goalon/TestApplication/test123.exe";
            string urlInfor = "https://supergoal-content-bucket.oss-cn-hongkong.aliyuncs.com/goalon/TestApplication/test123.exe";

       
            string localPath = $"{Application.dataPath.GetFilePathParentDirectory(1)}/{"Test"}/{"test123.exe"}";

            IOUtility.CheckOrCreateDirectory(System.IO.Path.GetDirectoryName(localPath));

            Debug.Log($"localPath={localPath}");

            FileInfo infor = IOUtility.GetFileInfo(localPath);

            long begin = 0;
            if (infor != null)
                begin = infor.Length;



            DownloadManager.GetContentLength(urlInfor, (isSuccess, length) =>
            {
                if (isSuccess == false)
                    return;
                Debug.Log($"开始下载 {begin}   {length}");
                DownloadManager.GetByteDataFromUrl(urlInfor, begin, length,  (webRequest, isSucces, url) =>
                {
                    Debug.Log("url=" + url);
                    DownloadHandlerBuffer handle = webRequest.downloadHandler as DownloadHandlerBuffer;
                    if (webRequest.isDone == false || webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        Debug.LogError($"Error=" + webRequest.error);
                        return;
                    }

                    if(handle.data.Length== length)
                    {
                        Debug.Log($"当前服务器返回了所有的数据 而不是指定部分数据");
                         IOUtility.CreateOrSetFileContent(localPath, handle.data, false); //追加数据
                    }
                    else
                    {
                        IOUtility.AppFileContent(localPath, handle.data, begin); //追加数据
                    }

                },TaskPriorityEnum.Immediately,60*5);

            });



        }
    }
}
