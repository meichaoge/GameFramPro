using GameFramePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTextAsset : MonoBehaviour
{
    public string mPath;
    public string mTextAsset;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            mTextAsset = ResourcesManager.LoadTextAssetSync(mPath);
            Debug.Log("加载配置文件成功");
        }
    }
}
