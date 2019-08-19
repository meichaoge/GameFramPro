using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using System;
 
 
 namespace GameFramePro.EventSystemEx
 {
     /// <summary>
     /// 移动平台
     /// </summary>
     public class MobileInputManager : IInputManager
     {

         public void CheckEventState()
         {
             
             if(Input.touchCount==0) return;  //没有任何触摸事件



         }
       
     }
 }