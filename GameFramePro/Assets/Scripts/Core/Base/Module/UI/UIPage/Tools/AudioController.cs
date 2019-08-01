using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;


namespace GameFramePro
{
    /// <summary>
    /// 作为一个组件 组合在其他模块中，提供访问AudioManger 的功能
    /// </summary>
    public class AudioController
    {
        private static AudioController s_GlobalAudioController = null;

        public static AudioController S_GlobalAudioController
        {
            get
            {
                if (s_GlobalAudioController == null)
                    s_GlobalAudioController = new AudioController();
                return s_GlobalAudioController;
            }
        } //全局的声音控制器


        private HashSet<string> mAllPlayAudioRecords = new HashSet<string>();


        #region 背景音
        //播放背景音
        public void PlayBackgroundAudioSync(string audioPath, float volume, bool isforceReplay = false)
        {
            if (string.IsNullOrEmpty(audioPath))
            {
                Debug.LogError("参数不能为null");
                return;
            }

            AudioManager.S_Instance.PlayBackgroundAudioSync(audioPath, volume, isforceReplay);
        }

        //停止背景音
        public void StopBackgroundAudio(bool isReleaseClip = false)
        {
            AudioManager.S_Instance.StopBackgroundAudio(isReleaseClip);
        }

        #endregion

        #region 普通音效
        //播放普通音效
        public void PlayNormalAudioSync(string audioPath, float volume, bool isforceReplay = false)
        {
            if (string.IsNullOrEmpty(audioPath))
            {
                Debug.LogError("参数不能为null");
                return;
            }

            if (AudioManager.S_Instance.PlayNormalAudioSync(audioPath, volume, isforceReplay))
            {
                if (mAllPlayAudioRecords.Contains(audioPath) == false)
                {
#if UNITY_EDITOR
                    Debug.LogError("PlayNormalAudioSync 记录失败,已经记录了key " + audioPath);
#endif
                }
                else
                {
                    mAllPlayAudioRecords.Add(audioPath);
                }
            }
        }

        //停止普通音效
        public void StopNormalAudio(string audioPath, bool isReleaseClip = false)
        {
            AudioManager.S_Instance.StopNormalAudio(audioPath, isReleaseClip);
            if (mAllPlayAudioRecords.Contains(audioPath))
                mAllPlayAudioRecords.Remove(audioPath);
        }

        #endregion

        /// <summary>
        /// 停止当前管理器管理的所有音效
        /// </summary>
        /// <param name="isContainBgAudio"></param>
        public void StopAllAudios(bool isContainBgAudio = true)
        {
            if (isContainBgAudio)
                StopBackgroundAudio(false);

            if (mAllPlayAudioRecords.Count == 0) return;
            foreach (var audioRecord in mAllPlayAudioRecords)
                StopNormalAudio(audioRecord, false);
        }
        
    }
}