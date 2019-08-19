using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UdpServerUIComponent : MonoBehaviour
{
    public InputField mListennerPort;
    public Button mListennerButton;
    public Text mReceiveMessageText;

    public Text mSendMessageText;
    public InputField mSendIpAddress;
    public InputField mSendEndPort;
    public InputField mSendMessage;
    public Button mSendMessageButton;

    // Start is called before the first frame update
    void Start()
    {
        mListennerButton.onClick.AddListener(OnListennnerButtonClick);
        mSendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
    }


    private void OnListennnerButtonClick()
    {
    }

    private void OnSendMessageButtonClick()
    {
    }


    public void ReceiveMessage(string message)
    {
    }

    public void SendMessage(string message)
    {
    }
}
