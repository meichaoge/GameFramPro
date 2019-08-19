using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestCourtinue : MonoBehaviour
{
    private Coroutine m_Coroutine;
    private void Start()
    {
        m_Coroutine = StartCoroutine(TestIEnumerator());
    }

    private void Update()
    {
        Debug.Log("Update=" + (m_Coroutine == null));

        if (Input.GetKeyDown(KeyCode.A))
        {
            StopCoroutine(m_Coroutine);
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
