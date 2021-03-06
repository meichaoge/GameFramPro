﻿using System.Collections;
using System.Collections.Generic;
using GameFramePro;
using UnityEngine;


/// <summary>/// RectTransform 扩展类 (有些方法可以使用RectTransformUtility)/// </summary>
public static class RectTransform_Ex
{
    #region 获取组件/子节点

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

    /// <summary>///Rectransform 扩展方法 ，返回RectTransform 子节点/// </summary>
    public static RectTransform GetChildEX(this RectTransform target, int index)
    {
        if (target == null)
        {
            Debug.LogError("target is Null");
            return null;
        }

        return target.transform.GetChild(index) as RectTransform;
    }

    /// <summary>/// 获取所在的Cancas 对象/// </summary>
    public static Canvas GetParentCanvas(this RectTransform target)
    {
        return target.GetComponentInParent<Canvas>();
    }

    #endregion

    #region RectTransform  重置  位置属性

    /// <summary>/// 重置 RectTransform 位置属性/// </summary>
    public static void ResetRectTransProperty(this RectTransform target)
    {
        target.ResetRectTransProperty(Vector2.zero, target.sizeDelta);
    }

    /// <summary>/// 重置 RectTransform 位置属性/// </summary>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos)
    {
        target.ResetRectTransProperty(anchorPos, target.sizeDelta);
    }

    /// <summary>/// 重置 RectTransform 位置属性/// </summary>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos, Vector2 size)
    {
        target.anchoredPosition = anchorPos;
        target.sizeDelta = size;
    }

    #endregion


    #region UI 坐标轴平行的优化
    /// <summary>
    /// 获取参数t 相对于画布的的坐标
    /// </summary>
    public static Rect GetCanvasRect_Standard(this RectTransform trans, Canvas canvas)
    {
        return GetCanvasRect_Standard(trans, Vector2.zero, canvas);
        //if (canvas == null)
        //    return new Rect();
        //Vector3[] m_WorldCorners = new Vector3[4];
        //Vector3[] m_CanvasCorners = new Vector3[4];

        //trans.GetWorldCorners(m_WorldCorners);
        ////        for (int i = 0; i < 4; ++i)
        ////            m_CanvasCorners[i] = canvas.transform.InverseTransformPoint(m_WorldCorners[i]);

        ////2019/9/20 优化
        //m_CanvasCorners[0] = canvas.transform.InverseTransformPoint(m_WorldCorners[0]); //左下角
        //m_CanvasCorners[2] = canvas.transform.InverseTransformPoint(m_WorldCorners[2]); //右上角

        //return new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
    }

    public static Rect GetCanvasRect_Standard(this RectTransform trans, Vector2 offset, Canvas canvas)
    {
        if (canvas == null)
            return new Rect();

        Vector3[] m_WorldCorners = new Vector3[4];
        Vector3[] m_CanvasCorners = new Vector3[4];

        //***    trans.GetWorldCorners(m_WorldCorners); 内部实现
        Rect rect = trans.rect;
        rect.center += offset;
        float x = rect.x;
        float y = rect.y;
        float xMax = rect.xMax;
        float yMax = rect.yMax;
        m_WorldCorners[0] = new Vector3(x, y, 0.0f);
        m_WorldCorners[1] = new Vector3(x, yMax, 0.0f);
        m_WorldCorners[2] = new Vector3(xMax, yMax, 0.0f);
        m_WorldCorners[3] = new Vector3(xMax, y, 0.0f);


        Matrix4x4 localToWorldMatrix = trans.localToWorldMatrix;
        //for (int index = 0; index < 4; ++index)
        //    m_WorldCorners[index] = localToWorldMatrix.MultiplyPoint(m_WorldCorners[index]);

        // trans.GetWorldCorners(m_WorldCorners);
        //        for (int i = 0; i < 4; ++i)
        //            m_CanvasCorners[i] = canvas.transform.InverseTransformPoint(m_WorldCorners[i]);

        //2019/9/20 优化
        m_CanvasCorners[0] = canvas.transform.InverseTransformPoint(localToWorldMatrix.MultiplyPoint(m_WorldCorners[0])); //左下角
        m_CanvasCorners[2] = canvas.transform.InverseTransformPoint(localToWorldMatrix.MultiplyPoint(m_WorldCorners[2])); //右上角


        return new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
    }

    public static Vector3[] GetLocalCorners_Standard(this RectTransform trans, Vector2 offset)
    {
        Vector3[] m_WorldCorners = new Vector3[4];
        Rect rect = trans.rect;
        rect.center += offset;
        float x = rect.x;
        float y = rect.y;
        float xMax = rect.xMax;
        float yMax = rect.yMax;
        m_WorldCorners[0] = new Vector3(x, y, 0.0f);
        m_WorldCorners[1] = new Vector3(x, yMax, 0.0f);
        m_WorldCorners[2] = new Vector3(xMax, yMax, 0.0f);
        m_WorldCorners[3] = new Vector3(xMax, y, 0.0f);

        return m_WorldCorners;
    }

    /// <summary>/// 获取相对于relativeTrans 的坐标范围/// </summary>
    public static Rect GetRelativeRect_Standard(this RectTransform trans, RectTransform relativeTrans, Canvas canvas)
    {
        Rect transRect = GetCanvasRect_Standard(trans, canvas);
        Rect relativeTransRect = GetCanvasRect_Standard(relativeTrans, canvas);

        return transRect.RelativeRect(relativeTransRect);
    }


    public static bool IsInsideVertical_Standard(this RectTransform trans, RectTransform relativeTrans, Canvas canvas)
    {
        Rect transRect = GetCanvasRect_Standard(trans, canvas);
        Rect relativeTransRect = GetCanvasRect_Standard(relativeTrans, canvas);
        return IsInsideVertical_Standard(transRect, relativeTransRect);

    }
    /// <summary>
    /// relativeTransRect1 是否垂直方向 上在  relativeTransRect2 中
    /// </summary>
    /// <param name="relativeTransRect1"></param>
    /// <param name="relativeTransRect2"></param>
    /// <returns></returns>
    public static bool IsInsideVertical_Standard(Rect relativeTransRect1, Rect relativeTransRect2)
    {
        bool isHeight = Mathf.Approximately(relativeTransRect1.height, relativeTransRect2.height) || relativeTransRect1.height <= relativeTransRect2.height;
        bool isDownPoint = Mathf.Approximately(relativeTransRect1.y, relativeTransRect2.y) || Mathf.Approximately(relativeTransRect1.y, relativeTransRect2.y + relativeTransRect2.height) || 
            (relativeTransRect1.y >= relativeTransRect2.y && relativeTransRect1.y <= relativeTransRect2.y + relativeTransRect2.height);
        bool isUpPoint = Mathf.Approximately(relativeTransRect1.y + relativeTransRect1.height, relativeTransRect2.y + relativeTransRect2.height) 
            ||  relativeTransRect1.y + relativeTransRect1.height <= relativeTransRect2.y + relativeTransRect2.height;

        return isHeight && isDownPoint && isUpPoint;
    }


    public static bool IsInsideHorizontial_Standard(this RectTransform trans, RectTransform relativeTrans, Canvas canvas)
    {
        Rect transRect = GetCanvasRect_Standard(trans, canvas);
        Rect relativeTransRect = GetCanvasRect_Standard(relativeTrans, canvas);
        return IsInsideHorizontial_Standard(transRect, relativeTransRect);

    }
    /// <summary>
    /// relativeTransRect1 是否水平方向 上在  relativeTransRect2 中
    /// </summary>
    /// <param name="relativeTransRect1"></param>
    /// <param name="relativeTransRect2"></param>
    /// <returns></returns>
    public static bool IsInsideHorizontial_Standard(Rect relativeTransRect1, Rect relativeTransRect2)
    {
        bool isWidth = Mathf.Approximately(relativeTransRect1.width, relativeTransRect2.width) || relativeTransRect1.width <= relativeTransRect2.width;
        bool isLeftPoint = Mathf.Approximately(relativeTransRect1.x, relativeTransRect2.x) || Mathf.Approximately(relativeTransRect1.x, relativeTransRect2.x + relativeTransRect2.width) ||
            (relativeTransRect1.x >= relativeTransRect2.x && relativeTransRect1.x <= relativeTransRect2.x + relativeTransRect2.width);
        bool isRightPoint = Mathf.Approximately(relativeTransRect1.x + relativeTransRect1.width, relativeTransRect2.x + relativeTransRect2.width) || (relativeTransRect1.x + relativeTransRect1.width <= relativeTransRect2.x + relativeTransRect2.width);

        return isWidth && isLeftPoint && isRightPoint;
    }


    public static bool IsInside_Standard(this RectTransform trans, RectTransform relativeTrans, Canvas canvas)
    {
        Rect transRect = GetCanvasRect_Standard(trans, canvas);
        Rect relativeTransRect = GetCanvasRect_Standard(relativeTrans, canvas);
        return IsInsideVertical_Standard(transRect, relativeTransRect) && IsInsideHorizontial_Standard(transRect, relativeTransRect);
    }

    #endregion







  



    public static bool IsIntersect(this RectTransform trans1, Vector2 offset, RectTransform trans2, Canvas canvas)
    {
        Rect relativeRect1 = trans1.GetCanvasRect_Standard(offset, canvas);
        Rect relativeRect2 = trans2.GetCanvasRect_Standard(canvas);

        return relativeRect1.IsIntersect_Rect2Rect(relativeRect2);
    }
    /// <summary>/// 判断两个矩形是否相交/// </summary>
    ///判断依据： 两个矩形相交的条件:两个矩形的中心距离在X和Y轴上都小于两个矩形长以及宽的一半
    public static bool IsIntersect(this RectTransform trans1, RectTransform trans2, Canvas canvas)
    {
        Rect relativeRect1 = trans1.GetCanvasRect_Standard(canvas);
        Rect relativeRect2 = trans2.GetCanvasRect_Standard(canvas);

        return relativeRect1.IsIntersect_Rect2Rect(relativeRect2);
    }
}