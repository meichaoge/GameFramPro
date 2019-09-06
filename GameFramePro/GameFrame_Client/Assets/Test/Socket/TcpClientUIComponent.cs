using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameFramePro;
using GameFramePro.NetWorkEx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>/// Tcp 客户端/// </summary>
public class TcpClientUIComponent : MonoBehaviour
{
    public Text mLocalHost;
    public InputField mListennerPort;
    public Button mStartClientButton;
    public Button mAutoSetPortButton;

    public Text mReceiveMessageText;

    public Text mSendMessageText;
    public InputField mSendIpAddress;
    public InputField mSendEndPort;
    public InputField mSendMessage;
    public Button mSendMessageButton;
    public Button mConectButton;

    private bool mIsStartClient = false;
    private BaseTcpClient mBaseTcpClient;

    // Start is called before the first frame update
    void Start()
    {
        mLocalHost.text = SocketUtility.GetLocalIpAddress().ToString();
        mSendIpAddress.text = "127.0.0.1"; //SocketUtility.GetLocalIpAddress().ToString();

        mStartClientButton.onClick.AddListener(OnStartClientButtonClick);
        mAutoSetPortButton.onClick.AddListener(OnAutoSetPortClick);
        mConectButton.onClick.AddListener(OnmConectButton);


        mSendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
    }

    public static byte[] intToBytes(int value)
    {
        byte[] src = new byte[4];
        src[3] = (byte) ((value >> 24) & 0xFF);
        src[2] = (byte) ((value >> 16) & 0xFF);
        src[1] = (byte) ((value >> 8) & 0xFF);
        src[0] = (byte) (value & 0xFF);

        return src;
    }

    private void OnDisable()
    {
        if (mIsStartClient == false) return;
        mBaseTcpClient?.StopClient();
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
        mBaseTcpClient = new BaseTcpClient("测试Tcp");
//        mSimpleTcpClient.OnReceiveMessageEvent += ReceiveMessage;
//        mSimpleTcpClient.OnSendMessageEvent += SendMessage;

        Debug.Log($"客户端启动 ");
    }

    private void OnmConectButton()
    {
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(mSendIpAddress.text), int.Parse(mSendEndPort.text));
        mBaseTcpClient.Connect(endpoint, 2000);
        Debug.Log($"客户端尝试连接到{endpoint}");
    }

    private void OnAutoSetPortClick()
    {
        int port = SocketUtility.GetNextAvailablePort();
        mListennerPort.text = port.ToString();
        Debug.Log($"自动选择端口号 {port}");
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

        var sendByteArray = ByteArray.GetByteArray();
        sendByteArray.EncodingGetBytes(mSendMessage.text, Encoding.UTF8);

        mBaseTcpClient.Send(0, sendByteArray);
        ByteArray.RecycleByteArray(sendByteArray);
    }


    public void ReceiveMessage(byte[] message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mReceiveMessageText.text);
        if (string.IsNullOrEmpty(mReceiveMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : ReceiveFrom {endPoint} {Encoding.UTF8.GetString(message)}");
        mReceiveMessageText.text = builder.ToString();
    }

    public void SendMessage(byte[] message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mSendMessageText.text);
        if (string.IsNullOrEmpty(mSendMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : SendTo {endPoint} {Encoding.UTF8.GetString(message)}");
        mSendMessageText.text = builder.ToString();
    }
}