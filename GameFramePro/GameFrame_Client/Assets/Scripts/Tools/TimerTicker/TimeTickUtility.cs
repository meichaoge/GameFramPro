using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 全局的计时器/// </summary>
    public class TimeTickUtility : Single<TimeTickUtility>, IUpdateCountTick
    {
        private Dictionary<int, BaseTimeTicker> m_AllTimerCallback = new Dictionary<int, BaseTimeTicker>();
        private List<BaseTimeTicker> m_AllDeadTimers = new List<BaseTimeTicker>(); //需要被注销的计时器

#if UNITY_EDITOR
        public List<BaseTimeTicker> m_AllTestTimers = new List<BaseTimeTicker>(); //测试显示数据
#endif


        #region IUpdateTick 接口

        public uint TickPerUpdateCount { get; protected set; } = 1;

        public void UpdateTick(float currentTime)
        {
            UpdateAllTickers();
        }

        #endregion


        private void UpdateAllTickers()
        {
#if UNITY_EDITOR

            m_AllTestTimers.Clear();
            m_AllTestTimers.AddRange(m_AllTimerCallback.Values);
#endif
            if (m_AllTimerCallback.Count == 0) return;
            if (m_AllDeadTimers.Count != 0)
            {
                for (int dex = 0; dex < m_AllDeadTimers.Count; ++dex)
                {
                    if (m_AllTimerCallback.ContainsKey(m_AllDeadTimers[dex].m_HashCode))
                        m_AllTimerCallback.Remove(m_AllDeadTimers[dex].m_HashCode);
                }

                m_AllDeadTimers.Clear();
            } //清理过时计时器

            foreach (var item in m_AllTimerCallback.Values)
            {
                if (item.m_IsEnable)
                    item.TimeTicked();
            }
        }

        /// <summary>///注册计时器(顺计时)/// </summary>
        public int RegisterTimer(float spaceTime, System.Action<float, int> callback)
        {
            ClockWiseTimeTicker recordInfor = new ClockWiseTimeTicker(spaceTime, callback);
            if (m_AllTimerCallback.ContainsKey(recordInfor.m_HashCode))
            {
                Debug.LogError("RegisterTimer  Fail");
                return 0;
            }

            m_AllTimerCallback.Add(recordInfor.m_HashCode, recordInfor);
            return recordInfor.m_HashCode;
        }


        /// <summary>/// 注册倒计时计时器 (到计时为0时候自动取消)/// </summary>
        /// <param name="deadTime">倒计时时长</param>
        public int RegisterCountDownTimer(float spaceTime, float deadTime, System.Action<float, int> callback)
        {
            CountDownTimeTicker recordInfor = new CountDownTimeTicker(spaceTime, deadTime, callback);
            if (m_AllTimerCallback.ContainsKey(recordInfor.m_HashCode))
            {
                Debug.LogError("RegisterCountDownTimer  Fail");
                return 0;
            }

            m_AllTimerCallback.Add(recordInfor.m_HashCode, recordInfor);
            return recordInfor.m_HashCode;
        }


        /// <summary>/// 标记为要删除的计时器/// </summary>
        public void UnRegisterTimer_Delay(int hashCode)
        {
            if (m_AllTimerCallback.TryGetValue(hashCode, out var timer))
            {
                timer.DeleteTimer();
                m_AllDeadTimers.Add(timer);
            }

            //Debug.LogError("UnRegisterTimer_Delay Fail,Not Exit" + hashCode);
        }
    }
}
