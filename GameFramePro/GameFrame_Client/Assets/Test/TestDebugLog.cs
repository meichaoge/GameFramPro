using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;
using GameFramePro.AnalysisEx;

public class TestDebugLog : MonoBehaviour
{
    private void Start()
    {
//         string trackStr = new System.Diagnostics.StackTrace().ToString();
//         Debug.Log ("Stack Info:" + trackStr);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            test002();
        }
    }

    void test002()
    {
        test003();
    }

    void test003()
    {
        Debug.Log("Update  " + DebugTese001());
        Debug.Log("test003 " + DebugUtility.GetCurStackTraceInfor());
    }

    private string DebugTese001()
    {
        StackTrace stackTrace = new System.Diagnostics.StackTrace(true);

        StringBuilder builder = new StringBuilder();
        StackFrame[] frames = stackTrace.GetFrames();
        foreach (var frame in frames)
        {
            builder.Append(frame.GetFileName());
            builder.Append("\t");
            builder.Append(frame.GetMethod().Name);
            builder.Append("\t");
            builder.Append(frame.GetFileLineNumber());
            builder.Append(System.Environment.NewLine);
        }

        return builder.ToString();
    }
}