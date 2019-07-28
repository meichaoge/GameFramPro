using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 音效管理器
    /// </summary>
    public class AudioManager : Single_Mono_AutoCreateNotDestroy<AudioManager>
    {
        #region 背景音

        private Dictionary<string, AudioClip> mAllBackgroundAudioClips = new Dictionary<string, AudioClip>(); //背景音
       // private AudioClip mCurBackgroundAudioClip = null; //当前的背景音
        private AudioSource mCurBackgroundAudioSource; //当前的背景音 播放组件
        #endregion



        protected override void Awake()
        {
            base.Awake();
            mCurBackgroundAudioSource = transform.GetAddComponent<AudioSource>();
        }



        #region 播放声音接口
        /// <summary>
        /// 播放背景音
        /// </summary>
        /// <param name="audioPath"></param>
        /// <param name="audioName"></param>
        /// <param name="volume"></param>
        public void PlayBackgroundAudio(string audioPath, float volume,bool isLoop=true,bool isForeceReplay=false)
        {
            string audioName = IOUtility.GetFileNameWithoutExtensionEx(audioPath);

            if (mCurBackgroundAudioSource.clip != null)
            {
                if(mCurBackgroundAudioSource.clip.name== audioName)
                {
                    mCurBackgroundAudioSource.volume = volume;
                    mCurBackgroundAudioSource.loop = isLoop;
                    if (isForeceReplay)
                    {
                        mCurBackgroundAudioSource.Stop();
                        mCurBackgroundAudioSource.Play();
                    }
                    return;
                }//当前正在播放这个背景音
            }

            AudioClip clip = null;
            if(mAllBackgroundAudioClips.TryGetValue(audioPath,out clip))
            {
                PlayBackgroundAudio(clip, volume,isLoop,isForeceReplay);
            }
            else
            {
                //TODO  这里不能直接赋值
                ResourcesManager.LoadAudioClipAssetSync(audioPath, mCurBackgroundAudioSource);
            }
        }

        private void PlayBackgroundAudio(AudioClip clip, float volume, bool isLoop = true, bool isForeceReplay = false)
        {
            if (mCurBackgroundAudioSource.clip != null)
            {
                if (mCurBackgroundAudioSource.clip.name == clip.name)
                {
                    mCurBackgroundAudioSource.volume = volume;
                    mCurBackgroundAudioSource.loop = isLoop;
                    if (isForeceReplay)
                    {
                        mCurBackgroundAudioSource.Stop();
                        mCurBackgroundAudioSource.Play();
                    }
                    return;
                }//当前正在播放这个背景音
                else
                {
                    mCurBackgroundAudioSource.Stop();
                }//结束当前的背景音
            }

            mCurBackgroundAudioSource.clip = clip;
            mCurBackgroundAudioSource.volume = volume;
            mCurBackgroundAudioSource.loop = isLoop;
            mCurBackgroundAudioSource.Play();
        }

        #endregion



    }
}