using GameFramePro.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test001 : MonoBehaviour
{
    public UIVerticalLoopScrollView mUIVerticalLoopScrollView;
    public int count = 50;
    public int mOffset = 0;
    public float yOffset = 0.1f;

    public RectTransform mTarget;
    public Vector2 mOfsset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mUIVerticalLoopScrollView.RefillCells(count, mOffset, yOffset);
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var item in mTarget.GetLocalCorners_Standard(Vector2.zero))
            {
                Debug.Log(item);
            }
            Debug.Log("----");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var item in mTarget.GetLocalCorners_Standard(mOfsset))
            {
                Debug.Log(item);
            }
            Debug.Log("----");

        
        }
    }
}
