using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using GameFramePro.NetWorkEx;
using UnityEngine;
using UnityEngine.UI;

public class TcpServerUIComponent : MonoBehaviour
{
    public Text mLocalAddress;
    public InputField mListennerPort;
    public Button mStartServerButton;
    public Button mAutoSetPortButton;

    public Text mReceiveMessageText;
    public Text mSendMessageText;

    private bool mIsStartClient = false;

    private TcpServer mTcpServer;

    void Start()
    {
        mLocalAddress.text = SocketUtility.GetLocalIpAddress(true).ToString();
        mStartServerButton.onClick.AddListener(OnStartServerButtonClick);
        mAutoSetPortButton.onClick.AddListener(OnAutoSetPortClick);
    }

    private void OnDisable()
    {
        if (mIsStartClient == false) return;

        mTcpServer.StopServer();
        Debug.Log("关闭服务器");
    }


    private void OnStartServerButtonClick()
    {
        if (string.IsNullOrEmpty(mListennerPort.text))
        {
            Debug.LogError("请输入服务端口号");
            return;
        }

        if (mIsStartClient)
        {
            Debug.LogError("服务器端已经启动了");
            return;
        }

        mIsStartClient = true;
        mTcpServer = new TcpServer(int.Parse(mListennerPort.text));
        mTcpServer.OnReceiveMessageEvent += ReceiveMessage;
        mTcpServer.OnSendMessageEvent += SendMessage;


        mTcpServer.StartServer();
        Debug.Log("服务器端启动了...");
    }

    private void OnAutoSetPortClick()
    {
        int port = SocketUtility.GetNextAvailablePort();
        mListennerPort.text = port.ToString();
        Debug.Log($"自动选择端口号 {port}");
    }


    private void ReceiveMessage(string message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mReceiveMessageText.text);
        if (string.IsNullOrEmpty(mReceiveMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : ReceiveFrom {endPoint} {message}");
        mReceiveMessageText.text = builder.ToString();
    }

    private void SendMessage(string message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mSendMessageText.text);
        if (string.IsNullOrEmpty(mSendMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : SendTo {endPoint} {message}");
        mSendMessageText.text = builder.ToString();
    }
}