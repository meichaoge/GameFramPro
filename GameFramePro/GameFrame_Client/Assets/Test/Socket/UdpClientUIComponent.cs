using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using GameFramePro;
using GameFramePro.NetWorkEx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UdpClientUIComponent : MonoBehaviour
{
    public InputField mListennerPort;
    public Button mStartClientButton;
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
    private SimpleUdpClient mSimpleUdpClient;

    // Start is called before the first frame update
    void Start()
    {
        mSendIpAddress.text = "127.0.0.1"; //SocketUtility.GetLocalIpAddress().ToString();

        mStartClientButton.onClick.AddListener(OnStartClientButtonClick);
        mAutoSetPortButton.onClick.AddListener(OnAutoSetPortClick);
        mJoinGoupButton.onClick.AddListener(OnJoinGroupClick);


        mSendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
    }

    private void OnDisable()
    {
        if (mIsStartClient == false) return;
        mSimpleUdpClient.StopClient();
        Debug.Log("关闭客户端");
    }


    private void OnStartClientButtonClick()
    {
        if (string.IsNullOrEmpty(mListennerPort.text))
        {
            Debug.LogError("请输入客户端端口号");
            return;
        }

        if (mIsStartClient)
        {
            Debug.LogError("客户端已经启动了");
            return;
        }

        mIsStartClient = true;
        mSimpleUdpClient = new SimpleUdpClient(int.Parse(mListennerPort.text));
        mSimpleUdpClient.OnReceiveMessageEvent += ReceiveMessage;
        mSimpleUdpClient.OnSendMessageEvent += SendMessage;


        mSimpleUdpClient.StartClient();
        Debug.Log("客户端启动");
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
            mSimpleUdpClient.JoinGroup(IPAddress.Parse("224.100.0.1"));
        else
            mSimpleUdpClient.LeaveGroup(IPAddress.Parse("224.100.0.1"));

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
            mSimpleUdpClient.SendMessage(mSendMessage.text, mSendIpAddress.text, int.Parse(mSendEndPort.text));
        else
        {
            mSimpleUdpClient.SendBrocast(mSendMessage.text, int.Parse(mSendEndPort.text));

            // mSimpleUdpClient.SendMessage(mSendMessage.text, new IPEndPoint(IPAddress.Broadcast, int.Parse(mSendEndPort.text)));
        }
    }


    public void ReceiveMessage(string message, EndPoint endPoint)
    {
        return;
        
        StringBuilder builder = new StringBuilder(mReceiveMessageText.text);
        if (string.IsNullOrEmpty(mReceiveMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : ReceiveFrom {endPoint} {message}");
        mReceiveMessageText.text = builder.ToString();
    }

    public void SendMessage(string message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mSendMessageText.text);
        if (string.IsNullOrEmpty(mSendMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : SendTo {endPoint} {message}");
        mSendMessageText.text = builder.ToString();
    }
}