using GameFramePro.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimpleScrollRect : MonoBehaviour
{
    public int mItemCount = 50;
    public int mItemOffset = 0;

    public SimpleLoopScrollRect mSimpleLoopScrollRect;
    // Start is called before the first frame update
    void Start()
    {
        mSimpleLoopScrollRect.mOnItemShowEvent += (rectTrans, index) =>
        {
            TestSimpleItem testSimpleItem = rectTrans.GetAddComponent<TestSimpleItem>();
            testSimpleItem.Initialed(index);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mSimpleLoopScrollRect.RefillData(mItemCount, mItemOffset);
        }
    }
}
