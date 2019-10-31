using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TestServerConnectState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //1.发出请求
        string url = "http://superxu3d.tcmapi.cn/goalon/TestApplication/test123.exe";
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        request.AddRange(0, 1);
        request.Timeout = 10000; //防止不支持时等太久
        HttpWebResponse response;
        try
        {
            response = (HttpWebResponse)request.GetResponse();
        }
        catch (WebException ex)
        {
            response = (HttpWebResponse)ex.Response;
        }
        //2.根据HTTP状态码判断是否支持断点续传
      Debug.Log(response != null && response.StatusCode == HttpStatusCode.PartialContent ? "YES" : "NO");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
