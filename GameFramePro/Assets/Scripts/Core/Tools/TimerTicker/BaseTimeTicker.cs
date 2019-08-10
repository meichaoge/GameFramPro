using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 计时器基类/// </summary>
    [System.Serializable]
    public class BaseTimeTicker
    {
        public System.Action<float, int> m_CallbackAc  { get; protected set; } = null; //回调包含时间 和Hashcode
        public int m_HashCode { protected set; get; }
        public float m_SpaceTime; //间隔
        public bool m_IsEnable { get; protected set; } = true; //标识这个计时器是否有效

        protected virtual void InitialTimer()
        {
            m_HashCode = GetHashCode();
            m_IsEnable = true;
        }


        /// <summary>/// 每一帧调用/// </summary>
        public virtual void TimeTicked()
        {
        }

        public void DeleteTimer()
        {
            m_IsEnable = false;
        }
    }
}
