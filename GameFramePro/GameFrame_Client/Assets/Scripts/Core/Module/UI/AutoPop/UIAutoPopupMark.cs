using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.UI
{
    //记录自动弹窗的信息
    public class UIAutoPopupMarkInfor
    {
        public string mPagePrefabName;
        public string mPagePrefabPath;
        public int mPageID;  //用于区分同一个预制体生成的不同实例
        public float mWidget; //权重
        public object mOtherConfigure = null;// 其他配置信息

        public UIAutoPopupMarkInfor(string name, string path, int id, float widget, object other)
        {
            mPagePrefabName = name;
            mPagePrefabPath = path;
            mPageID = id;
            mWidget = widget;
            mOtherConfigure = other;
        }
    }


    /// <summary>
    /// 挂载在自动弹出的窗口上用于标识和信息记录
    /// </summary>
    public class UIAutoPopupMark : MonoBehaviour
    {
        public UIAutoPopupMarkInfor mUIAutoPopupMarkInfor { get;  set; }


        //public void OnDisable()
        //{
        //    UIHomeAutoPopupManager.CheckAndAutoPopup();
        //}

    }
}