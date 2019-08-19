using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 倒计时计时器/// </summary>
    [System.Serializable]
    public class CountDownTimeTicker : BaseTimeTicker
    {
        public float m_DeadTime { get; protected set; } //倒计时时长
        public float m_LastRecordTime { get; protected set; }


        public CountDownTimeTicker(float spaceTime, float deadTime, System.Action<float, int> callback)
        {
            m_SpaceTime = spaceTime;
            m_CallbackAc = callback;
            m_DeadTime = deadTime;
            InitialTimer();
        }

        protected override void InitialTimer()
        {
            base.InitialTimer();
            m_LastRecordTime = Time.realtimeSinceStartup;
        }

        public override void TimeTicked()
        {
            m_DeadTime -= Time.unscaledDeltaTime;
            if (m_DeadTime <= 0)
            {
                m_CallbackAc?.Invoke(0, m_HashCode);
                TimeTickUtility.S_Instance.UnRegisterTimer_Delay(m_HashCode); //标记为要删除
                return;
            }

            if (Time.realtimeSinceStartup - m_LastRecordTime >= m_SpaceTime)
            {
                m_CallbackAc?.Invoke(m_DeadTime, m_HashCode);
                m_LastRecordTime = Time.realtimeSinceStartup;
            }
        }
    }
}
