using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro.FrameSync;

public class TestFix64 : MonoBehaviour
{
    public float mdata = 3.1415f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Fix64 data2 = mdata * Fix64.One;
            Debug.Log($"{data2}");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Fix64 data2 = mdata * Fix64.One;
            Debug.Log($"{(float)data2}");
        }
    }
}