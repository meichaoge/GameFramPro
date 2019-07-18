using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 非物理逻辑模块 这个是在update 模块刷新的
    /// </summary>
    public interface IUpdateTick
    {      /// <summary>
           /// 定义每这个数量的Mono  Update 更新一次
           /// </summary>
        int TickPerUpdateCount { get; }

        void UpdateTick(); //定时获取状态

    }
}