using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;

public class TestSerialize : MonoBehaviour
{

    public class Test0001
    {
        public Dictionary<int, string> mData = new Dictionary<int, string>();
    }

    public Test0001 mTest0001;
    public string mInfor;

    // Start is called before the first frame update
    void Start()
    {
        mTest0001 = new Test0001();
        for (int dex = 0; dex < 10; dex++)
        {
            mTest0001.mData[dex] = dex.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mInfor = SerializeManager.SerializeObject(mTest0001);
            Debug.Log(mInfor);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
          var  data2 = SerializeManager.DeserializeObject<Test0001>(mInfor);
            Debug.Log(data2.mData);
        }
    }
}
