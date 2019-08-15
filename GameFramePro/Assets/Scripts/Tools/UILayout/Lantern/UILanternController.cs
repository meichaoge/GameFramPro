using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>/// 用于水平方向上滚动视图 (走马灯)/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UILanternController : MonoBehaviour
{
    /// <summary>
    /// 显示的内容项
    /// </summary>
    [System.Serializable]
    public class LoopContentItemInfor
    {
        public int mMessageID;
        public string mContentMessage;
        public bool mIsEnableAfterShow = true; //有些消息只需要显示一次 可以使用这个

        public LoopContentItemInfor(string mesage, int messageID, bool isEnableAfterShow)
        {
            mContentMessage = mesage;
            mMessageID = messageID;
            mIsEnableAfterShow = isEnableAfterShow;
        }

        public LoopContentItemInfor(LoopContentItemInfor infor, int messageID, bool isEnableAfterShow)
        {
            mMessageID = messageID;
            mContentMessage = infor.mContentMessage;
            mIsEnableAfterShow = isEnableAfterShow;
        }
    }

    #region UI

    public GameObject mContentPrefab; //负责显示内容
    public RectTransform mViewMaskRectTrans;
    protected Canvas mCanvas;
    protected Rect mViewMaskRect;

    #endregion

    #region 参数

    protected int MessageID = 1;
    [Range(0f, 20f)] [SerializeField] protected float mLoopSpaceTime = 1.5f; //间隔时间
    [Range(50, 1000)] [SerializeField] protected float mBaseTweenDistance = 100f; //基准移动距离
    [Range(0.5f, 20f)] [SerializeField] protected float mBaseTweenTime = 1f; //基准移动距离时间
    [Range(1f, 200f)] [SerializeField] protected float mContentItemSpace = 20f; //两个项之间的间距

    #endregion

    #region 回调事件

    public System.Action<UILanternMessageItem> OnCompleteTweenCallback = null; //完成一次Tween动画回调 

    #endregion

    #region Data

    [SerializeField] protected List<LoopContentItemInfor> mAllContentInfors = new List<LoopContentItemInfor>(); // 所有接口发送过来的数据 (可能会插队)

    [SerializeField] protected Queue<UILanternMessageItem> mAllShowingContentMessageItem = new Queue<UILanternMessageItem>();

    protected List<LoopContentItemInfor> mAllRecycleMessage = new List<LoopContentItemInfor>(); //隐藏时候缓存消息

    protected Coroutine mLoopCoroutine = null;
    protected LoopContentItemInfor mWillShowContent = null; //将要显示的项
    protected UILanternMessageItem mLastShowContenItem = null; //最后显示的项 可能为null


    protected int mMaxShowingItemCount = 3; //每次最多同时显示3条消息

    #endregion

    #region Unity Mono

    protected virtual void Awake()
    {
        if (mContentPrefab == null)
        {
            Debug.LogError("UILanternController Fail!!  mContentPrefab is Null");
        }

        mCanvas = GetComponentInParent<Canvas>();
        mViewMaskRect = mViewMaskRectTrans.GetCanvasRect(mCanvas);
        UIObjectPoolManager.Instance.InitPool(mContentPrefab, mMaxShowingItemCount + 1);
    }

    protected virtual void OnEnable()
    {
        mLoopCoroutine = StartCoroutine(LoopShowContent());
    }

    protected virtual void OnDisable()
    {
        //   RececlyMessageItems();
        StopAllCoroutines();
    }

    protected virtual void RececlyMessageItems()
    {
        while (mAllShowingContentMessageItem.Count != 0)
        {
            UILanternMessageItem item = mAllShowingContentMessageItem.Dequeue();
            if (item != null)
            {
                Rect itemRect = item.rectTransform.GetCanvasRect(mCanvas);
                if (itemRect.x + itemRect.width > mViewMaskRect.x + mViewMaskRect.width)
                {
                    //Debug.Log("需要重新播放 " + item.mInforData.mContentMessage);
                    mAllRecycleMessage.Add(item.mInforData);
                } //说明消息没哟显示完则缓存然后插入队列中

                DeleteMeessageItem(item, false);

                if (OnCompleteTweenCallback != null)
                    OnCompleteTweenCallback(item);
            }
        }

        if (mAllRecycleMessage.Count != 0)
        {
            mAllContentInfors.InsertRange(0, mAllRecycleMessage);
            mAllRecycleMessage.Clear();
        }

        if (mViewMaskRectTrans.childCount > 0)
        {
            int count = mViewMaskRectTrans.childCount;
            for (int dex = 0; dex < count; dex++)
            {
                var target = mViewMaskRectTrans.GetChild(0).GetComponent<UILanternMessageItem>();

                DeleteMeessageItem(target, false);
            }
        } //确保数据都被删除了

        mLastShowContenItem = null;
        mAllShowingContentMessageItem.Clear();
    }

    //#if UNITY_EDITOR
    //    protected List<LoopContentItemInfor> mShowingContentItemInfors = new List<LoopContentItemInfor>();//正在显示的项
    //    protected void Update()
    //    {
    //        mShowingContentItemInfors.Clear();
    //        foreach (var item in mAllShowingContentMessageItem)
    //        {
    //            mShowingContentItemInfors.Add(item.mInforData);
    //        }
    //    }
    //#endif

    #endregion

    #region 创建视图

    protected virtual IEnumerator LoopShowContent()
    {
        while (true)
        {
            DoLoopShowContent();

            yield return LoopWait();
        }
    }

    protected virtual IEnumerator LoopWait()
    {
        if (mLoopSpaceTime != 0F)
            yield return new WaitForSeconds(mLoopSpaceTime);
        else
        {
            yield return null;
            //yield return new WaitForSeconds(1);
        }
    }

    protected virtual bool DoLoopShowContent()
    {
        if (mViewMaskRectTrans.childCount >= mMaxShowingItemCount)
            return false;

        mWillShowContent = GetNextShowContentItem();
        if (mWillShowContent == null)
            return false;
        mAllContentInfors.RemoveAt(0);

        UILanternMessageItem tempLastMessageItem = mLastShowContenItem; //使用临时变量避免修改原有的引用
        //  EditorPauseApplication.Pause();
        if (tempLastMessageItem == null && mViewMaskRectTrans.childCount > 0)
            tempLastMessageItem = mViewMaskRectTrans.GetChild(mViewMaskRectTrans.childCount - 1).GetComponent<UILanternMessageItem>();

        float endAnchorPos = 0, tweenTime = 0;
        UILanternMessageItem viewItem = GetMeessageItem();
        viewItem.ShowContent(mWillShowContent);

        if (tempLastMessageItem != null)
        {
            Rect LastShowTargetRect = tempLastMessageItem.rectTransform.GetCanvasRect(mCanvas);

            if (LastShowTargetRect.x + LastShowTargetRect.width >= mViewMaskRect.x + mViewMaskRect.width - mContentItemSpace) //这里-mContentItemSpace 为了确保两个项之间的距离最小是mContentItemSpace
            {
                //    Debug.Log("上一个记录很长 超过Mask区域 则下一个消息接在这个消息后面 " + mLastShowContenItem.rectTransform.anchoredPosition.x);
                viewItem.rectTransform.anchoredPosition = new Vector2(tempLastMessageItem.rectTransform.anchoredPosition.x + tempLastMessageItem.rectTransform.rect.width + mContentItemSpace, 0);
            } //上一个记录很长 超过Mask区域 则下一个消息接在这个消息后面
            else
            {
                viewItem.rectTransform.anchoredPosition = new Vector2(mViewMaskRectTrans.rect.width, 0);
            } //在显示区域的右边开始
        }
        else
        {
            viewItem.rectTransform.anchoredPosition = new Vector2(mViewMaskRectTrans.rect.width, 0);
        }

        endAnchorPos = -1 * viewItem.mWidth - mContentItemSpace; //防止边界问题 每个消息多位移一点点
        mLastShowContenItem = viewItem;


        tweenTime = mBaseTweenTime * Mathf.Abs(endAnchorPos - viewItem.rectTransform.anchoredPosition.x) / mBaseTweenDistance;
        Tweener tween = viewItem.rectTransform.DOAnchorPosX(endAnchorPos, tweenTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            Rect viewTargetRect = viewItem.rectTransform.GetCanvasRect(mCanvas);
            Rect mViewMaskRect = mViewMaskRectTrans.GetCanvasRect(mCanvas);
            if (mViewMaskRect.Overlaps(viewTargetRect) == false)
            {
                //  Debug.Log("OnCompleteLastTween Recycle " + viewItem.gameObject.name);
                if (mAllShowingContentMessageItem.Count > 0)
                {
                    if (mAllShowingContentMessageItem.Peek() != viewItem)
                    {
                        Debug.Log("保存的数据记录与存在的不对应  "); //当调用了ClearAllContentItem 接口后，由于没有删除已经存在的消息 这里可能不一致
                    }
                    else
                    {
                        mAllShowingContentMessageItem.Dequeue();
                    }
                }

                DeleteMeessageItem(viewItem, true);
            }

            if (OnCompleteTweenCallback != null)
                OnCompleteTweenCallback(viewItem);
        });

        viewItem.SetTweener(tween);

        mAllShowingContentMessageItem.Enqueue(viewItem);

        return true;
    }

    //protected virtual void OnCompleteTween()
    //{
    //    if (mAllShowingContentMessageItem.Count == 0)
    //        return;
    //    var viewItem = mAllShowingContentMessageItem.Dequeue();

    //    Rect viewTargetRect = viewItem.rectTransform.GetCanvasRect(mCanvas);
    //    Rect mViewMaskRect = mViewMaskRectTrans.GetCanvasRect(mCanvas);
    //    if (mViewMaskRect.Overlaps(viewTargetRect) == false)
    //    {
    //        //  Debug.Log("OnCompleteLastTween Recycle " + viewItem.gameObject.name);
    //        DeleteMeessageItem(viewItem, true);
    //    }

    //    if (OnCompleteTweenCallback != null)
    //        OnCompleteTweenCallback(viewItem);
    //}

    #endregion


    #region 辅助接口

    protected virtual void DeleteMeessageItem(UILanternMessageItem item, bool isAutoCompleteOrKill)
    {
        if (item.mContentShowTweener != null && item.mContentShowTweener.IsComplete() == false)
        {
            if (isAutoCompleteOrKill)
                item.mContentShowTweener.Complete();
            else
                item.mContentShowTweener.Kill();
            item.mContentShowTweener = null;
        }

        if (mLastShowContenItem == item)
            mLastShowContenItem = null;
        UIObjectPoolManager.Instance.ReturnObjectToPool(item.gameObject);
    }

    protected virtual UILanternMessageItem GetMeessageItem()
    {
        GameObject go = UIObjectPoolManager.Instance.GetObjectFromPool(mContentPrefab.name);
        go.transform.SetParent(mViewMaskRectTrans, false);
        UILanternMessageItem viewItem = go.GetAddComponentEx<UILanternMessageItem>();

        viewItem.rectTransform.pivot = viewItem.rectTransform.anchorMax = viewItem.rectTransform.anchorMin = new Vector2(0, 0.5f);

        return viewItem;
    }

    /// <summary>
    /// 获取下一个需要显示的内容
    /// </summary>
    /// <returns></returns>
    protected virtual LoopContentItemInfor GetNextShowContentItem()
    {
        if (mAllContentInfors.Count == 0)
            return null;

        return mAllContentInfors[0];
    }

    ///// <summary>
    ///// 获取上一个显示的内容
    ///// </summary>
    ///// <param name="current"></param>
    ///// <returns></returns>
    //private LoopContentItemInfor GetPreviousShowContentItem(LoopContentItemInfor current)
    //{
    //    if (mAllContentInfors.Count == 0)
    //        return null;

    //    for (int dex = 0; dex < mAllContentInfors.Count; dex++)
    //    {
    //        if (mAllContentInfors[dex].mMessageID == current.mMessageID)
    //        {
    //            if (dex != 0)
    //                return mAllContentInfors[dex - 1];
    //            return null;
    //        }
    //    }
    //    return null;
    //}

    //private LoopContentItemInfor GetFirstShowContentItem(ref Queue<LoopContentItemInfor> dataSorces)
    //{
    //    if (dataSorces == null || dataSorces.Count == 0)
    //        return null;
    //    return dataSorces.Dequeue();
    //}

    #endregion

    #region 对外接口

    /// <summary>
    /// 添加一个要显示的内容
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="isEnableAfterShow">是否只显示一次</param>
    public virtual void ShowMessage(string msg, bool isEnableAfterShow)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        ++MessageID;
        LoopContentItemInfor itemInfor = new LoopContentItemInfor(msg, MessageID, isEnableAfterShow);
        mAllContentInfors.Add(itemInfor);
    }

    public virtual void ShowMessage(LoopContentItemInfor itemInfor, bool isEnableAfterShow)
    {
        if (itemInfor == null)
            return;
        ++MessageID;

        LoopContentItemInfor infor = new LoopContentItemInfor(itemInfor, MessageID, isEnableAfterShow);
        mAllContentInfors.Add(infor);
    }

    /// <summary>
    /// 删除所有的数据
    /// </summary>
    /// <param name="isDeleteShowingItems">=true 标示立刻删除所有的项，=false 则标示显示完之后再删除</param>
    public virtual void ClearAllContentItem(bool isDeleteShowingItems)
    {
        RececlyMessageItems();
        mAllContentInfors.Clear();
        mAllShowingContentMessageItem.Clear();
        mAllRecycleMessage.Clear();
        mWillShowContent = null;
        mLastShowContenItem = null;
    }

    ///// <summary>
    ///// 删除第一个已经显示的数据
    ///// </summary>
    //public void RemoveFirstShowedMessage()
    //{
    //    if (mAllContentInfors.Count == 0)
    //        return;
    //    Debug.Log("Remvoe Message  " + mAllContentInfors[0].mMessageID);
    //    mAllContentInfors.RemoveAt(0);
    //}

    //public LoopContentItemInfor GetMessageContentInforByIndex(int index)
    //{
    //    if (index >= mAllContentInfors.Count)
    //    {
    //        Debug.LogError("获取失败 index= " + index);
    //        return null;
    //    }
    //    return mAllContentInfors[index];
    //}

    #endregion
}
