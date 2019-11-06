using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectTransformExpand : MonoBehaviour
{
    public Canvas mCanvas;
    public RectTransform mTarget1;
    public RectTransform mTarget2;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(mTarget1.GetCanvasRect_Standard(mCanvas));
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(mTarget2.GetRelativeRect_Standard(mTarget1, mCanvas));
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(mTarget2.IsIntersect(mTarget1, mCanvas));
        }
    }
}