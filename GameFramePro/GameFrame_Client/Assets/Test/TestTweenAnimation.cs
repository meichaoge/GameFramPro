using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class TestTweenAnimation : MonoBehaviour
{
    public Transform mTargetFrom;

    public Transform mTargetTo;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mTargetFrom.DOMove(mTargetTo.position, 5);
        }
    }
}
