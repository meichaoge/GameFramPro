using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

public class TestIEnumerater : MonoBehaviour
{
    private void Start()
    {
            StartCoroutine(Test004());
            return;
            ;
        
        CoroutineEx text001=new CoroutineEx(Test003());
        text001.StartCoroutine();
        text001.OnCompleteCoroutineExEvent += OnComplete;

    }

    IEnumerator Test004()
    {
        while (true)
        {
            yield return AsyncManager.WaitFor_Null;
            Debug.Log(Time.frameCount);
        }
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
    
    
    
    
    
}