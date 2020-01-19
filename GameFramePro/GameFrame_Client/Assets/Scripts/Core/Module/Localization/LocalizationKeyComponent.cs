using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GameFramePro.Localization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    [System.Serializable]
    public class LocalizationKeyComponent : MonoBehaviour
    {
        [SerializeField]
        private Text mTargetText;

        [SerializeField]
        private string mConnectLocalizationKey = string.Empty;

        private void Awake()
        {
            if (mTargetText == null)
            {
                BindTargetText();
#if UNITY_EDITOR
                string key = mTargetText.text;
                if (LocalizationManager.CheckIfMatchLocalizationKey(key))
                {
                    if (LocalizationManager.IsExitLocalizationKey(key))
                        mConnectLocalizationKey = key;
                    else
                        Debug.LogError($"检测到没有记录的key{key}  关联对象{gameObject.name}  请检查是否有大小写问题");
                }
                else
                {
                    Debug.LogError($"检测到不合法的key{key}  关联对象{gameObject.name}");
                }
#endif
            }
        }

        public void BindTargetText()
        {
            if (mTargetText == null)
                mTargetText = GetComponent<Text>();
        }

        public void UpdateConnectLocalizationKey(string localizationKey, bool isIgnoreCase = false, bool isUpdateText = false)
        {
#if UNITY_EDITOR
            if (LocalizationManager.IsExitLocalizationKey(localizationKey, isIgnoreCase) == false)
            {
                Debug.LogError($"检测到没有记录的key{localizationKey}  关联对象{gameObject.name}");
                return;
            }
#endif
            mConnectLocalizationKey = localizationKey;
            if (isUpdateText)
                mTargetText.text = LocalizationManager.GetLocalizationByKey(localizationKey);
        }

        public void UpdateConnectLocalizationKey(bool isIgnoreCase, bool isUpdateText = false)
        {
            if (mTargetText == null) return;
            UpdateConnectLocalizationKey(mTargetText.text, isIgnoreCase, isUpdateText);
        }

        //检测指定的Key 是否存在
        public void CheckIsExitTargetLocalizationKey(bool isIgnoreCase = false)
        {
            if (mTargetText == null) return;
            Debug.Log($"检测是否存在key{  mTargetText.text }  =>{LocalizationManager.IsExitLocalizationKey(mTargetText.text, isIgnoreCase)}");
        }

        /// <summary>
        /// 翻译转换
        /// </summary>
        public void TranslateLocalization()
        {
            if (mTargetText == null) return;
            mTargetText.text = LocalizationManager.GetLocalizationByKey(mConnectLocalizationKey);
        }

        //#if UNITY_EDITOR
        //     void OnReset()
        //        {
        //            mTargetText = GetComponent<Text>();
        //        }
        //#endif

    }
}