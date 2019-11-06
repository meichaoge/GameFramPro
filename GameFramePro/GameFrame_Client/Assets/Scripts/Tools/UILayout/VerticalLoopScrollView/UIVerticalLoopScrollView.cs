using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameFramePro.UI
{
    /// <summary>
    /// 解决Unity 自带的Vertical LayoutGroup 是从上到下的布局方式。改成从下往上的布局方式，
    /// TODO 当开始的元素不足以填充的时候滑动有问题
    /// </summary>
    public class UIVerticalLoopScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] protected GameObject mPrefabSources = null;
        [SerializeField] protected RectTransform mViewPort = null;
        [SerializeField] protected RectTransform mContent = null;

        [SerializeField]
        protected Vector2 mItemStartPositon = Vector2.zero;
        [SerializeField]
        protected Vector2 mItemSpace = Vector2.zero;

        [SerializeField]
        [Range(0.1f,2f)]
        protected float mSmoothDeapTime = 1f;

        /// <summary>
        ///  循环列表项
        /// </summary>
        public class UIVerticalLayoutItemInfor
        {
            public RectTransform rectTransform { get; set; }
            public int mItemIndex { get; set; }  //索引ID 

            #region 构造函数
            static UIVerticalLayoutItemInfor() {
                s_UIVerticalLayoutItemInforPoolMgr = new NativeObjectPool<UIVerticalLayoutItemInfor>(10, OnBeforeGeUIVerticalLayoutItemInfor, OnBeforeRecycleUIVerticalLayoutItemInfor);
            }
            public UIVerticalLayoutItemInfor() { }
            private UIVerticalLayoutItemInfor(RectTransform rectTrans, int index)
            {
                rectTransform = rectTrans;
                mItemIndex = index;
            }
            #endregion

            #region 对象池

            private static NativeObjectPool<UIVerticalLayoutItemInfor> s_UIVerticalLayoutItemInforPoolMgr;

            private static void OnBeforeGeUIVerticalLayoutItemInfor(UIVerticalLayoutItemInfor record)
            {
            }

            private static void OnBeforeRecycleUIVerticalLayoutItemInfor(UIVerticalLayoutItemInfor record)
            {
                if (record== null) return;
                record. rectTransform = null;
                record.mItemIndex = -1;
            }

            /// <summary>
            /// 获取 UIVerticalLayoutItemInfor 实例对象
            /// </summary>
            /// <returns></returns>
            public static UIVerticalLayoutItemInfor GetUIVerticalLayoutItemInfor()
            {
                return s_UIVerticalLayoutItemInforPoolMgr.GetItemFromPool();
            }

            /// <summary>
            /// 获取 UIVerticalLayoutItemInfor 实例对象
            /// </summary>
            public static UIVerticalLayoutItemInfor GetUIVerticalLayoutItemInfor(RectTransform rectTrans,int index)
            {
                var uIVerticalLayoutItemInfor = s_UIVerticalLayoutItemInforPoolMgr.GetItemFromPool();
                uIVerticalLayoutItemInfor.rectTransform = rectTrans;
                uIVerticalLayoutItemInfor.mItemIndex = index;


                return uIVerticalLayoutItemInfor;
            }

            /// <summary>
            /// 释放 UIVerticalLayoutItemInfor 对象
            /// </summary>
            public static void ReleaseUIVerticalLayoutItemInfor(UIVerticalLayoutItemInfor uIVerticalLayoutItemInfor)
            {
                s_UIVerticalLayoutItemInforPoolMgr.RecycleItemToPool(uIVerticalLayoutItemInfor);
            }

            #endregion

        }


        #region 数据

        protected int mItemCount { get; set; } = 0;
        protected int mItemOffset { get; set; } = 0;

        protected Vector2 mItemScrollOffset = Vector2.zero;
        protected Rect mViewPortRelativeRect { get; set; }

        protected LinkedList<UIVerticalLayoutItemInfor> mAllShowingItems = new LinkedList<UIVerticalLayoutItemInfor>(); //所有显示的元素

        #endregion

        protected Canvas mCanvas { get; set; }
        public event System.Action<RectTransform, int> mOnItemCreateEvent = null;
        public event System.Action<RectTransform, int> mOnItemRemoveEvent = null;


        #region Mono 
        private void Awake()
        {
            mCanvas = mViewPort.GetComponentInParent<Canvas>();
            mViewPortRelativeRect = mViewPort.GetCanvasRect_Standard(mCanvas);

            mUIVerticalLoopScrollViewUnityMonoPool = new UnityGameObjectPool(System.Guid.NewGuid().ToString());
            mUIVerticalLoopScrollViewUnityMonoPool.InitialedPool(30, mPrefabSources, null, null);
        }


        void Update()
        {
            if (mIsDraging) return;
            if (Mathf.Approximately(mSpeed, 0)) return;

            float TargetOffset = mSpeed * Time.deltaTime;
            float current = mContent.anchoredPosition.y;
            float destination = Mathf.SmoothDamp(current, current + TargetOffset, ref mSpeed, mSmoothDeapTime, Mathf.Infinity, Time.unscaledDeltaTime);

            Vector2 offsetxy = new Vector2(0, OnScrollView(destination - current));

            if (Mathf.Abs(offsetxy.y) <= 0.1f)
            {
                mSpeed = 0;
                return;
            }

            mContent.anchoredPosition += offsetxy;
        }
        #endregion

        #region 对象池
        //关卡星级
        private UnityGameObjectPool mUIVerticalLoopScrollViewUnityMonoPool = null;

       
        public UIVerticalLayoutItemInfor GetItemFromPool(int index)
        {
            var obj = mUIVerticalLoopScrollViewUnityMonoPool.GetItemFromPool();
            obj.transform.SetParent(mContent, false);
            obj.transform.localScale = Vector3.one;

            UIVerticalLayoutItemInfor uIVerticalLayoutItem = UIVerticalLayoutItemInfor.GetUIVerticalLayoutItemInfor(obj.transform.transform as RectTransform,index);
            mOnItemCreateEvent?.Invoke(uIVerticalLayoutItem.rectTransform, index);
            return uIVerticalLayoutItem;
        }

        public void DeleteLoopScollItem(UIVerticalLayoutItemInfor uIVerticalLayoutItem)
        {
            if (uIVerticalLayoutItem == null) return;
            mOnItemRemoveEvent?.Invoke(uIVerticalLayoutItem.rectTransform, uIVerticalLayoutItem.mItemIndex);
            mUIVerticalLoopScrollViewUnityMonoPool.RecycleItemToPool(uIVerticalLayoutItem.rectTransform.gameObject);
            UIVerticalLayoutItemInfor.ReleaseUIVerticalLayoutItemInfor(uIVerticalLayoutItem);
        }

        #endregion

        #region 对外接口

        /// <summary>
        /// 填充元素 
        /// </summary>
        /// <param name="count">总个个数</param>
        /// <param name="offset">元素偏移</param>
        /// <param name="yOffset">显示这个元素的Y坐标偏移</param>
        public void RefillCells(int count, int offset, float yOffset)
        {
            mItemCount = count;
            mItemOffset = offset;

            InitialedState();
            SetItemLayout();
            StartCoroutine(DoOffsetMove(yOffset));
        }

        #endregion


        #region 内部实现

        /// <summary>
        /// 初始化或者回复状态
        /// </summary>
        protected void InitialedState()
        {
            foreach (var item in mAllShowingItems)
                DeleteLoopScollItem(item);
            mAllShowingItems.Clear();
        }

        protected void SetItemLayout()
        {
            float lastPosY = 0;
            mItemScrollOffset = Vector2.zero;
            for (int dex = mItemOffset; dex < mItemCount; dex++)
            {
                UIVerticalLayoutItemInfor layoutItem = GetItemFromPool(dex);

                if (dex == mItemOffset)
                {
                    layoutItem.rectTransform.anchoredPosition = mItemStartPositon;
                    lastPosY = layoutItem.rectTransform.sizeDelta.y + mItemStartPositon.y;
                }
                else
                {
                    layoutItem.rectTransform.anchoredPosition = new Vector2(0, lastPosY + mItemSpace.y);
                    lastPosY += layoutItem.rectTransform.sizeDelta.y + mItemSpace.y;
                }
                mAllShowingItems.AddLast(layoutItem);
                if (layoutItem.rectTransform.IsIntersect(mItemScrollOffset, mViewPort, mCanvas) == false)
                    return;
            }

        }


        #endregion


        #region 辅助

        private float OnScrollView(float offset)
        {
            if (Mathf.Approximately(offset, 0f))
                return 0f;


            bool isBottomView = offset > 0f;
            if (isBottomView)
                return ScrollUp(offset);      //查看下面的内容
            return ScrollDown(offset);
        }

        /// <summary>
        /// 查看下面的内容
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private float ScrollUp(float offset)
        {
            //Debug.Log($"查看下面的内容");
            float moveOffset = 0;
            mItemScrollOffset.y = offset;

            while (mAllShowingItems.First != null)
            {
                UIVerticalLayoutItemInfor first = mAllShowingItems.First.Value;
                if (first == null)
                    break;
                if (first.mItemIndex > 0 && first.rectTransform.IsIntersect(mItemScrollOffset, mViewPort, mCanvas))
                {
                    UIVerticalLayoutItemInfor layoutItem = GetItemFromPool(first.mItemIndex - 1);
                    layoutItem.rectTransform.SetAsFirstSibling();
                    layoutItem.rectTransform.anchoredPosition = new Vector2(0, first.rectTransform.anchoredPosition.y - (mItemSpace.y + layoutItem.rectTransform.sizeDelta.y));
                    mAllShowingItems.AddFirst(layoutItem);
                }
                else
                    break;

            } //增加可见元素

            if (mAllShowingItems.First == null)
                return 0;
            Rect firstItemTransRect = mAllShowingItems.First.Value.rectTransform.GetCanvasRect_Standard(mItemScrollOffset, mCanvas);
            float firstOffset = firstItemTransRect.y - mViewPortRelativeRect.y;
            //Debug.Log($"firstOffset={firstOffset}");

            if (firstOffset >= 0)
            {
                moveOffset = offset - firstOffset; //达到边界
                                                   //   Debug.Log($"到达边界{moveOffset} ：{offset}  ：：{firstOffset}  {mAllShowingItems.First.Value.rectTransform.anchoredPosition} content={mContent.anchoredPosition}");
            }
            else
                moveOffset = Mathf.Max(firstOffset, offset);

            mItemScrollOffset.y = moveOffset;

            while (mAllShowingItems.Last != null)
            {
                UIVerticalLayoutItemInfor lastItem = mAllShowingItems.Last.Value;
                if (lastItem == null)
                    break;
                //   if (IsInsideView(lastItem.rectTransform) == false)
                //     Debug.Log($"ScrollUp moveOffset=={moveOffset}previous={lastItem.rectTransform.anchoredPosition}  content={mContent.anchoredPosition}");
                if (lastItem.rectTransform.IsIntersect(mItemScrollOffset, mViewPort, mCanvas) == false)
                {
                    mAllShowingItems.RemoveLast();
                    DeleteLoopScollItem(lastItem);
                }
                else
                    break;

            } //删除上面不可见的元素

            return moveOffset;
        }

        /// <summary>
        /// 查看上面的内容
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private float ScrollDown(float offset)
        {
            // Debug.Log($"查看上面的内容");
            float moveOffset = 0;
            mItemScrollOffset.y = offset;

            while (mAllShowingItems.Last != null)
            {
                UIVerticalLayoutItemInfor lastItem = mAllShowingItems.Last.Value;
                if (lastItem == null)
                    break;
                if (lastItem.mItemIndex < mItemCount - 1 && lastItem.rectTransform.IsIntersect(mItemScrollOffset, mViewPort, mCanvas))        //IsInsideView(lastItem.rectTransform))
                {
                    UIVerticalLayoutItemInfor layoutItem = GetItemFromPool(lastItem.mItemIndex + 1);
                    layoutItem.rectTransform.SetAsLastSibling();
                    layoutItem.rectTransform.anchoredPosition = new Vector2(0, lastItem.rectTransform.anchoredPosition.y + (mItemSpace.y + lastItem.rectTransform.sizeDelta.y));
                    mAllShowingItems.AddLast(layoutItem);
                }
                else
                    break;
            } //增加可见元素

            if (mAllShowingItems.Last == null)
                return 0;

            Rect lastItemTransRect = mAllShowingItems.Last.Value.rectTransform.GetCanvasRect_Standard(mItemScrollOffset, mCanvas);
            float lastOffset = lastItemTransRect.y + lastItemTransRect.height - (mViewPortRelativeRect.y + mViewPortRelativeRect.height);
            //Debug.Log($"lastOffset={lastOffset}");

            if (lastOffset <= 0)
            {
                //if (mAllShowingItems.Last.Value.mItemIndex == mItemCount - 1)
                //    return 0;

                moveOffset = offset - lastOffset;
                //  Debug.Log($"ScrollDown  到达边界{moveOffset} ：{offset}  ：：{lastOffset}");
                // moveOffset = (mViewPortRelativeRect.y + mViewPortRelativeRect.height) - (lastItemTransRect.y + lastItemTransRect.height);
                //  return 0; //达到边界
            }
            else
                moveOffset = Mathf.Min(lastOffset, offset);
            mItemScrollOffset.y = moveOffset;

            while (mAllShowingItems.First != null)
            {
                UIVerticalLayoutItemInfor first = mAllShowingItems.First.Value;
                if (first == null)
                    break;

                //  if (IsInsideView(first.rectTransform) == false)
                //Debug.Log($"ScrollDown moveOffset=={moveOffset} content={mContent.anchoredPosition}");
                if (first.rectTransform.IsIntersect(mItemScrollOffset, mViewPort, mCanvas) == false)
                {
                    mAllShowingItems.RemoveFirst();
                    DeleteLoopScollItem(first);
                }
                else
                    break;
            } //删除下面不可见的元素

            return moveOffset;
        }

        private IEnumerator DoOffsetMove(float offsetLayout)
        {
            yield return AsyncManager.WaitFor_Null;
            OnScrollView(offsetLayout);
        }
        #endregion


        #region 滑动拖拽事件
        protected Vector2 mLastRecordPostiion = Vector2.zero;
        protected bool mIsDraging { get; set; } = false;
        protected float mSpeed = 0; //滑动速度
        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsDraging = true;
            mLastRecordPostiion = eventData.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            float offset = (eventData.position.y - mLastRecordPostiion.y);
            if (Mathf.Abs(offset) < 3f)
                return;
            mContent.anchoredPosition += new Vector2(0, OnScrollView(offset));
            mLastRecordPostiion = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mIsDraging = false;
            float offset = (eventData.position.y - mLastRecordPostiion.y);
            mSpeed = offset / Time.unscaledDeltaTime;
        }


        #endregion


    }
}