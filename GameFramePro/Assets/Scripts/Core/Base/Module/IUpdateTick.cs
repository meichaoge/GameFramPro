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
        uint TickPerUpdateCount { get; }

        /// <summary>
        /// 检测是否满足被调用UpdateTick 刷新的逻辑
        /// </summary>
        bool CheckIfNeedUpdateTick();

        /// <summary>
        /// 计时滴答
        /// </summary>
        /// <param name="currentTime">当前的时间(单位秒)</param>
        void UpdateTick(float currentTime); //定时获取状态

    }
}