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
        private const string TotalPlayerPrefsKeyRecord = "totalPlayerPrefsKeyRecordKeys";

        private HashSet<string> mAllPlayerPrefsKeys = new HashSet<string>();

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
        }



        #region 封装接口
        /// <summary>
        /// TODO 需要封装对 TotalPlayerPrefsKeyRecord 的过滤
        /// </summary>
        public static void DeleteAll() { PlayerPrefs.DeleteAll(); }
        public static void DeleteKey(string key) { PlayerPrefs.DeleteKey(key); }
        public static void Save() { PlayerPrefs.Save(); }
        public static bool HasKey(string key) { return PlayerPrefs.HasKey(key); }







        public static float GetFloat(string key, float defaultValue=0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        public static int GetInt(string key, int defaultValue=0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public static string GetString(string key, string defaultValue="")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }




        public static void SetFloat(string key, float value )
        {
            PlayerPrefs.SetFloat(key, value);
        }
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        #endregion



    }
}