using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestLinkList : MonoBehaviour
{

    public LinkedList<int> mData = new LinkedList<int>();


    private void Start()
    {
        Debug.Log(System.DateTime.UtcNow.Ticks);
        for (int dex = 0; dex < 5; dex++)
        {
            LinkedListNode<int> node = new LinkedListNode<int>(dex*3);
            mData.AddLast(node);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            LinkedListNode<int> newNode = new LinkedListNode<int>(5);

            LinkedListNode<int> last = mData.Last;

            while (true)
            {
                if (last.Previous == null)
                {
                    mData.AddBefore(mData.First, newNode);
                    break;
                }
                if(last.Previous.Value<= newNode.Value)
                {
                    mData.AddAfter(last.Previous, newNode);
                    break;
                }
                last = last.Previous;
            }
        }



        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var item in mData)
            {
                Debug.Log(item);
            }
        }
    }


}
