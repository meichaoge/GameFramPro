//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;


//public enum LoopScrollRectDirectionUsage
//{
//    //水平方向
//    LeftToRight,
//    RightToLeft,
//    //垂直方向
//    UpToDown = 10,
//    DownToUp,
//}

///// <summary>
///// 扩展SrollRect View
///// </summary>
//[RequireComponent(typeof(ScrollRect))]
//public class SimpleLoopScrollRect : MonoBehaviour
//{
//    /// <summary>
//    /// 循环列表项的属性
//    /// </summary>
//    protected class LoopScrollRectItemInfor
//    {
//        public RectTransform mItemRectTrans;
//        public int mItemIndex;
//    }


//    #region 对外引用的参数和可调节参数

//    [SerializeField]
//    protected GameObject mItemPrefab;
//    [SerializeField]
//    protected RectTransform mViewportRectTrans;
//    [SerializeField]
//    protected RectTransform mContentRectTrans;
//    //[SerializeField]
//    //protected RectTransform mItemParentRect;

//    [SerializeField]
//    protected Vector2 mItemSpace = new Vector2(10, 10);

//    [SerializeField]
//    protected LoopScrollRectDirectionUsage mLoopScrollRectDirection = LoopScrollRectDirectionUsage.DownToUp;

//    #endregion

//    #region 内部引用

//    protected ScrollRect mTargetScrollRect { get; set; }
//    protected Canvas mParentCanvas { get; set; }
//    #endregion

//    #region 数据
//    public int mTotalDataCount { get; protected set; } = 0;
//    protected int mOffset { get; set; } = 0;  //数据偏移
//    protected Vector2 mLastScrollRectNormalPos = Vector2.zero;

//    protected LinkedList<LoopScrollRectItemInfor> mAllShowingItems { get; set; } = new LinkedList<LoopScrollRectItemInfor>();

//    protected Vector2 mItemLayoutInitialedPos = Vector2.zero;

//    #endregion

//    #region 事件
//    public event System.Action<RectTransform> mOnItemCreateEvent = null;
//    public event System.Action<RectTransform, int> mOnItemShowEvent = null;
//    public event System.Action<RectTransform> mOnItemDeleteEvent = null;

//    /// <summary>
//    /// 第一个项的索引
//    /// </summary>
//    public int mItemStartIndex;//{ get; protected set; } = 0;
//    /// <summary>
//    /// 最后一个项的索引
//    /// </summary>
//    public int mItemEndtIndex;//{ get; protected set; } = 0;

//    #endregion

//    #region Mono

//    protected virtual void Awake()
//    {
//        mTargetScrollRect = GetComponent<ScrollRect>();
//        mParentCanvas = GetComponentInParent<Canvas>();
//        mTargetScrollRect.onValueChanged.AddListener(OnScrolleRectValueChanged);


//     //   mContentRectTrans.pivot = new Vector2(0.5f, 0.5f);

//    //   SetRectTransformProperty(mContentRectTrans);
//        SetRectTransformProperty(mItemPrefab.GetAddComponentEx<RectTransform>());
//    }

//    protected virtual void OnDestroy()
//    {
//        mTargetScrollRect.onValueChanged.RemoveAllListeners();
//    }

//    #endregion

//    #region 实现
//    /// <summary>
//    /// 对外调用的接口 创建Item 项
//    /// </summary>
//    /// <param name="dataCount"></param>
//    /// <param name="offset"></param>
//    public void RefillData(int dataCount, int offset = 0)
//    {
//        mTotalDataCount = dataCount;
//        if (offset < 0)
//            mOffset = 0;
//        if (mOffset >= mTotalDataCount - 1)
//            mOffset = mTotalDataCount - 1;

//        mOffset = offset;
//        ResetState();
//        SetContentInitialedState();

//        InitialedItemLayoutView();
//    }

//    /// <summary>
//    /// 根据选择的样式设置对应Content 和Item 预制体的RectTransform 属性
//    /// </summary>
//    /// <param name="target"></param>
//    protected virtual void SetRectTransformProperty(RectTransform target)
//    {
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                target.anchorMin = Vector2.zero;
//                target.anchorMax = new Vector2(1, 0);
//                target.pivot = new Vector2(0.5f, 0);

//                break;
//            default:
//                Debug.LogError($"没有处理的数据{mLoopScrollRectDirection}");
//                break;
//        }
//    }

//    protected void ResetState()
//    {
//        mItemStartIndex = mItemEndtIndex = -1;


//        foreach (var item in mAllShowingItems)
//            ReleaseItem(item);
//    }


//    /// <summary>
//    /// 根据不同的样式设置起始的 AnchorPosition
//    /// </summary>
//    protected virtual void SetContentInitialedState()
//    {
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                mContentRectTrans.anchoredPosition = Vector2.zero;
//                break;
//            default:
//                Debug.LogError($"SetContentInitialedState{mLoopScrollRectDirection}");
//                break;
//        }

//    }

//    /// <summary>
//    /// 根据数据源的数量创建对应的项
//    /// </summary>
//    protected virtual void InitialedItemLayoutView()
//    {
//        Vector2 lastPosition = Vector2.zero;
//        mItemStartIndex = mOffset;
//        for (int dex = mOffset; dex < mTotalDataCount; dex++)
//        {
//         //   if (CheckIfOutSideOfView()) return;
//            mItemEndtIndex = dex;
//            LoopScrollRectItemInfor item = GetItem(dex);
//            if (item == null)
//            {
//                Debug.LogError($"加载项失败{dex}");
//                continue;
//            }

//            mOnItemShowEvent?.Invoke(item.mItemRectTrans, dex);
//            lastPosition = GetItemPosition(item.mItemRectTrans, lastPosition);
//            mAllShowingItems.AddLast(item);
//        }
//    }

//    /// <summary>
//    /// 根据不同的布局方式 获取对应的 AnchorPosition
//    /// </summary>
//    /// <param name="item"></param>
//    /// <param name="lastPosition"></param>
//    /// <returns>增加一个项后的记录位置 </returns>
//    protected Vector2 GetItemPosition(RectTransform item, Vector2 lastPosition)
//    {
//        Vector2 itemSpace = Vector2.zero;
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                item.anchoredPosition = lastPosition;
//                itemSpace = new Vector2(0, item.sizeDelta.y + mItemSpace.y);
//                mContentRectTrans.sizeDelta = new Vector2(mContentRectTrans.sizeDelta.x, (lastPosition.y + itemSpace.y));
//          //      mItemParentRect.sizeDelta = new Vector2(mItemParentRect.sizeDelta.x, mContentRectTrans.sizeDelta.y);
//                break;
//            default:
//                break;
//        }

//        return lastPosition + itemSpace;
//    }

//    /// <summary>
//    /// 检测是否不再视口区域
//    /// </summary>
//    /// <returns></returns>
//    protected virtual bool CheckIfOutSideOfView()
//    {
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                return !mContentRectTrans.IsInsideVertical_Standard(mViewportRectTrans, mParentCanvas);
//            default:
//                break;
//        }

//        return false;
//    }

//    #endregion


//    #region 布局刷新
//    /// <summary>
//    /// 滑动ScrollView 时候操作
//    /// </summary>
//    protected virtual void OnScrollRectViewScroll()
//    {
//        if (CheckStartViewState())
//            AddItemAtStart();
//    }

//    protected virtual bool CheckStartViewState()
//    {
//        bool isVisible = false;

//        switch (mLoopScrollRectDirection)
//        {   
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                var firstItems = mAllShowingItems.First;
//                if (firstItems == null)
//                    return false;
//                isVisible = firstItems.Value.mItemRectTrans.IsInsideVertical_Standard(mViewportRectTrans, mParentCanvas);
//                if (isVisible==false)
//                return false;
//                if (firstItems.Value.mItemIndex >0)
//                    return true;
//                return false;
//                break;
//            default:
//                Debug.LogError($"没有处理的类型");
//                break;
//        }
//        Debug.LogError($"默认返回值");
//        return false;
//    }

//    protected virtual void AddItemAtStart()
//    {
//        var firstItems = mAllShowingItems.First;
//        if (firstItems == null)
//            return ;


//        LoopScrollRectItemInfor loopScrollRectItem = null;
//        Vector2 anchorPosOffset = Vector2.zero;
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                loopScrollRectItem = GetItem(firstItems.Value.mItemIndex + 1);
//                mOnItemShowEvent?.Invoke(loopScrollRectItem.mItemRectTrans, loopScrollRectItem.mItemIndex);

//                anchorPosOffset.y =( loopScrollRectItem.mItemRectTrans.sizeDelta.y + mItemSpace.y);
//                mContentRectTrans.sizeDelta += anchorPosOffset;
//                loopScrollRectItem.mItemRectTrans.anchoredPosition = firstItems.Value.mItemRectTrans.anchoredPosition - anchorPosOffset;
//            //    mItemParentRect.sizeDelta = new Vector2(mItemParentRect.sizeDelta.x, mContentRectTrans.sizeDelta.y);
//           //     mItemParentRect.anchoredPosition += new Vector2(0, 0.5f * anchorPosOffset.y);

//                mAllShowingItems.AddFirst(loopScrollRectItem);
//                break;
//            default:
//                break;
//        }

   
//    }


//    #endregion

//    #region Item 项的创建和销毁

//    protected virtual LoopScrollRectItemInfor GetItem(int index)
//    {
//        if (mItemPrefab == null)
//            return null;
//        GameObject go = GameObject.Instantiate(mItemPrefab, mContentRectTrans, false);
//        go.name = "item_" + index;
//        RectTransform result = go.transform as RectTransform;

//        mOnItemCreateEvent?.Invoke(result);

//        LoopScrollRectItemInfor loopScrollRectItem = new LoopScrollRectItemInfor();
//        loopScrollRectItem.mItemRectTrans = result;
//        loopScrollRectItem.mItemIndex = index;

//        return loopScrollRectItem;
//    }

//    protected virtual void ReleaseItem(LoopScrollRectItemInfor item)
//    {
//        if (item == null || item.mItemRectTrans == null)
//            return;
//        mOnItemDeleteEvent?.Invoke(item.mItemRectTrans);
//        GameObject.Destroy(item.mItemRectTrans);
//    }

//    #endregion

//    protected virtual void OnScrolleRectValueChanged(Vector2 value)
//    {

//        return;
//        //   Debug.Log($"OnScrolleRectValueChanged={value}");
//        switch (mLoopScrollRectDirection)
//        {
//            case LoopScrollRectDirectionUsage.LeftToRight:
//                break;
//            case LoopScrollRectDirectionUsage.RightToLeft:
//                break;
//            case LoopScrollRectDirectionUsage.UpToDown:
//                break;
//            case LoopScrollRectDirectionUsage.DownToUp:
//                if (Mathf.Approximately(mLastScrollRectNormalPos.y, value.y) == false)
//                {
//                    mLastScrollRectNormalPos.y = value.y;
//                    OnScrollRectViewScroll();
//                }
//                break;
//            default:
//                break;
//        }

//    }

//}
