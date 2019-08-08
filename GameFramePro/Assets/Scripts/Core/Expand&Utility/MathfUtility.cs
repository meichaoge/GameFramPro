using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    public static class MathfUtility
    {
        /// <summary>
        /// 获取最接近的2次幂的值 (最接近data 的2次幂)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetNearnestPowerNumber(int data, int maxPower = 11, int minPower = 5)
        {
            int temple1 = 1;
            int temple2 = 1;

            for (int dex = 1; dex < maxPower; ++dex)
            {
                temple2 = temple1 << 1;
                if (temple2 >= data && temple1 < data)
                {
                    if (temple2 - data <= data - temple1)
                        return temple2;
                    return temple1;
                }//说明结果在 temple2 与temple1 之间
                temple1 = temple1 << 1;
            }
            temple1 = temple1 << 1;
            if (temple1 < Mathf.Pow(2, minPower))
                temple1 = (int)Mathf.Pow(2, minPower);
            Debug.Log(string.Format("超过最大的大小  返回默认最大值maxPower ={0} 2^ maxPower= {1}", maxPower, temple1));
            return temple1;
        }


        /// <summary>
        /// 获得比元素据大的最小2次幂
        /// </summary>
        /// <param name="data"></param>
        /// <param name="maxPower"></param>
        /// <param name="minPower">Unity 最小的格式是32</param>
        /// <returns></returns>
        public static int GetMinMaxPowerNumber(int data, int maxPower = 11, int minPower = 5)
        {
            int temple1 = 1;
            for (int dex = 1; dex < maxPower; ++dex)
            {
                temple1 = temple1 << 1;
                if (temple1 >= data)
                    break;
            }
            if (temple1 < Mathf.Pow(2, minPower))
                temple1 = (int)Mathf.Pow(2, minPower);
            return temple1;
        }



    }
}