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

    public Toggle mBrocastToggle;

    public InputField mGroupIpInput;
    public Toggle mGroupIpListenner;


    private bool mIsStartClient = false;
    private SimpleUdpClient mSimpleUdpClient;

    // Start is called before the first frame update
    void Start()
    {
        mLocalHost.text = SocketUtility.GetLocalIpAddress().ToString();
        mBrocastToggle.isOn = false;
        mSendIpAddress.text = "127.0.0.1"; //SocketUtility.GetLocalIpAddress().ToString();
        mGroupIpInput.text = "234.5.6.7";

        mStartClientButton.onClick.AddListener(OnStartClientButtonClick);
        mAutoSetPortButton.onClick.AddListener(OnAutoSetPortClick);
        mGroupIpListenner.isOn = false;
        mGroupIpListenner.onValueChanged.AddListener(OnJoinGroupClick);

        mBrocastToggle.onValueChanged.AddListener(OnBrocastStateChange);
        mSendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
    }

    private void OnDisable()
    {
        if (mIsStartClient == false) return;
        mSimpleUdpClient.StopClient();
        Debug.Log("关闭客户端");
    }


    private void OnBrocastStateChange(bool isOn)
    {
        if (mIsStartClient == false) return;
        mSimpleUdpClient.SetBrocastEnableState(isOn);
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
        mBrocastToggle.isOn = mSimpleUdpClient.mIsEnableBrocast;
        Debug.Log("客户端启动");
    }

    private void OnAutoSetPortClick()
    {
        int port = SocketUtility.GetNextAvailablePort();
        mListennerPort.text = port.ToString();
        Debug.Log($"自动选择端口号 {port}");
    }

    private void OnJoinGroupClick(bool isOn)
    {
        if (isOn)
        {
            mSimpleUdpClient.JoinGroup(IPAddress.Parse(mGroupIpInput.text));

            ReceiveMessage(Encoding.UTF8.GetBytes($"加入组播{IPAddress.Parse(mGroupIpInput.text)}"), new IPEndPoint(IPAddress.Parse(mGroupIpInput.text), 0));
        }
        else
        {
            mSimpleUdpClient.LeaveGroup(IPAddress.Parse(mGroupIpInput.text));
            ReceiveMessage(Encoding.UTF8.GetBytes($"离开组播{IPAddress.Parse(mGroupIpInput.text)}"), new IPEndPoint(IPAddress.Parse(mGroupIpInput.text), 0));
        }
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


        ByteArray sendByteArray = ByteArrayPool.S_Instance.GetByteArray();
        sendByteArray.EncodingGetBytes(mSendMessage.text, Encoding.UTF8);

        if (mGroupIpListenner.isOn)
        {
            mSimpleUdpClient.SendMessage(0, sendByteArray, mGroupIpInput.text, int.Parse(mSendEndPort.text));
        }
        else
        {
            if (mBrocastToggle.isOn == false)
                mSimpleUdpClient.SendMessage(0, sendByteArray, mSendIpAddress.text, int.Parse(mSendEndPort.text));
            else
            {
                mSimpleUdpClient.SendBrocast(0, sendByteArray, int.Parse(mSendEndPort.text));
                // mSimpleUdpClient.SendMessage(mSendMessage.text, new IPEndPoint(IPAddress.Broadcast, int.Parse(mSendEndPort.text)));
            }
        }
    }


    public void ReceiveMessage(byte[] message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mReceiveMessageText.text);
        if (string.IsNullOrEmpty(mReceiveMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : ReceiveFrom {endPoint} {message}");
        mReceiveMessageText.text = builder.ToString();
    }

    public void SendMessage(byte[] message, EndPoint endPoint)
    {
        StringBuilder builder = new StringBuilder(mSendMessageText.text);
        if (string.IsNullOrEmpty(mSendMessageText.text) == false)
            builder.Append(System.Environment.NewLine);
        builder.Append($"[{DateTime.Now}] : SendTo {endPoint} {message}");
        mSendMessageText.text = builder.ToString();
    }
}