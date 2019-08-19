using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestRool : MonoBehaviour
{
    public Transform target;

    public void Start()
    {
    }


    private void Update()
    {
        target.Rotate(0, 5, 0);
    }
}
