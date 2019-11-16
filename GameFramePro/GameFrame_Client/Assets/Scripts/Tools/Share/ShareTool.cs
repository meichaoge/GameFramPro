using GameFramePro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 分享的内容类型
    /// </summary>
    public enum ShareContentType
    {
        TextOnly,
        ImageOnly,
        TextAndUrl,  //文本和连接
        ImageAndText, //图文
    }

    /// <summary>
    /// 分享的内容
    /// </summary>
    public class SharePlatformContent
    {
        public ShareContentType mShareContentType = ShareContentType.TextAndUrl;
        public string mTextContent;
        public string mUrl;
        public string mImageAbsUri; //图片绝对路径
    }

    /// <summary>
    /// 分享的平台
    /// </summary>
    public enum SharePlatform
    {
        Facebook,
        Twitter,
        WhatsApp,
        Copy,
    }

    /// <summary>
    /// 分享功能公共入口
    /// </summary>
    public static class ShareTool
    {
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern int _WhatsAppShare(string text, string url);

    [DllImport("__Internal")]
    private static extern void _FBShareLink(string text, string url);
   [DllImport("__Internal")]
    private static extern void _FBShareImg(string path);

    [DllImport("__Internal")]
    private static extern void _TwitterShare(string text, string img, string url);
#endif

        /// <summary>
        /// 默认的分享平台
        /// </summary>
        public static readonly List<SharePlatform> S_DefaultSharePlatforms = new List<SharePlatform>() { SharePlatform.Facebook, SharePlatform.Twitter, SharePlatform.WhatsApp, SharePlatform.Copy };



        /// <summary>
        /// 将内容分享到不同平台上
        /// </summary>
        /// <param name="sharePlatform"></param>
        /// <param name="mSharePlatformContent"></param>
        public static void ShareContentToPlatform(SharePlatform sharePlatform, SharePlatformContent mSharePlatformContent)
        {
            switch (sharePlatform)
            {
                case SharePlatform.Facebook:
                    FacebookAction(mSharePlatformContent);
                    break;
                case SharePlatform.Twitter:
                    TwitterAction(mSharePlatformContent);
                    break;
                case SharePlatform.WhatsApp:
                    WhatsAppAction(mSharePlatformContent);
                    break;
                case SharePlatform.Copy:
                    CopyAction(mSharePlatformContent);
                    break;
                default:
                    Debug.LogError($"没有处理的分享平台{sharePlatform}");
                    break;
            }
        }

        #region 分享到不同渠道的实现

        private static void WhatsAppAction(SharePlatformContent mSharePlatformContent)
        {
            switch (mSharePlatformContent.mShareContentType)
            {
                case ShareContentType.TextOnly:
                    break;
                case ShareContentType.ImageOnly:
                    Debug.LogError($" WhatsApp 不支持ios");
#if UNITY_IPHONE && !UNITY_EDITOR
        //if(_WhatsAppShare(m_Text, m_Url) > 0) {
        //    AlertView.Show(Lang.StaticString("not_install_whatsapp_tips"), Lang.StaticString("confirm"));
                     Debug.LogError($"不支持ios");
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
       // jo.Call("wathsAppShare", m_Url, m_Text);
                       AppPlatformManager.Current.Call("wathsAppShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                case ShareContentType.TextAndUrl:
#if UNITY_IPHONE && !UNITY_EDITOR
        if(_WhatsAppShare(m_Text, m_Url) > 0) {
            AlertView.Show(Lang.StaticString("not_install_whatsapp_tips"), Lang.StaticString("confirm"));
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
       // jo.Call("wathsAppShare", m_Url, m_Text);
                       AppPlatformManager.Current.Call("wathsAppShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                case ShareContentType.ImageAndText:
                    break;
                default:
                    Debug.LogError($"没有处理的类型" + mSharePlatformContent.mShareContentType);
                    break;
            }




        }

        private static void FacebookAction(SharePlatformContent mSharePlatformContent)
        {

            switch (mSharePlatformContent.mShareContentType)
            {
                case ShareContentType.TextOnly:
                    break;
                case ShareContentType.ImageOnly:
#if UNITY_IPHONE && !UNITY_EDITOR
        _FBShareImg(mSharePlatformContent.mImageAbsUri);
#elif UNITY_ANDROID && !UNITY_EDITOR
                  //    jo.Call("initFacebookShare");
     AppPlatformManager.Current.Call("initFacebookShare");
                    AppPlatformManager.Current.Call("facebookShare", mSharePlatformContent.mImageAbsUri);
#endif

                    break;
                case ShareContentType.TextAndUrl:
#if UNITY_IPHONE && !UNITY_EDITOR
        _FBShareLink(m_Text, m_Url);
#elif UNITY_ANDROID && !UNITY_EDITOR
                  //    jo.Call("initFacebookShare");
       // jo.Call("facebookShare", m_Url, m_Text);
     AppPlatformManager.Current.Call("initFacebookShare");
                    AppPlatformManager.Current.Call("facebookShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif


                    break;
                case ShareContentType.ImageAndText:
                    break;
                default:
                    Debug.LogError($"没有处理的类型" + mSharePlatformContent.mShareContentType);
                    break;
            }
        }

        //Twitter
        private static void TwitterAction(SharePlatformContent mSharePlatformContent)
        {
            switch (mSharePlatformContent.mShareContentType)
            {
                case ShareContentType.TextOnly:
#if UNITY_IPHONE && !UNITY_EDITOR
           _TwitterShare(mSharePlatformContent.mTextContent,null, null);
#elif UNITY_ANDROID && !UNITY_EDITOR
    //    jo.Call("twitterShare", m_Url, m_Text);
                    AppPlatformManager.Current.Call("twitterShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                case ShareContentType.ImageOnly:
#if UNITY_IPHONE && !UNITY_EDITOR
        _TwitterShare(null, mSharePlatformContent.mImageAbsUri, null);
#elif UNITY_ANDROID && !UNITY_EDITOR
    //    jo.Call("twitterShare", m_Url, m_Text);
                    AppPlatformManager.Current.Call("twitterShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                case ShareContentType.TextAndUrl:

#if UNITY_IPHONE && !UNITY_EDITOR
        _TwitterShare(mSharePlatformContent.mTextContent, null, mSharePlatformContent.mUrl);
#elif UNITY_ANDROID && !UNITY_EDITOR
    //    jo.Call("twitterShare", m_Url, m_Text);
                    AppPlatformManager.Current.Call("twitterShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                case ShareContentType.ImageAndText:
#if UNITY_IPHONE && !UNITY_EDITOR
           _TwitterShare(mSharePlatformContent.mTextContent, mSharePlatformContent.mImageAbsUri, null);
#elif UNITY_ANDROID && !UNITY_EDITOR
    //    jo.Call("twitterShare", m_Url, m_Text);
                    AppPlatformManager.Current.Call("twitterShare", mSharePlatformContent.mUrl, mSharePlatformContent.mTextContent);
#endif
                    break;
                default:
                    Debug.LogError($"没有处理的类型" + mSharePlatformContent.mShareContentType);
                    break;
            }
        }

        //复制
        private static void CopyAction(SharePlatformContent mSharePlatformContent)
        {
            switch (mSharePlatformContent.mShareContentType)
            {
                case ShareContentType.TextOnly:
                    //LoginState.instance().CopyTableNo = mSharePlatformContent.mTextContent;
                    break;
                case ShareContentType.TextAndUrl:
                    //if (string.IsNullOrEmpty(mSharePlatformContent.mTextContent))
                    //    LoginState.instance().CopyTableNo = mSharePlatformContent.mUrl;
                    //else
                    //    LoginState.instance().CopyTableNo = string.Format("{0}\n{1}}", mSharePlatformContent.mTextContent, mSharePlatformContent.mUrl);
                    break;
                case ShareContentType.ImageOnly:
                case ShareContentType.ImageAndText:
                    Debug.LogError($"复制不支持的类型");
                    break;
                default:
                    Debug.LogError($"没有处理的类型" + mSharePlatformContent.mShareContentType);
                    break;
            }

        //    Clipboard.CopyToClipboard(LoginState.instance().CopyTableNo);
        }
        #endregion


        #region 其他
        /// <summary>
        /// 截取指定相机的 RenderTexture 并保存到本地
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="completeCallback"></param>
        /// <returns></returns>
        public static IEnumerator GetCameraTextureForShare(Camera camera, int width, int height, System.Action<bool, string> completeCallback)
        {
            RenderTexture renderTexture = camera.targetTexture;
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                camera.targetTexture = renderTexture;
            }
            yield return AsyncManager.WaitFor_EndOfFrame;
            try
            {
                RenderTexture.active = renderTexture;

                Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                tex.Apply();

                //将图片信息编码为字节信息  
                byte[] bytes = tex.EncodeToJPG();

                string timeStr =DateTime.Now.Ticks+ ".png";

                if (System.IO.Directory.Exists(ConstDefine.S_ShareImageTopDirectory) == false)
                    System.IO.Directory.CreateDirectory(ConstDefine.S_ShareImageTopDirectory);

                string mPath = ConstDefine.S_ShareImageTopDirectory.CombinePathEx(timeStr);

                //保存  
                using (System.IO.FileStream fs = new FileStream(mPath, FileMode.Create, FileAccess.Write, FileShare.None, bytes.Length, FileOptions.WriteThrough))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }

                 //   System.IO.File.WriteAllBytes(mPath, bytes);

                Debug.Log($"截取相机图片保存路径{mPath}");

                completeCallback?.Invoke(true, mPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"截取相机图片保存失败{e}");
                completeCallback?.Invoke(false, string.Empty);
            }

        }
        #endregion

    }
}