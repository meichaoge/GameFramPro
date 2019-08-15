using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 正常的 顺计时 计时器/// </summary>
    public class ClockWiseTimeTicker : BaseTimeTicker
    {
        public float m_CurrentTime { get; protected set; } = 0; //计时过去多久
        public float m_LastRecordTime { get; protected set; } = 0; //上一帧时候记录距离上一次到达m_SpaceTime 间隔过去的时间


//        public ClockWiseTimeTicker()
//        {
//        }

        public ClockWiseTimeTicker(float spaceTime, System.Action<float, int> callback)
        {
            m_SpaceTime = spaceTime;
            m_CallbackAc = callback;
            InitialTimer();
        }


        protected override void InitialTimer()
        {
            base.InitialTimer();
            m_CurrentTime = 0;
            m_LastRecordTime = Time.realtimeSinceStartup;
        }

        /// <summary>/// 每一帧调用/// </summary>Normal
        public override void TimeTicked()
        {
            m_CurrentTime += Time.unscaledDeltaTime;
            if (Time.realtimeSinceStartup - m_LastRecordTime >= m_SpaceTime)
            {
                m_CallbackAc?.Invoke(m_CurrentTime, m_HashCode);
                m_LastRecordTime = Time.realtimeSinceStartup;
            }
        }
    }
}
