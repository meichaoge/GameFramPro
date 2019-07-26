using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    /// <summary>
    /// 特指UGUI  组件
    /// </summary>
    public class UGUIComponent : IUIComponent
    {
        private Dictionary<string, GameObject> mAllUIPageInstanceChildNodes = new Dictionary<string, GameObject>();  //缓存所有获取过的UI节点

        private Dictionary<string, Button> mAllReferenceButtons = new Dictionary<string, Button>();
        private Dictionary<string, Text> mAllReferenceTexts = new Dictionary<string, Text>();
        private Dictionary<string, Image> mAllReferenceImages = new Dictionary<string, Image>();

        private Dictionary<string, Component> mAllReferenceComponent = new Dictionary<string, Component>(); //所有脚本中引用的组件

        public void InitailedComponentReference()
        {

        }

        #region 接口实现

        public void AddButtonListenner(string buttonName, Delegate click)
        {
            throw new NotImplementedException();
        }

        public void RemoveButtonListenner(string buttonName)
        {
            throw new NotImplementedException();
        }

        public void SetSprite(string imageName, Sprite sp)
        {
            throw new NotImplementedException();
        }

        public void SetText(string textName, string textStr)
        {
            throw new NotImplementedException();
        }



        #endregion


        #region 辅助
        //public GameObject FindChildGameObjectByName(string name)
        //{

        //}

        #endregion



    }


}