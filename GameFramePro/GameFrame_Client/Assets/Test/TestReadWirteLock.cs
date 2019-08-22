using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using Random = System.Random;

public class TestReadWirteLock : MonoBehaviour
{
    private ConcurrentQueue<int> mMessageQueue = new ConcurrentQueue<int>();
    private Thread newCreateThread;
    private Thread newShowThread;

    private int data = 0;

    private void Start()
    {
        newCreateThread = new Thread(CreateMessage);
        newCreateThread.Start();

        newShowThread = new Thread(ShowMessage);
        newShowThread.Start();
    }

    private void OnDisable()
    {
        newCreateThread.Abort();
        newShowThread.Abort();
    }

    private void Update()
    {
        if (mMessageQueue.Count > 0)
        {
            int result;
            mMessageQueue.TryDequeue(result: out result);
            Debug.Log($"--{result} ");
        }
    }

    private void CreateMessage(object obj)
    {
        while (true)
        {
            Random random = new Random();


            mMessageQueue.Enqueue(data);
            ++data;
            Thread.Sleep(random.Next(1, 100));
        }
    }

    private void ShowMessage(object obj)
    {
        while (true)
        {
            Random random = new Random();

            if (mMessageQueue.IsEmpty == false)
            {
                mMessageQueue.TryDequeue(result: out var result);
                Debug.Log($"--{result} ");
            }

            Thread.Sleep(random.Next(1, 100));
        }
    }
}