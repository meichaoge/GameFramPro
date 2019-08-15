using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>/// RectTransform 扩展类 (有些方法可以使用RectTransformUtility)/// </summary>
public static class RectTransform_Ex
{
    public static T GetAddComponent<T>(this RectTransform target) where T : Component
    {
        if (target == null)
        {
            Debug.LogError("GetAddComponent  Fail,Target RectTransform Is Null");
            return null;
        }

        T result = target.GetComponent<T>();
        if (result == null)
            result = target.gameObject.AddComponent<T>();
        return result;
    }

    /// <summary>
    /// 重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    public static void ResetRectTransProperty(this RectTransform target)
    {
        target.ResetRectTransProperty(Vector2.zero, target.sizeDelta);
    }

    /// <summary>
    ///  重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="anchorPos"></param>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos)
    {
        target.ResetRectTransProperty(anchorPos, target.sizeDelta);
    }

    /// <summary>
    ///  重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="anchorPos"></param>
    /// <param name="size"></param>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos, Vector2 size)
    {
        target.anchoredPosition = anchorPos;
        target.sizeDelta = size;
    }

    /// <summary>
    ///Rectransform 扩展方法 ，返回RectTransform 子节点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static RectTransform GetChildEX(this RectTransform target, int index)
    {
        if (target == null)
        {
            Debug.LogError("target is Null");
            return null;
        }

        return target.transform.GetChild(index) as RectTransform;
    }

    #region 下面四个方法需要验证是否可靠 (TODO)

    ///// <summary>
    ///// 水平方向上 target 是否在trans 里面
    ///// </summary>
    ///// <param name="trans"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //public static bool IsInsideRect_Horizontial(this RectTransform trans, RectTransform target)
    //{
    //    Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
    //    if (relativePos.x == 0)
    //        return true;

    //    bool IsoutsideLeft = false;
    //    bool IsoutsideRight = false;

    //    if (relativePos.x > 0)
    //    {
    //        IsoutsideRight = relativePos.x - target.rect.width / 2f >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
    //    }
    //    else
    //    {
    //        IsoutsideRight = relativePos.x + target.rect.width / 2f <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
    //    }

    //    return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    //}

    ///// <summary>
    ///// 垂直方向上 target 是否在trans 里面
    ///// </summary>
    ///// <param name="trans"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //public static bool IsInsideRect_Vertical(this RectTransform trans, RectTransform target)
    //{
    //    Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
    //    if (relativePos.y == 0)
    //        return true;

    //    bool IsoutsideTop = false;
    //    bool IsoutsideBottom = false;


    //    if (relativePos.y > 0)
    //    {
    //        IsoutsideTop = relativePos.y - target.rect.height / 2f >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
    //    }
    //    if (relativePos.y < 0)
    //    {
    //        IsoutsideBottom = relativePos.y + target.rect.height / 2f <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
    //    }

    //    return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    //}


    ///// <summary>
    ///// 垂直方向上 世界坐标点 是否在trans 里面
    ///// </summary>
    ///// <param name="trans"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //public static bool IsInsideRect_Vertical(this RectTransform trans, Vector3 wordPosition)
    //{
    //    Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
    //    if (relativePos.y == 0)
    //        return true;

    //    bool IsoutsideTop = false;
    //    bool IsoutsideBottom = false;


    //    if (relativePos.y > 0)
    //    {
    //        IsoutsideTop = relativePos.y >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
    //    }
    //    if (relativePos.y < 0)
    //    {
    //        IsoutsideBottom = relativePos.y <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
    //    }

    //    return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    //}

    ///// <summary>
    ///// 水平方向上 世界坐标点 wordPosition 是否在trans 里面
    ///// </summary>
    ///// <param name="trans"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //public static bool IsInsideRect_Horizontial(this RectTransform trans, Vector3 wordPosition)
    //{
    //    Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
    //    if (relativePos.x == 0)
    //        return true;

    //    bool IsoutsideLeft = false;
    //    bool IsoutsideRight = false;

    //    if (relativePos.x > 0)
    //    {
    //        IsoutsideRight = relativePos.x >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
    //    }
    //    else
    //    {
    //        IsoutsideLeft = relativePos.x <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
    //    }

    //    return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    //}

    #endregion

    #region 判断两个 RectTransform 是否有交集 （TODO 可以使用 GetRelativeRect 接口扩展）

    /// <summary>
    /// 水平方向上 target 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Horizontial(this RectTransform trans, RectTransform target)
    {
        Vector2 relativePos = trans.InverseTransformPoint(target.position); //target相对于trans坐标
        if (relativePos.x == 0)
            return true;

        bool IsoutsideLeft = false;
        bool IsoutsideRight = false;

        if (relativePos.x > 0)
        {
            IsoutsideRight = relativePos.x - target.rect.width / 2f >= trans.rect.width / 2f; //target左边界是否超出trans右边界
        }
        else
        {
            IsoutsideRight = relativePos.x + target.rect.width / 2f <= -1 * trans.rect.width / 2f; //target右边界是否超出trans左边界
        }

        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    }

    /// <summary>
    /// 垂直方向上 target 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Vertical(this RectTransform trans, RectTransform target)
    {
        Vector2 relativePos = trans.InverseTransformPoint(target.position); //target相对于trans坐标
        if (relativePos.y == 0)
            return true;

        bool IsoutsideTop = false;
        bool IsoutsideBottom = false;


        if (relativePos.y > 0)
        {
            IsoutsideTop = relativePos.y - target.rect.height / 2f >= trans.rect.height / 2f; //target下边界是否超出trans上边界
        }

        if (relativePos.y < 0)
        {
            IsoutsideBottom = relativePos.y + target.rect.height / 2f <= -1 * trans.rect.height / 2f; //target上边界是否超出trans下边界
        }

        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    }


    /// <summary>
    /// 垂直方向上 世界左边点 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Vertical(this RectTransform trans, Vector3 wordPosition)
    {
        Vector2 relativePos = trans.InverseTransformPoint(wordPosition); //target相对于trans坐标
        if (relativePos.y == 0)
            return true;

        bool IsoutsideTop = false;
        bool IsoutsideBottom = false;


        if (relativePos.y > 0)
        {
            IsoutsideTop = relativePos.y >= trans.rect.height / 2f; //target下边界是否超出trans上边界
        }

        if (relativePos.y < 0)
        {
            IsoutsideBottom = relativePos.y <= -1 * trans.rect.height / 2f; //target上边界是否超出trans下边界
        }

        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    }

    /// <summary>
    /// 水平方向上 世界坐标点 wordPosition 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Horizontial(this RectTransform trans, Vector3 wordPosition)
    {
        Vector2 relativePos = trans.InverseTransformPoint(wordPosition); //target相对于trans坐标
        if (relativePos.x == 0)
            return true;

        bool IsoutsideLeft = false;
        bool IsoutsideRight = false;

        if (relativePos.x > 0)
        {
            IsoutsideRight = relativePos.x >= trans.rect.width / 2f; //target左边界是否超出trans右边界
        }
        else
        {
            IsoutsideLeft = relativePos.x <= -1 * trans.rect.width / 2f; //target右边界是否超出trans左边界
        }

        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    }

    #endregion


    static readonly Vector3[] m_WorldCorners = new Vector3[4];
    static readonly Vector3[] m_CanvasCorners = new Vector3[4];

    /// <summary>/// 获取参数t 相对于画布的的坐标/// </summary>
    public static Rect GetCanvasRect(this RectTransform trans, Canvas canvas)
    {
        if (canvas == null)
            return new Rect();

        trans.GetWorldCorners(m_WorldCorners);
        for (int i = 0; i < 4; ++i)
            m_CanvasCorners[i] = canvas.transform.InverseTransformPoint(m_WorldCorners[i]);

        return new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
    }

    /// <summary>/// 获取相对于relativeTrans 的坐标范围/// </summary>
    public static Rect GetRelativeRect(this RectTransform trans, RectTransform relativeTrans, Canvas canvas)
    {
        Rect transRect = GetCanvasRect(trans, canvas);
        Rect relativeTransRect = GetCanvasRect(relativeTrans, canvas);

        return transRect.MinusRect(relativeTransRect);
    }
}
