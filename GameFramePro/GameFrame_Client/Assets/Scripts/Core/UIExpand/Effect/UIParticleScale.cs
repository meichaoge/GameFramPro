using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameFramePro
{
    /// <summary>
    /// 适用于UI特效缩放
    /// </summary>
    public class UIParticleScale : MonoBehaviour
    {
        private Dictionary<Transform, Vector3> mAllScaleDatas = new Dictionary<Transform, Vector3>();
        float designWidth = 0;//开发时分辨率宽
        float designHeight = 0;//开发时分辨率高

#if UNITY_EDITOR
        [Header("编辑器下标示是否运行时实时更新，默认false")]
        public bool mRealTimeUpdateOnEditor = false;
#endif

        void Awake()
        {

        }


        private void GetAllTarget()
        {
            foreach (ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (mAllScaleDatas.ContainsKey(p.transform) == false)
                    mAllScaleDatas[p.transform] = p.transform.localScale;
            }

            foreach (MeshRenderer p in transform.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mAllScaleDatas.ContainsKey(p.transform) == false)
                    mAllScaleDatas[p.transform] = p.transform.localScale;
            }

            GetCanvasSize();
        }

        private void GetCanvasSize()
        {
            CanvasScaler parentCanvasScale = transform.GetComponentInParent<CanvasScaler>();
            if (parentCanvasScale == null || parentCanvasScale.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
#if UNITY_EDITOR
                Debug.LogError("特效适配失败 Canvas对象({0})上CanvasScaler 的模式不是ScaleWithScreenSize");
#endif
                return;
            }
            designWidth = parentCanvasScale.referenceResolution.x;
            designHeight = parentCanvasScale.referenceResolution.y;
        }

        void Start()
        {

            GetAllTarget();

            //  return;
            AutoAdapter();
        }

        private void AutoAdapter()
        {

            if (designWidth == 0 || designHeight == 0) return;
            float designScale = designWidth / designHeight;
            float scaleRate = (float)Screen.width / (float)Screen.height;

            foreach (var item in mAllScaleDatas)
            {
                if (item.Key != null)
                {
                    if (scaleRate < designScale)
                    {
                        float scaleFactor = scaleRate / designScale;
                        item.Key.localScale = item.Value * scaleFactor;
                    }
                    else
                    {
                        item.Key.localScale = item.Value;
                    }
                }
            }
        }



#if UNITY_EDITOR
        //[SerializeField]
        //private List<Transform> mAllTarget = new List<Transform>();
        void Update()
        {
            if (mRealTimeUpdateOnEditor)
            {
                GetAllTarget();
                AutoAdapter(); ; //Editor下修改屏幕的大小实时预览缩放效果
            }

        }
#endif
    }
}