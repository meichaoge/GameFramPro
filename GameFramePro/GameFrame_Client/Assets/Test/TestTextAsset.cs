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
            LoadAssetResult<TextAsset> assetResult = ResourcesManager.LoadAssetSync<TextAsset>(mPath);
            if (assetResult == null || assetResult.IsLoadAssetEnable == false)
            {
                mTextAsset = string.Empty;
                return;
            }
            assetResult.ReferenceWithComponent(null, textAsset => mTextAsset = textAsset == null ? string.Empty : textAsset.text);


            Debug.Log("加载配置文件成功");
        }
    }
}
