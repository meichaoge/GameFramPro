using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化
    /// </summary>
    public class UGUIComponentReference:MonoBehaviour
    {
        public enum UGUIComponentTypeEnum
        {
            GameObject,
            Transform,
            Text,
            Button,
            Image,
        }




        public List<Component> mAllSerializeUGUIComponents = new List<Component>();


        //public class UGUIComponentInfor
        //{

        //}






    }
}