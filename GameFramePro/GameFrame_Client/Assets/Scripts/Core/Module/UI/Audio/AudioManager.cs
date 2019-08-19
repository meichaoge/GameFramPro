using System;
using System.Collections;
using System.Collections.Generic;
using GameFramePro.ResourcesEx;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 音效管理器/// </summary>
    public class AudioManager : Single_Mono<AudioManager>, IUpdateCountTick
    {
        protected override bool IsNotDestroyedOnLoad { get;  } = true; //标示不会一起销毁


        #region 背景音

        private readonly Dictionary<string, BaseBeReferenceInformation> mAllBackgroundAudioClips = new Dictionary<string, BaseBeReferenceInformation>(); //背景音
        private AudioSource mCurBackgroundAudioSource; //当前的背景音 播放组件

        #endregion

        #region 普通的声音

        private int CurNormalAudioSourceCount { get; set; } = 0; //当前的普通音效组件个数
        private Stack<AudioSource> mAllAvailableAudioSources; //可以使用的普通音效组件
        private readonly LinkedList<AudioSource> mAllPlayingAudioSources = new LinkedList<AudioSource>(); //正在播放中的音效

        #endregion

        protected override void Awake()
        {
            base.Awake();
            mCurBackgroundAudioSource = transform.GetAddComponent<AudioSource>();
            InitialedNormalAudioSources();
        }

        private void LateUpdate()
        {
            UpdateTick(0);
        }


        #region IUpdateTick 接口

        private int curUpdateCount = 0; //当前的帧基数
        public uint TickPerUpdateCount { get; protected set; } = 10;

        public bool CheckIfNeedUpdateTick()
        {
            ++curUpdateCount;
            if (curUpdateCount == 1)
                return true; //确保第一次被调用

            if (curUpdateCount < TickPerUpdateCount)
                return false;

            curUpdateCount = 0;
            return true;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick() == false) return;
            CheckCompleteAudioSources();
        }

        #endregion


        #region 播放 & 停止  背景声音接口

        /// <summary>
        /// 播放背景音
        /// </summary>
        /// <param name="audioPath"></param>
        /// <param name="volume"></param>
        /// <param name="isLoop"></param>
        /// <param name="isforceReplay"></param>
        /// <returns></returns>
        public bool PlayBackgroundAudioSync(string audioPath, float volume, bool isLoop = true, bool isforceReplay = false)
        {
            if (string.IsNullOrEmpty(audioPath))
                return false;

            string audioName = IOUtility.GetFileNameWithoutExtensionEx(audioPath);
            if (string.IsNullOrEmpty(audioName))
                return false;
            if (mCurBackgroundAudioSource.clip != null)
            {
                if (mCurBackgroundAudioSource.clip.name == audioName)
                {
                    mCurBackgroundAudioSource.volume = volume;
                    mCurBackgroundAudioSource.loop = isLoop;
                    if (isforceReplay)
                    {
                        mCurBackgroundAudioSource.Stop();
                        mCurBackgroundAudioSource.Play();
                    }

                    return true;
                } //当前正在播放这个背景音
            }

            if (mAllBackgroundAudioClips.TryGetValue(audioPath, out var clipAsset) == false)
                clipAsset = ResourcesManager.GetAudioClipByPathSync(mCurBackgroundAudioSource, audioPath, false, false);

            if (clipAsset == null || clipAsset.IsReferenceAssetEnable == false)
                return false;
            return PlayBackgroundAudioSync(clipAsset, volume, isLoop, isforceReplay);
        }


        private bool PlayBackgroundAudioSync(BaseBeReferenceInformation clip, float volume, bool isLoop = true, bool isforceReplay = false)
        {
            if (clip == null || clip.IsReferenceAssetEnable == false)
                return false;

            if (mCurBackgroundAudioSource.clip != null)
            {
                if (mCurBackgroundAudioSource.clip.name == clip.AssetName)
                {
                    mCurBackgroundAudioSource.volume = volume;
                    mCurBackgroundAudioSource.loop = isLoop;
                    if (isforceReplay)
                    {
                        mCurBackgroundAudioSource.Stop();
                        mCurBackgroundAudioSource.Play();
                    }

                    return true;
                } //当前正在播放这个背景音
                else
                    mCurBackgroundAudioSource.Stop(); //结束当前的背景音
            }

            clip.SetAudioClip(mCurBackgroundAudioSource);
            mCurBackgroundAudioSource.volume = volume;
            mCurBackgroundAudioSource.loop = isLoop;
            mCurBackgroundAudioSource.Play();

            return true;
        }

        /// <summary>///  停止当前背景/// </summary>
        /// <param name="isReleaseClip">是否在停止的时候释放引用的音效资源</param>
        public void StopBackgroundAudio(bool isReleaseClip = false)
        {
            if (mCurBackgroundAudioSource == null) return;
            if (null == mCurBackgroundAudioSource.clip) return;

            mCurBackgroundAudioSource.Stop();
            if (isReleaseClip)
            {
                mCurBackgroundAudioSource.clip = null;
                ResourcesUtility.ReleaseComponentReference(mCurBackgroundAudioSource);
            }
        }

        /// <summary>/// 停止播放背景/// </summary>
        /// <param name="isReleaseClip">是否在停止的时候释放引用的音效资源</param>
        private void StopBackgroundAudio(string audioName, bool isReleaseClip = false)
        {
            if (mCurBackgroundAudioSource == null) return;
            if (mCurBackgroundAudioSource.clip == null) return;
            if (mCurBackgroundAudioSource.clip.name != audioName) return;

            mCurBackgroundAudioSource.Stop();
            if (isReleaseClip)
            {
                mCurBackgroundAudioSource.clip = null;
                ResourcesUtility.ReleaseComponentReference(mCurBackgroundAudioSource);
            }
        }

        #endregion

        #region 普通的音效

        /// <summary>/// 普通的音效播放/// </summary>
        /// <param name="isforceReplay">如果正在播放是否强制重播</param>
        public bool PlayNormalAudioSync(string audioPath, float volume, bool isforceReplay = false)
        {
            if (string.IsNullOrEmpty(audioPath))
                return false;
            string audioName = IOUtility.GetFileNameWithoutExtensionEx(audioPath);
            if (string.IsNullOrEmpty(audioName))
                return false;

            if (IsPlayingNormalAudio(audioName, out var curAudioSource))
            {
                if (isforceReplay)
                {
                    curAudioSource.Stop();
                    curAudioSource.Play();
                }

                return true;
            }

            curAudioSource = GetNextAvailableAudioSource();
            var assetInfor = ResourcesManager.GetAudioClipByPathSync(curAudioSource, audioPath, false, false);

            if (assetInfor == null || assetInfor.IsReferenceAssetEnable == false)
            {
                mAllAvailableAudioSources.Push(curAudioSource);
                assetInfor = null;
                return false;
            }
            else
            {
                assetInfor.SetAudioClip(curAudioSource);
                curAudioSource.volume = volume;
                mAllPlayingAudioSources.AddLast(curAudioSource);
                return true;
            }
        }

        /// <summary>/// 停止播放一个音效/// </summary>
        /// <param name="isReleaseClip">是否在停止的时候释放引用的音效资源</param>
        public void StopNormalAudio(string audioPath, bool isReleaseClip = false)
        {
            if (string.IsNullOrEmpty(audioPath))
                return;
            string audioName = IOUtility.GetFileNameWithoutExtensionEx(audioPath);
            if (string.IsNullOrEmpty(audioName))
                return;

            var isPlaying = IsPlayingNormalAudio(audioName, out var curAudioSources);
            if (isPlaying == false) return;
            curAudioSources.Stop();
            if (isReleaseClip)
            {
                curAudioSources.clip = null;
                ResourcesUtility.ReleaseComponentReference(curAudioSources);
            }

            RecycleNormalAudioSource(curAudioSources);
        }

        #endregion


        #region 辅助

        /// <summary>
        /// 根据指定的参数个数 初始化对应参数个数的普通的声音播放组件
        /// </summary>
        /// <param name="perfectCount"></param>
        private void InitialedNormalAudioSources(uint perfectCount = 2)
        {
            CurNormalAudioSourceCount = (int) perfectCount;
            mAllAvailableAudioSources = new Stack<AudioSource>((int) perfectCount);
            for (int index = 0; index < perfectCount; index++)
            {
                AudioSource audioSource = InstantiateAudioSourceInstance(string.Format("NormalAudio_{0}", index));
                audioSource.enabled = false;
                mAllAvailableAudioSources.Push(audioSource);
            }
        }

        /// <summary>
        /// 定时检测哪些任务完成了
        /// </summary>
        private void CheckCompleteAudioSources()
        {
            if (mAllPlayingAudioSources.Count == 0) return;
            var targetAudioSources = mAllPlayingAudioSources.First;
            while (targetAudioSources != null)
            {
                if (targetAudioSources.Value.isPlaying)
                {
                    targetAudioSources = targetAudioSources.Next;
                    continue;
                }

                var next = targetAudioSources.Next;
                RecycleNormalAudioSource(targetAudioSources.Value);
                mAllPlayingAudioSources.Remove(targetAudioSources);
                targetAudioSources = next;
            }
        }


        /// <summary>
        /// 实例化获取一个 AudioSources组件对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AudioSource InstantiateAudioSourceInstance(string name)
        {
            GameObject goInstance = ResourcesManager.Instantiate(name);
            goInstance.transform.SetParent(transform);
            return goInstance.GetAddComponentEx<AudioSource>();
        }

        /// <summary>
        /// 获取一个可用的普通音效组件
        /// </summary>
        /// <returns></returns>
        private AudioSource GetNextAvailableAudioSource()
        {
            AudioSource target = null;
            while (mAllAvailableAudioSources.Count > 0)
            {
                target = mAllAvailableAudioSources.Pop();
                if (target != null)
                {
                    if (target.isPlaying)
                    {
                        Debug.LogError(string.Format("有音效组件还在播放中，但是被放置到可用队列中 {0}", target.gameObject.name));
                    }
                    else
                    {
                        target.enabled = true;
                        return target;
                    }
                }
            }

            ++CurNormalAudioSourceCount;
            target = InstantiateAudioSourceInstance(string.Format("NormalAudio_{0}", CurNormalAudioSourceCount));
            return target;
        }

        /// <summary>
        /// 回收一个可用的普通音效组件
        /// </summary>
        /// <param name="target"></param>
        private void RecycleNormalAudioSource(AudioSource target)
        {
            if (target == null) return;
            if (target.isPlaying)
            {
                Debug.LogError("RecycleNormalAudioSource 失败，当期音效 {0} 还在播放中 ", target.name);
                return;
            }

            target.enabled = false;
            mAllAvailableAudioSources.Push(target);
        }

        /// <summary>
        /// 是否正在播放参数指定的普通音效
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        private bool IsPlayingNormalAudio(string audioName, out AudioSource curAudioSources)
        {
            curAudioSources = null;

            if (mAllPlayingAudioSources.Count == 0) return false;
            var targetAudioSources = mAllPlayingAudioSources.First;
            while (targetAudioSources != null)
            {
                if (targetAudioSources.Value.name == audioName)
                {
                    if (targetAudioSources.Value.isPlaying)
                    {
                        curAudioSources = targetAudioSources.Value;
                        return true;
                    }

                    var audioSource = targetAudioSources.Value;
                    Debug.LogInfor("检测到 音效{0} 播放完成，已经自动回收", audioSource.name);
                    RecycleNormalAudioSource(audioSource);
                    var next = targetAudioSources.Next;
                    mAllPlayingAudioSources.Remove(targetAudioSources);
                    targetAudioSources = next;
                    continue;
                }

                targetAudioSources = targetAudioSources.Next;
            }

            return false;
        }

        #endregion
    }
}
