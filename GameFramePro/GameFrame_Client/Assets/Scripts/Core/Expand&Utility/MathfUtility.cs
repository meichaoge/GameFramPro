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


        #region 贝塞尔曲线
        public static Vector3 Bezier(float t, List<Vector3> points)
        {
            if (points == null || points.Count == 0)
                return Vector3.zero;
            if (points.Count == 1)
                return points[0];

            List<Vector3> temp = new List<Vector3>(points.Count - 1);
            for (int dex = 0; dex < points.Count - 1; dex++)
            {
                Vector3 lerpPoint = (1 - t) * points[dex] + t * points[dex + 1];
                temp.Add(lerpPoint);
            }
            return Bezier(t, temp);
        }

        public static Vector3 Bezier(float t, List<Transform> transPoints)
        {
            if (transPoints == null || transPoints.Count == 0)
                return Vector3.zero;
            if (transPoints.Count == 1)
                return transPoints[0].position;

            List<Vector3> temp = new List<Vector3>(transPoints.Count - 1);
            for (int dex = 0; dex < transPoints.Count - 1; dex++)
            {
                Vector3 lerpPoint = (1 - t) * transPoints[dex].position + t * transPoints[dex + 1].position;
                temp.Add(lerpPoint);
            }
            return Bezier(t, temp);
        }
        /// <summary>
        /// 三个点之间的线性插值
        /// </summary>
        /// <param name="t"></param>
        /// <param name="po"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3 Bezier(float t, Vector3 po, Vector3 p1, Vector3 p2)
        {
            Vector3 temp1 = (1 - t) * po + t * p1;
            Vector3 temp2 = (1 - t) * p1 + t * p2;
            return (1 - t) * temp1 + t * temp2;
        }

        /// <summary>
        /// 取两个向量 start,end 的中间向量的一半，然后缩放halfDistanceScale 作为中间点向量，然后进行二阶插值
        /// </summary>
        /// <param name="t"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="halfDistanceScale">中间向量缩放背书</param>
        /// <returns></returns>
        public static Vector3 Bezier(float t, Vector3 start, Vector3 end, float halfDistanceScale)
        {
            Vector3 middlePoint = 0.5f * halfDistanceScale * (start + end);

            Vector3 temp1 = (1 - t) * start + t * middlePoint;
            Vector3 temp2 = (1 - t) * middlePoint + t * end;
            return (1 - t) * temp1 + t * temp2;
        }
        #endregion


        #region 螺旋线
        /// <summary>
        /// 对数螺旋线 (  r = a * e.power(b * θ))
        /// </summary>
        /// <param name="t">插值数据[0,1]</param>
        /// <param name="center">中心点坐标</param>
        /// <param name="a">参数用来修改影响 范围</param>
        /// <param name="b">参数用来控制角度变化快慢</param>
        /// <param name="turns">转过的圈数</param>
        /// <returns></returns>
        public static Vector3 LogarithmicSpiral(float t, Vector3 center, float a = 0.3f, float b = 0.25f, float turns = 2.5f, bool isClockWise = false)
        {
            float angle = 2 * Mathf.PI * turns * t;  //旋转的角度
            float radiu = a * Mathf.Exp(angle * b);

            Vector3 offset;
            if (isClockWise)
                offset = new Vector3(radiu * Mathf.Cos(angle), 0, radiu * Mathf.Sin(angle));
            else
                offset = new Vector3(radiu * Mathf.Cos(angle), 0, -1 * radiu * Mathf.Sin(angle));

            offset += center;
            return offset;
        }

        /// <summary>
        /// 阿基米德螺旋线 r=a+b* θ
        /// </summary>
        /// <param name="t"></param>
        /// <param name="center"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="turns"></param>
        /// <returns></returns>
        public static Vector3 ArchimedesSpiral(float t, Vector3 center, float a = 0.3f, float b = 0.25f, float turns = 2.5f, bool isClockWise = false)
        {
            float angle = 2 * Mathf.PI * turns * t;  //旋转的角度
            float radiu = a + (angle * b);

            Vector3 offset;
            if (isClockWise)
                offset = new Vector3(radiu * Mathf.Cos(angle), 0, -1 * radiu * Mathf.Sin(angle));
            else
                offset = new Vector3(radiu * Mathf.Cos(angle), 0, radiu * Mathf.Sin(angle));
            offset += center;
            return offset;
        }


        /// <summary>
        /// 逆向推导的对数螺旋线 指定了旋转路径的起点、最长的半径长度、旋转圈数
        /// </summary>
        /// <param name="t"></param>
        /// <param name="endPoint">结束点位置</param>
        /// <param name="distance">旋转结束时候终点到起点的距离</param>
        /// <param name="turns">转动的圈数</param>
        /// <param name="offsetRadiu">螺旋线偏移旋转角度</param>
        /// <param name="isClockWise">是否是顺时针旋转</param>
        /// <returns></returns>
        public static Vector2 OppositeLogarithmicSpiral(float t, Vector2 endPoint, float distance = 10, float turns = 1f, float offsetRadiu = 0, bool isClockWise = true)
        {
            float θ = 2 * Mathf.PI * turns * t; //转过的角度
            float radiu = distance * (Mathf.Exp(t * turns) - 1) / (Mathf.Exp(turns) - 1);  //distance * Mathf.Exp(θ - 2 * Mathf.PI * turns);      // 

            float β = 0;
            Vector2 position;
            if (isClockWise)
            {
                β = offsetRadiu - 2 * Mathf.PI * turns;
                float δ = θ + β;
                position = new Vector2(Mathf.Cos(δ), Mathf.Sin(δ)) * radiu;
            }
            else
            {
                β = 2 * Mathf.PI - offsetRadiu - 2 * Mathf.PI * turns;
                float δ = θ + β;
                position = new Vector2(Mathf.Cos(δ), Mathf.Sin(Mathf.PI + δ)) * radiu;
            }

            position += endPoint;
            return position;
        }

        /// <summary>
        /// 逆向推导的对数螺旋线 指定了旋转路径的起点、终点、旋转圈数
        /// </summary>
        /// <param name="t"></param>
        /// <param name="endPoint"> 结束点位置</param>
        /// <param name="startPoint">起点位置</param>
        /// <param name="turns">转动的圈数</param>
        /// <param name="isClockWise">是否是顺时针旋转</param>
        /// <returns></returns>
        public static Vector2 OppositeLogarithmicSpiral(float t, Vector2 endPoint, Vector2 startPoint, float turns = 1f, bool isClockWise = true)
        {
            float distance = (startPoint - endPoint).magnitude;

            Vector2 relativePos = new Vector2(startPoint.x - endPoint.x, startPoint.y - endPoint.y).normalized;
            float radiu = Mathf.Acos(Vector2.Dot(relativePos, Vector2.right));

            float θ = Mathf.Rad2Deg * radiu;

            if (startPoint.y < endPoint.y)
                radiu = Mathf.PI * 2 - radiu;

            return OppositeLogarithmicSpiral(t, endPoint, distance, turns, radiu, isClockWise);
        }

        #endregion

    }
}