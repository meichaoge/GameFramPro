using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using GameFramePro;

public class TestCoroutinueAsync : MonoBehaviour
{
    public bool isMain;

    public void Start()
    {
        Debug.Log("Start::   " + Thread.CurrentThread.IsBackground);
//        CoroutinueAsync CoroutinueAsync001 = new CoroutinueAsync(TestCoroutinueAsync001(), isMain);
//           StartCoroutine(CoroutinueAsync001);

        SuperCoroutine CoroutinueAsync002 = new SuperCoroutine(TesSuperCoroutine005(), isMain);
        StartCoroutine(CoroutinueAsync002);
    }

    private IEnumerator TestCoroutinueAsync001()
    {
        yield return 1;
        Debug.Log("TestCoroutinueAsync001...... " + 1 + "   " + Thread.CurrentThread.IsBackground);
        yield return new WaitForSeconds(5);

        HardWork();
        yield return AsyncManager.JumpToBackground;

        Debug.Log("TestCoroutinueAsync001...... " + 2 + "   " + Thread.CurrentThread.IsBackground);
        yield return 2;
        yield return new WaitForSeconds(5);
        HardWork();
        yield return AsyncManager.JumpToUnity;
        Debug.Log("TestCoroutinueAsync001...... " + 3 + "   " + Thread.CurrentThread.IsBackground);
        yield return new WaitForSeconds(10);

        yield return 3;
    }


    private IEnumerator TesSuperCoroutine005()
    {
        Debug.Log($"Time.Frame={Time.frameCount}");
        var inner = new SuperCoroutine(TesSuperCoroutine006());
        AsyncManager.Invoke(1, () => { inner.StopCoroutine(); });

        yield return inner.WaitDone();
        Debug.Log("TesSuperCoroutine005");
    }


    private IEnumerator TesSuperCoroutine006()
    {
        Debug.Log("TesSuperCoroutine006");
        yield return new WaitForSeconds(3);
        Debug.Log("TesSuperCoroutine006 Complete");
    }

    private IEnumerator TesSuperCoroutine002()
    {
        yield return 1;
        Debug.Log("TesSuperCoroutine002...... " + 1 + "   " + Thread.CurrentThread.IsBackground);
        yield return new WaitForSeconds(5);

        HardWork();
        yield return AsyncManager.JumpToBackground;

        Debug.Log("TesSuperCoroutine002...... " + 2 + "   " + Thread.CurrentThread.IsBackground);
        yield return 2;
        yield return new WaitForSeconds(5);
        HardWork();
        yield return AsyncManager.JumpToUnity;
        Debug.Log("TesSuperCoroutine002...... " + 3 + "   " + Thread.CurrentThread.IsBackground);
        yield return new WaitForSeconds(10);

        yield return 3;
    }

    private IEnumerator TesSuperCoroutine004()
    {
        Debug.Log($" TesSuperCoroutine004 " + Thread.CurrentThread.IsBackground);
        yield return AsyncManager.JumpToBackground;
        Thread.Sleep(5000);

        Debug.Log("跳转子线程等待5秒" + Thread.CurrentThread.IsBackground);
        yield return null;

        yield return AsyncManager.JumpToUnity;
        yield return null;

        Debug.Log("跳转主线程等待5秒" + Thread.CurrentThread.IsBackground);
        Thread.Sleep(5000);

        yield return new WaitForSeconds(5);
    }

    private IEnumerator TesSuperCoroutine003()
    {
        Debug.Log($"xxx Frame={Time.frameCount} ");
        yield return 1;
        Debug.Log($" Frame={Time.frameCount} 1");
        yield return null;

        Debug.Log($"xxx Frame={Time.frameCount} ");
        yield return 2;
        Debug.Log($" Frame={Time.frameCount} 2");
        yield return null;

        Debug.Log($"xxx Frame={Time.frameCount} ");
        yield return 3;
        Debug.Log($" Frame={Time.frameCount} 3");
        yield return null;

        Debug.Log($"xxx Frame={Time.frameCount} ");
        yield return 4;
        Debug.Log($" Frame={Time.frameCount} 4");
        yield return null;
    }

    private void HardWork()
    {
        double sum = 1d;
        for (int dIndex = 0; dIndex < 10000000; dIndex++)
        {
            sum += dIndex / sum * dIndex * dIndex * dIndex;
        }
    }
}
