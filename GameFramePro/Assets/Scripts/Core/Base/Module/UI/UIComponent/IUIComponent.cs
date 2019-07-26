using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 用于继承自UIBasePage 对象获取UI预制体上的组件
    /// </summary>
    public interface IUIComponent
    {

        //Transform FindChildByName(string searchName);
        //Transform FindChildByPath(string searchPath);



        void AddButtonListenner(string buttonName, Delegate click);
        void RemoveButtonListenner(string buttonName);
        void SetText(string textName, string textStr);
        void SetSprite(string imageName, Sprite sp);


    }
}