using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectrans : MonoBehaviour
{
    public RectTransform mView;
    public RectTransform mContent;

    Canvas mTargetCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mTargetCanvas = mView.GetComponentInParent<Canvas>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(mView.rect);
            Debug.Log(mContent.rect);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log($"Horizotntial={mContent.IsInsideHorizontial_Standard(mView, mTargetCanvas)} " +
                $" ::Vertival={mContent.IsInsideVertical_Standard(mView, mTargetCanvas)} " +
                $"::Total={mContent.IsInside_Standard(mView, mTargetCanvas)}");
        }
    }
}
