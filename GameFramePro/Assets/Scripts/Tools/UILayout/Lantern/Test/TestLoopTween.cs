using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoopTween : MonoBehaviour
{
    public UILanternController mUILoopContentController;


    public string message;

    void Start()
    {
        mUILoopContentController.OnCompleteTweenCallback += OnCompleteTween;
    }

    int mMessageID = 10000;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    message += mMessageID.ToString();
        //    ++mMessageID;
        //}

        if (Input.GetKeyDown(KeyCode.A))
        {
            //int data = Random.Range(1, 20000000);
            message += mMessageID.ToString();
            ++mMessageID;

            mUILoopContentController.ShowMessage(message, true);
        }
    }


    private void OnCompleteTween(UILanternMessageItem item)
    {
    }
}
