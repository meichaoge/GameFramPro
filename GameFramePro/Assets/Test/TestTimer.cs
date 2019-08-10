using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

public class TestTimer : MonoBehaviour
{
    private void Start()
    {
   //     TimeTickUtility.S_Instance.RegisterTimer(1, Ticker1);
     //   TimeTickUtility.S_Instance.RegisterCountDownTimer(1, 60, Ticker2);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            TimeTickUtility.S_Instance.RegisterTimer(1, Ticker1);

        if(Input.GetKeyDown(KeyCode.B))
            TimeTickUtility.S_Instance.RegisterCountDownTimer(1, 60, Ticker2);
    }

    private void Ticker1(float time, int hashcode)
    {
        Debug.Log($"顺时针  hashcode={hashcode}  time={time}");
    }

    private void Ticker2(float time, int hashcode)
    {
        Debug.Log($"逆时针  hashcode={hashcode}  time={time}");
    }
}
