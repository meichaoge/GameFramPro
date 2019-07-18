using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

public class TestPath :MonoBehaviour
{
    public string path1;
    public string path2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(path1.ComparePathEx(path2));
        }
    }

}
