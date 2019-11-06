using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>/// 物理计算时候的扩展方法/// </summary>
    public static class Physic_MathExpand
    {
        #region 相交计算

        ///判断依据： 两个矩形相交的条件:两个矩形的中心距离在X和Y轴上都小于两个矩形长以及宽的一半
        //两个矩形相交判断
        public static bool IsIntersect_Rect2Rect(this Rect rect1, Rect rect2)
        {
            Vector2 distance = rect1.center - rect2.center;
            float halfSumX = 0.5F * (rect1.width + rect2.width);
            float halfSumY = 0.5F * (rect1.height + rect2.height);

            return Mathf.Abs(distance.x) <= halfSumX && Mathf.Abs(distance.y) <= halfSumY;
        }

        //点是否在矩形内 必须是同一个坐标系下坐标
        public static bool IsIntersect_Rect2Point(this Rect rect1, Vector2 position)
        {
            bool isInsideX = position.x >= rect1.x && position.x <= (rect1.x + rect1.width);
            bool isInsideY = position.y >= rect1.y && position.y <= (rect1.y + rect1.height);

            return isInsideX && isInsideY;
        }

        #endregion

        #region 相对大小
        /// <summary>/// 两个矩形相加/// </summary>
        public static Rect RelativeRect(this Rect a, Rect b)
        {
            return new Rect(a.x - b.x, a.y - b.y, a.width, a.height);
        }
        #endregion
    }
}