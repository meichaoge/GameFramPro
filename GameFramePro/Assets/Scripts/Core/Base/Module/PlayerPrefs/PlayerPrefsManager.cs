using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 封装Unity  PlayerPrefs 。方便统计所有本地化的的数据
    /// </summary>
    public class PlayerPrefsManager : Single<PlayerPrefsManager>
    {
        private const string TotalPlayerPrefsKeyRecord = "__totalPlayerPrefsKeyRecordKeys";
        private static HashSet<string> mAllPlayerPrefsKeys = new HashSet<string>(); //所有本地化key

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            if (PlayerPrefs.HasKey(TotalPlayerPrefsKeyRecord))
            {
                string totalKeys = PlayerPrefs.GetString(TotalPlayerPrefsKeyRecord, string.Empty);
                if (string.IsNullOrEmpty(totalKeys)) return;
                mAllPlayerPrefsKeys = SerilazeManager.DeserializeObject<HashSet<string>>(totalKeys);
            }
        }

        public override void DisposeInstance()
        {
            Save();
            base.DisposeInstance();
        }

        #region 封装接口
        /// <summary>
        /// TODO 需要封装对 TotalPlayerPrefsKeyRecord 的过滤
        /// </summary>
        public static void DeleteAll()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog("提示", "当前操作是否确定要删除所有的本地化保存的 PlayerPrefs 数据!! ", "删除", "取消") == false)
                return;
#endif

            mAllPlayerPrefsKeys.Clear();
            PlayerPrefs.DeleteAll();
            Debug.LogInfor("删除了所有的本地化 PlayerPrefs 数据, 时间 " + System.DateTime.Now.ToLongTimeString());
        }
        public static void DeleteKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("DeleteKey Fail,Parameter is null");
                return;
            }

            if (mAllPlayerPrefsKeys.Contains(key))
                mAllPlayerPrefsKeys.Remove(key);
            PlayerPrefs.DeleteKey(key);
        }

        //TODO 需要检测这里是否会执行
        public static void Save()
        {
            PlayerPrefs.SetString(TotalPlayerPrefsKeyRecord, SerilazeManager.SerializeObject(mAllPlayerPrefsKeys));
            PlayerPrefs.Save();
        }
        public static bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("DeleteKey Fail,Parameter is null");
                return false;
            }
            return PlayerPrefs.HasKey(key);
        }







        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public static string GetString(string key, string defaultValue = ConstDefine.S_StringEmpty)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }




        public static void SetFloat(string key, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("SetFloat Fail,Parameter is null");
                return;
            }
            if (mAllPlayerPrefsKeys.Contains(key) == false)
                mAllPlayerPrefsKeys.Add(key);
            PlayerPrefs.SetFloat(key, value);
        }
        public static void SetInt(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("SetInt Fail,Parameter is null");
                return;
            }
            if (mAllPlayerPrefsKeys.Contains(key) == false)
                mAllPlayerPrefsKeys.Add(key);
            PlayerPrefs.SetInt(key, value);
        }
        public static void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("SetString Fail,Parameter is null");
                return;
            }
            if (mAllPlayerPrefsKeys.Contains(key) == false)
                mAllPlayerPrefsKeys.Add(key);
            PlayerPrefs.SetString(key, value);
        }

        #endregion



    }
}