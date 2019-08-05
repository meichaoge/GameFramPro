using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 引用计数的接口
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        int ReferenceCount { get; }

        /// <summary>
        /// 增加引用计数
        /// </summary>
        void AddReference();
        
        /// <summary>
        /// 减少引用计数
        /// </summary>
        /// <param name="isForceDelete">是否强制减少引用计数</param>
        void ReduceReference(bool isForceDelete = false);
    }
}
