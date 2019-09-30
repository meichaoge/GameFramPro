using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatCallback : MonoBehaviour
{
[SerializeField]
    private Text mMessageContent;



    private void FacebookLoginSuccess(string token)
    {
        Debug.Log("登录成功"+token);
        mMessageContent.text = "登录成功" + token;
    }
    
    private void FacebookLoginCancel(string cancel)
    {
        Debug.Log("登录失败"+cancel);
        mMessageContent.text = "登录失败" + cancel;
    }
    private void FacebookLoginError(string error)
    {
        Debug.Log("登录错误"+error);
        mMessageContent.text = "登录错误" + error;
    }
    
    
    
}
