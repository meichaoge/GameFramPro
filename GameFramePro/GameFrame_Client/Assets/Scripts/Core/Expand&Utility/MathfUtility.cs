using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    public static class MathfUtility
    {
        #region 2 的幂次方处理

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

        #endregion


        #region 随机数处理
        /// <summary>
        /// 获取指定随机数生成概率的随机数
        /// </summary>
        /// <param name="probabity"指定范围内元素生成的概率数组</param>
        /// <returns>返回对应每个概率值的数组索引</returns>
        public static int GetRandomOnProbability(int[] probabity)
        {
            int total = 100;
            int random = UnityEngine.Random.Range(1, 1 + total);
            int sum = 0;
            for (int dex = 0; dex < probabity.Length; dex++)
            {
                sum += probabity[dex];
                if (random < sum)
                    return dex;
            }
            return 0;
        }

        /// <summary>
        /// 随机重排 (Fisher–Yates shuffle算法) 复杂度O(n)
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] RandomSort(int[] sources)
        {
            int[] result = new int[sources.Length];
            System.Array.Copy(sources, result, sources.Length);

            for (int dex = result.Length - 1; dex >= 0; dex--)
            {
                int randomIndex = UnityEngine.Random.Range(0, dex + 1);
                if (randomIndex == dex) continue;
                int temp = result[dex];
                result[dex] = result[randomIndex];
                result[randomIndex] = temp;
            }

            return result;
        }


        #endregion

    }
}