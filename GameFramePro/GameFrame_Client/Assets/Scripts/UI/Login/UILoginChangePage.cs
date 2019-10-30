using System;
using System.Collections.Generic;
using System.Collections;
using GameFramePro.Protocol.LoginModule;
using GameFramePro.Upgrade;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using GameFramePro.Localization;

namespace GameFramePro.UI
{
    /// <summary>/// 登录主界面/// </summary>
    public class UILoginChangePage : UIBaseChangePage
    {
        #region UI

        private Transform m_BasicInforContent;
        private Text m_AppVersionText;
        private Transform m_LoginContent;
        private Text m_AssetVersionText;
        private Text m_NameText;
        private Text m_PasswordText;
        private InputField m_NameInputField;
        private InputField m_PasswordInputField;
        private Button m_LoginButton;
        private Dropdown m_SelectLanguageDropdown;

        #endregion

        #region Data

        private List<Language> mAllSupportLanguages; //支持的语言

        #endregion


        #region 基类重写

        protected override void OnInitialed()
        {
            base.OnInitialed();
            GetUIComponentReference();
            //TODO
        }

        protected override void OnBeforeVisible()
        {
            base.OnBeforeVisible();
            ShowAppVisonView();
            ShowNameOrPasswordView();

            ShowAllSupportLanguage();
            RegistEventMessage();
        }

        protected override void OnBeforeInVisible()
        {
            UnRegistEventMessage();
            base.OnBeforeInVisible();
        }

        protected override void OnBeforeDestroyed()
        {
            m_NameInputField.onValueChanged.RemoveAllListeners();
            m_PasswordInputField.onValueChanged.RemoveAllListeners();
            m_SelectLanguageDropdown.onValueChanged.RemoveAllListeners();
            m_LoginButton.onClick.RemoveAllListeners();
            base.OnBeforeDestroyed();
        }

        /// <summary>/// 根据挂载的 UGUIComponentReference 脚本生成对应的引用配置/// </summary>
        private void GetUIComponentReference()
        {
            m_BasicInforContent = GetComponentByName<Transform>("BasicInforContent");
            m_AppVersionText = GetComponentByName<Text>("AppVersionText");
            m_LoginContent = GetComponentByName<Transform>("LoginContent");
            m_AssetVersionText = GetComponentByName<Text>("AssetVersionText");
            m_NameText = GetComponentByName<Text>("NameText");
            m_PasswordText = GetComponentByName<Text>("PasswordText");
            m_NameInputField = GetComponentByName<InputField>("NameInputField");
            m_PasswordInputField = GetComponentByName<InputField>("PasswordInputField");
            m_LoginButton = GetComponentByName<Button>("LoginButton");
            m_SelectLanguageDropdown = GetComponentByName<Dropdown>("SelectLanguageDropdown");

            //**
            m_NameInputField.onValueChanged.AddListener(OnUserNameInputCallback);
            m_PasswordInputField.onValueChanged.AddListener(OnUserPasswordInputCallback);
            m_SelectLanguageDropdown.onValueChanged.AddListener(OnSelectLanguageClick);
            m_LoginButton.onClick.AddListener(OnLoginButtonClick);
            
            GetComponentByName<Button>("FacebookButton").onClick.AddListener(OnFacebookButtonClick);
            GetComponentByName<Button>("WetChatButton").onClick.AddListener(OnWetChatButtonClick);

            
        }


        private void RegistEventMessage()
        {
            EventTrigger.RegisterMessageHandler<LoginResponse>((int) UIEventUsage.OnResponse_Login, OnLoginCallback);
            EventTrigger.RegisterMessageHandler((int) UIEventUsage.OnResponse_Login, OnLoginCallback2);
        }

        private void UnRegistEventMessage()
        {
            EventTrigger.UnRegisterMessageHandler<LoginResponse>((int) UIEventUsage.OnResponse_Login, OnLoginCallback);
            EventTrigger.UnRegisterMessageHandler((int) UIEventUsage.OnResponse_Login, OnLoginCallback2);
        }

        #endregion

        #region  创建视图

        /// <summary>/// 显示App 版本信息/// </summary>
        private void ShowAppVisonView()
        {
            m_AppVersionText.text = ApplicationManager.S_Instance.mAppVersion;
            m_AssetVersionText.text = ApplicationManager.S_Instance.mAppResourcesVersion;
        }

        /// <summary>/// 用户名或者密码输入回调/// </summary>
        private void ShowNameOrPasswordView()
        {
            m_NameInputField.text = String.Empty;
            m_PasswordInputField.text = String.Empty;
        }

        /// <summary>/// 显示支持的语言/// </summary>
        private void ShowAllSupportLanguage()
        {
            mAllSupportLanguages = EnumUtility.GetEnumValue<Language>();

            m_SelectLanguageDropdown.options.Clear();
            foreach (var language in mAllSupportLanguages)
            {
                m_SelectLanguageDropdown.options.Add(new Dropdown.OptionData(language.ToString()));
            }
        }

        #endregion

        #region  按钮点击事件

        private void OnFacebookButtonClick()
        {
            Debug.Log("android facebook 登录");
            PlatformManager.Current.Call("LoginFacebook");
        }
        
        private void OnWetChatButtonClick()
        {
            Debug.Log("android 微信 登录");
            PlatformManager.Current.Call("LoginWetchat");
        }
        
        
        private void OnUserNameInputCallback(string inputName)
        {
        }

        private void OnUserPasswordInputCallback(string inputPassword)
        {
        }

        private void OnSelectLanguageClick(int selectIndex)
        {
            Debug.LogEditorInfor($"切换语言到{mAllSupportLanguages[selectIndex]}");
        }


        private void OnLoginButtonClick()
        {
            if (string.IsNullOrEmpty(m_NameInputField.text))
            {
                Debug.LogInfor("输入的用户名为null ");
                UIPageHelpUtility.ShowTipMessage("输入的用户名为null");
                return;
            }

            if (string.IsNullOrEmpty(m_PasswordInputField.text))
            {
                Debug.LogInfor("输入的用户密码为null ");
                UIPageHelpUtility.ShowTipMessage("输入的用户密码为null");

                return;
            }

            Debug.LogInfor("登录网络请求--");
            LoginNetworkModule.S_Instance.RequestLogin(m_NameInputField.text, m_PasswordInputField.text);
        }

        #endregion


        #region 网络事件回调

        private void OnLoginCallback(int messageID, LoginResponse response)
        {
            Debug.Log("登录网络回调成功" + Thread.CurrentThread.IsThreadPoolThread + "  " + Thread.CurrentThread.IsBackground);
            if (response.mIsSuccess)
            {
                UIPageManager.OpenChangePage<UIHomeChangePage>(NameDefine.UIHomeChangePageName, PathDefine.UIHomeChangePagePath);
            }
        }

        private void OnLoginCallback2(int messageID)
        {
            Debug.Log($" 测试向下转发的消息");
        }

        #endregion
    }
}
