using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 跑马灯项
/// </summary>
public class UILanternMessageItem : MonoBehaviour
{

    private Text mConentText;
    public RectTransform rectTransform { get { return transform as RectTransform; } }
    public UILanternController.LoopContentItemInfor mInforData { get; private set; }  //显示的消息
    public float mWidth { get { return mConentText.preferredWidth; } }
    public Tweener mContentShowTweener = null;

    private void Awake()
    {
        mConentText = GetComponent<Text>();
    }
    public void ShowContent(UILanternController.LoopContentItemInfor infor)
    {
        mInforData = infor;

        if (infor == null)
        {
            if (mContentShowTweener != null && mContentShowTweener.IsComplete() == false)
                mContentShowTweener.Complete();
            return;
        }

        mConentText.text = infor.mContentMessage;
#if UNITY_EDITOR
        gameObject.name = "Item_" + infor.mMessageID;
#endif
    }

    public void SetTweener(Tweener twee)
    {
        mContentShowTweener = twee;
    }

}
