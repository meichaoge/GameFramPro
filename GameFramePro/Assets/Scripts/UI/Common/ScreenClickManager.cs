using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 屏幕点击特效管理  
    /// </summary>
    public class ScreenClickManager : MonoBehaviour
    {
        //屏幕特效的信息
        private class ScreenEffectInfor
        {
            public GameObject EffectItem;
            public float RemainTime = 0;
            public bool IsShowing = true;
        }
        

        //shouzhidianji 特效名
        public GameObject effect;
        private float maxShowTimw = 0.6f; //最大的显示时间
        private  static bool IsNeedCheckClickEvent = true; //表示是否 监听点击事件


        public Vector3 ClickPositionInWorld { get; private set; }
        private Vector3 EffectLocalPos;  //点击时候特效应该相对于屏幕特效的层级


        private List<ScreenEffectInfor> mAllScreenEffectItems = new List<ScreenEffectInfor>(5);

        private float lastClickTime = 0;

        ScreenEffectInfor effectItem = null;

     


        private void Update()
        {
            if (effect == null)
                return;

            if (Time.time - lastClickTime < maxShowTimw + Time.deltaTime)
            {
                for (int dex = 0; dex < mAllScreenEffectItems.Count; dex++)
                {
                    effectItem = mAllScreenEffectItems[dex];
                    if (effectItem.IsShowing == false)
                        continue;
                    effectItem.RemainTime -= Time.deltaTime;
                    if (effectItem.RemainTime <= 0)
                    {
                        effectItem.EffectItem.SetActive(false);
                        effectItem.IsShowing = false;
                    }
                }
            } //+Time.deltaTime 确保上一次执行完了状态修改  避免没有必要的循环


            if (IsNeedCheckClickEvent == false) return;

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Vector3 position = Input.mousePosition;
                ClickPositionInWorld = Camera.main.ScreenToWorldPoint(position);
                EffectLocalPos = transform.InverseTransformPoint(ClickPositionInWorld);
                EffectLocalPos.z = 0;
                lastClickTime = Time.time;

                bool needCreateItem = true;
                for (int dex = 0; dex < mAllScreenEffectItems.Count; dex++)
                {
                    effectItem = mAllScreenEffectItems[dex];
                    if (effectItem.IsShowing == false)
                    {
                        needCreateItem = false;
                        effectItem.EffectItem.SetActive(true);
                        effectItem.RemainTime = maxShowTimw;
                        effectItem.EffectItem.transform.localPosition = EffectLocalPos;
                        effectItem.IsShowing = true;
                        break;
                    }
                }

                if (needCreateItem)
                {
                    ScreenEffectInfor infor = new ScreenEffectInfor();
                    infor.EffectItem = Instantiate(effect, transform) as GameObject;
                    //     infor.EffectItem.transform.SetParent(transform);

                    infor.EffectItem.transform.localPosition = EffectLocalPos;
                    infor.IsShowing = true;
                    infor.RemainTime = maxShowTimw;
                    mAllScreenEffectItems.Add(infor);
                }
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