using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using GameFramePro;

public class TestCourtinue : MonoBehaviour
{
    private Coroutine m_Coroutine;

    private void Start()
    {
        m_Coroutine = StartCoroutine(TestIEnumerator());
        Thread thread = new Thread(BeginThread);
        thread.IsBackground = true;
        thread.Start();
    }

    private SuperCoroutine mTestSuperCoroutine;

    private void Update()
    {
//        Debug.Log("Update=" + (m_Coroutine == null));
//
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            StopCoroutine(m_Coroutine);
//        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            mTestSuperCoroutine = AsyncManager.InvokeRepeating(0, 1, () => { Debug.Log($"time={Time.time}"); });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            mTestSuperCoroutine.StopCoroutine();
        }
    }

    void BeginThread(object obj)
    {
        int data = 1;
        mTestSuperCoroutine = AsyncManager.InvokeRepeating(0, 1, () =>
        {
            data++;
            Debug.Log($"time={Time.time}    {data}");
        });

        while (true)
        {
            if (data == 100)
                mTestSuperCoroutine.StopCoroutine();

            Thread.Sleep(1000);
        }
    }

    public IEnumerator TestIEnumerator()
    {
        int count = 0;
        while (true)
        {
            ++count;
            Debug.Log("count=" + count);
            if (count == 5)
            {
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }
}