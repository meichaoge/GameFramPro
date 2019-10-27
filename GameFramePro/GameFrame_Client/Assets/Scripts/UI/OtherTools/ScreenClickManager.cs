using System.Collections;
using System.Collections.Generic;
using GameFramePro.ResourcesEx;
using GameFramePro.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro
{
    /// <summary>/// 屏幕点击特效管理  确保AppSetting.S_IsClickEffectEnable=true 这个脚本才能有效 /// </summary>
    public class ScreenClickManager : Single<ScreenClickManager>, IUpdateCountTick
    {
        //屏幕特效的信息
        private class ScreenEffectInfor
        {
        //    private BaseBeReferenceGameObjectInformation EffectItem;
        private GameObject mEffectItem;
            public float RemainTime { get; private set; } //剩余可以显示的时间
            public bool IsShowing { get; private set; } = false;

            private ParticleSystem[] mAllParticleSystems;

//            public void SetEffetctItem(BaseBeReferenceGameObjectInformation information)
//            {
//               // EffectItem = information;
//                mAllParticleSystems = information.GetComponentsInChildren<ParticleSystem>(true);
//            }
            
            public void SetEffetctItem(GameObject information)
            {
                mEffectItem = information;
                mAllParticleSystems = information.GetComponentsInChildren<ParticleSystem>(true);
            }

            //Update 中调用 用于刷新每个特效项的存活时间
            public void Tick(float dealtTime)
            {
                if (IsShowing == false) return;
                RemainTime -= dealtTime;
                if (RemainTime <= 0)
                {
                    RemainTime = 0;
                    HideEffect();
                }
            }

            //在指定的位置播放这个特效
            public void PlayEffect(Vector2 localPos, float maxAliveTime)
            {
                if (IsShowing) return;
                IsShowing = true;
                if (mEffectItem != null)
                    mEffectItem.transform.localPosition=localPos;
                RemainTime = maxAliveTime;

                if (mAllParticleSystems == null) return;
                foreach (var target in mAllParticleSystems)
                {
                    if (target == null) continue;
                    target.enableEmission = true;
                    target.Play(false);
                }
            }

            public void HideEffect(ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting)
            {
                if (IsShowing == false) return;
                IsShowing = false;
                if (mAllParticleSystems == null) return;
                foreach (var target in mAllParticleSystems)
                {
                    if (target == null) continue;
                    target.Stop(false, stopBehavior);
                    target.enableEmission = false;
                }
            }
        }

        #region Reference

        private Camera mUICamera
        {
            get { return UIPageManager.S_UICamera; }
        }

        private Transform mScreenEffectCanvasTrans; //最上层屏幕点击特效层
        private List<ScreenEffectInfor> mAllScreenEffectItems = new List<ScreenEffectInfor>(5); //所有的粒子特效 

        #endregion

        #region State

        private float maxShowTime = 0.6f; //最大的显示时间 超过时间粒子停止播放
        public Vector3 ClickPositionInWorld { get; private set; }
        private Vector3 EffectLocalPos; //点击时候特效应该相对于屏幕特效的层级

        private float lastClickTime = 0; //记录上一次点击屏幕的时间  减少刷新粒子状态
        private static bool IsNeedCheckClickEvent = true; //表示是否 监听点击事件

        #endregion


        #region 初始化

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            GetTargetScreenEffectCanvasTrans();
        }


        private void GetTargetScreenEffectCanvasTrans()
        {
            if (mScreenEffectCanvasTrans != null) return;
            GameObject go = GameObject.Find("TopScreenEffect");
            if (go == null)
            {
                Debug.LogError($"屏幕特效管理器初始化失败，需要一个名为 ['TopScreenEffect'] 的屏幕特效层来显示屏幕点击特效 ");
                return; //**下面为应该的设置
//                go = ResourcesManager.Instantiate("TopScreenEffect");
//                Canvas canvas=    go.AddComponent<Canvas>();
//                canvas.sortingLayerName = "TopScreenEffect";
//                canvas.renderMode = RenderMode.ScreenSpaceCamera;
//                canvas.worldCamera = UIPageManager.S_UICamera;
//                
//                go.AddComponent<GraphicRaycaster>();
//                CanvasScaler canvasScaler=        go.AddComponent<CanvasScaler>();
//                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//                canvasScaler.referenceResolution = AppSetting.S_ReferenceResolution;
            }

            mScreenEffectCanvasTrans = go.transform;
        }

        #endregion

        #region IUpdateTick 接口

        protected int curUpdateCount = 0; //当前的帧基数
        public uint TickPerUpdateCount { get; protected set; } = 1;

        public bool CheckIfNeedUpdateTick()
        {
            
            if (ApplicationManager.S_Instance.mApplicationConfigureSettings.mIsClickEffectEnable == false)
                return false;
            return true;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick() == false) return;
            UpdateAction();
        }

        #endregion


        private void UpdateAction()
        {
            if (Time.time - lastClickTime < maxShowTime + Time.deltaTime)
            {
                foreach (var effectItem in mAllScreenEffectItems)
                    effectItem.Tick(Time.deltaTime);
            } //+Time.deltaTime 确保上一次执行完了状态修改  避免没有必要的循环

            if (IsNeedCheckClickEvent == false)
                return;

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Vector3 position = Input.mousePosition;
                ClickPositionInWorld = mUICamera.ScreenToWorldPoint(position);
                EffectLocalPos = mScreenEffectCanvasTrans.InverseTransformPoint(ClickPositionInWorld);
                EffectLocalPos.z = 0;
                lastClickTime = Time.time;

                bool needCreateItem = true;
                foreach (var effectItem in mAllScreenEffectItems)
                {
                    if (effectItem.IsShowing == false)
                    {
                        needCreateItem = false;
                        effectItem.PlayEffect(EffectLocalPos, maxShowTime);
                        break;
                    }
                }

                if (needCreateItem)
                {

                    GameObject go = ResourcesManager.InstantiateAssetSync(EffectDefine.S_ScreenClickEffectPath, mScreenEffectCanvasTrans, true);
                    if (go == null)
                    {
                        Debug.LogError("获取资源为null ");
                        return;
                    }
                    
//                    BaseBeReferenceGameObjectInformation information = ResourcesManager.InstantiateGameObjectByPathSync(mScreenEffectCanvasTrans, EffectDefine.S_ScreenClickEffectPath, true);
//                    if (information == null)
//                    {
//                        Debug.LogError("获取资源为null ");
//                        return;
//                    }
//
//                    if (information.IsReferenceAssetEnable == false)
//                    {
//                        information.ReduceReference();
//                        return;
//                    }

                    ScreenEffectInfor effectInfor = new ScreenEffectInfor();

                    effectInfor.SetEffetctItem(go);
                    effectInfor.PlayEffect(EffectLocalPos, maxShowTime);
                    mAllScreenEffectItems.Add(effectInfor);
                } //新建特效
            }
        }


        #region 对外接口

        /// <summary>
        /// 暂时禁用点击特效
        /// </summary>
        public static void StopCickEffect()
        {
            IsNeedCheckClickEvent = false;
        }

        public static void RestartCickEffect()
        {
            IsNeedCheckClickEvent = true;
        }

        #endregion
    }
}
