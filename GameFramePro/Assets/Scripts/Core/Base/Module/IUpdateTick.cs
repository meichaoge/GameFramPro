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
    {
        float TickTimeInterval { get; } //每次被调用的时间间隔

        /// <summary>
        /// 被调用，参数是 Time.realtimeSinceStartup
        /// </summary>
        /// <param name="currentTime"></param>
        void Tick(float realtimeSinceStartup);  

    }
}