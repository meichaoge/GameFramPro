using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using GameFramePro.NetWorkEx;
using UnityEngine;
using UnityEngine.UI;

public class UdpServerUIComponent : MonoBehaviour
{
    public InputField mListennerPort;
    public Button mStartServerButton;
    public Button mAutoSetPortButton;

    public Text mReceiveMessageText;

    public Text mSendMessageText;
    public InputField mSendIpAddress;
    public InputField mSendEndPort;
    public InputField mSendMessage;
    public Button mSendMessageButton;

    public Toggle mBrocastToggle;
    public Button mJoinGoupButton;

    private bool mIsJoinGroup = true;
    private bool mIsStartClient = false;

    private UdpServer mUdpServer;

    // Start is called before the first frame update
    void Start()
    {
        mSendIpAddress.text = "127.0.0.1";

        mStartServerButton.onClick.AddListener(OnStartServerButtonClick);
        mAutoSetPortButton.onClick.AddListener(OnAutoSetPortClick);
        mJoinGoupButton.onClick.AddListener(OnJoinGroupClick);


        mSendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
    }

    private void OnDisable()
    {
        if (mIsStartClient == false) return;
        mUdpServer.StopServer();
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
        mUdpServer = new UdpServer(int.Parse(mListennerPort.text));
        mUdpServer.OnReceiveMessageEvent += ReceiveMessage;
        mUdpServer.OnSendMessageEvent += SendMessage;


        mUdpServer.StartServer();
        Debug.Log("服务器端启动了...");
    }

    private void OnAutoSetPortClick()
    {
        int port = SocketUtility.GetNextAvailablePort();
        mListennerPort.text = port.ToString();
        Debug.Log($"自动选择端口号 {port}");
    }

    private void OnJoinGroupClick()
    {
        if (mIsJoinGroup)
            mUdpServer.JoinGroup(IPAddress.Parse("224.100.0.1"));
        else
            mUdpServer.LeaveGroup(IPAddress.Parse("224.100.0.1"));

        mIsJoinGroup = !mIsJoinGroup;
    }


    private void OnSendMessageButtonClick()
    {
        if (mIsStartClient == false)
        {
            Debug.LogError("请先启动客户端");
            return;
        }

        if (string.IsNullOrEmpty(mSendEndPort.text))
        {
            Debug.LogError("请先输入正确的服务器端口号");
            return;
        }

        if (mBrocastToggle.isOn == false)
            mUdpServer.SendMessage(mSendMessage.text, mSendIpAddress.text, int.Parse(mSendEndPort.text));
        else
        {
            mUdpServer.SendBrocast(mSendMessage.text, int.Parse(mSendEndPort.text));

            // mSimpleUdpClient.SendMessage(mSendMessage.text, new IPEndPoint(IPAddress.Broadcast, int.Parse(mSendEndPort.text)));
        }
    }


    private void ReceiveMessage(string message, EndPoint endPoint)
    {
        return;
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