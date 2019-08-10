using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>/// 定时刷新的计时器接口/// </summary>
    public interface IUpdateTimeTick : IUpdateTick
    {
        /// <summary>/// 刷新的时间间隔/// </summary>
        float TickPerTimeInterval { get; }
    }
}
