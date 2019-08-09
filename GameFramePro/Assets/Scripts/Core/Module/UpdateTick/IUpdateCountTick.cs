using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 固定帧率的计时器接口
    /// </summary>
    public interface IUpdateCountTick : IUpdateTick
    {
        /// <summary>
        /// 定义每这个数量的Mono  Update 更新一次
        /// </summary>
        uint TickPerUpdateCount { get; }
    }
}
