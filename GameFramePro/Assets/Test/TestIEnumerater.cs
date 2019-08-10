using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

public class TestIEnumerater : MonoBehaviour
{
    private void Start()
    {
        ///StartCoroutine(Test004());
        //StartCoroutine(TestWaitDone());

//        var yy = StartCoroutine(OutCoroutine());
//        AsyncManager.Invoke(10, () =>
//        {
//            Debug.Log("stop");
//            StopCoroutine(yy);
//        });


        //AsyncManager.StartCoroutineEx(TestLoop());
//
        var xx = AsyncManager.InvokeRepeating(3, 1f, DoRepeat);

        AsyncManager.Invoke(10, () =>
        {
            xx.StopCoroutine();
            Debug.Log("xx" + xx);
        });
        return;
        CoroutineEx text001 = new CoroutineEx(Test003());
        text001.StartCoroutine();
        text001.OnCompleteCoroutineExEvent += OnComplete;
    }


    private IEnumerator OutCoroutine()
    {
        int data = 0;
        yield return StartCoroutine(InnerCoroutine());
        while (true)
        {
            ++data;
            Debug.Log("OutCoroutine " + data);
            yield return AsyncManager.WaitFor_OneSecond;
        }
    }


    private IEnumerator InnerCoroutine()
    {
        int data = 0;
        while (true)
        {
            ++data;
            Debug.Log("InnerCoroutine " + data);
            yield return AsyncManager.WaitFor_OneSecond;
        }
    }


    IEnumerator Test004()
    {
        while (true)
        {
            yield return AsyncManager.WaitFor_Null;
            Debug.Log(Time.frameCount);
        }
    }

    private void DoRepeat()
    {
        Debug.LogError("111" + Time.realtimeSinceStartup);
    }


    void OnComplete(CoroutineEx ss)
    {
        Debug.LogError("完成协程");
    }


    private IEnumerator Test001()
    {
        yield return StartCoroutine(Test002());
        Debug.Log("Test001");
    }


    private IEnumerator Test002()
    {
        Debug.Log("Test002");
        yield return new WaitForSeconds(2f);
        Debug.Log("完成协程Te002");
        yield break;
    }


    private IEnumerator Test003()
    {
        Debug.Log("Test003");
        yield return new WaitForSeconds(2f);
        Debug.Log("完成协程Te003");
    }


    private IEnumerator TestWaitDone()
    {
        CoroutineEx test = new CoroutineEx(TestLoop());
        yield return test.WaitDone(true);
        Debug.Log("完成协程 TestWaitDone");
    }

    private IEnumerator TestLoop()
    {
        for (int dIndex = 0; dIndex < 20; dIndex++)
        {
            Debug.LogError("" + dIndex);
            yield return AsyncManager.WaitFor_Null;
        }
    }
}
