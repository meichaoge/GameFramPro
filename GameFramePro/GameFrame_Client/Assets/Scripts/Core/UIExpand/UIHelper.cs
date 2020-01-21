using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro
{
    //UI 开发时候常用的功能
    public static class UIHelper
    {
        #region Text 文本处理 (截断到指定长度/)

        /// <summary>/// 将指定的Text  中内容控制在 targetTextWidth 长度以内，如果appendString不为空，则追加这个结尾/// </summary>
        public static string ControlTextLength(Text targetText, string textContent, float targetTextWidth, string appendString = "...")
        {
            int appendStringLength = 0;
            if (string.IsNullOrEmpty(appendString) == false)
                appendStringLength = CalculateLengthOfText(targetText, appendString);

            int textContentLength = CalculateLengthOfText(targetText, textContent); //要显示的文本长度
            if (textContentLength <= targetTextWidth)
                return textContent;


            if (appendStringLength == 0)
                return StripLength(targetText, textContent, targetTextWidth);
            return StripLength(targetText, textContent, targetTextWidth - appendStringLength) + appendString;
        }

        /// <summary>/// 获取指定Text  上显示指定内容时候的需要的长度/// </summary>
        public static int CalculateLengthOfText(Text targetText, string message)
        {
            int totalLength = 0;
            Font myFont = targetText.font; //chatText is my Text component
            myFont.RequestCharactersInTexture(message, targetText.fontSize, targetText.fontStyle);
            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = message.ToCharArray();

            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, targetText.fontSize);

                totalLength += characterInfo.advance;
            }

            return totalLength;
        }

        /// <summary>/// 截断指定Text 上显示的指定文本，长度限制在 maxWidth/// </summary>
        private static string StripLength(Text targetText, string textContent, float maxWidth)
        {
            int totalLength = 0;
            Font myFont = targetText.font; //chatText is my Text component
            myFont.RequestCharactersInTexture(textContent, targetText.fontSize, targetText.fontStyle);

            CharacterInfo characterInfo = new CharacterInfo();
            char[] contentChars = textContent.ToCharArray();

            int index = 0;
            foreach (var contentChar in contentChars)
            {
                myFont.GetCharacterInfo(contentChar, out characterInfo, targetText.fontSize);
                totalLength += characterInfo.advance;
                if (totalLength > maxWidth)
                    break;
                ++index;
            }

            return textContent.Substring(0, index);
        }

        #endregion


        #region 节点路径获取

        /// <summary>/// 找到从当前节点到父节点的路径/// </summary>
        public static string GetPathToRoot(this Transform target, Transform root, string split = "/")
        {
            if (target == null)
                return string.Empty;

            List<string> allPaths = new List<string>(10);
            Transform parentTrans = target;
            bool isFind = false;
            while (true)
            {
                allPaths.Add(parentTrans.name);

                if (parentTrans == root)
                {
                    isFind = true;
                    break;
                }

                if (parentTrans == null)
                    break;
                parentTrans = parentTrans.parent;
            }

            if (isFind == false)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            allPaths.Reverse();
            for (int dex = 0; dex < allPaths.Count; dex++)
            {
                builder.Append(allPaths[dex]);
                if (dex < allPaths.Count - 1)
                    builder.Append(split);
            }

            return builder.ToString();
        }

        #endregion
    }
}