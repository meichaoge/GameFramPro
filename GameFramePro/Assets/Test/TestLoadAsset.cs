using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramePro;

public class TestLoadAsset : MonoBehaviour
{
    public string assetPath;
    public Image mTarget;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UnityEngine.Object obj = ResourcesManager.LoadAssetSync(assetPath);
            mTarget.sprite = (obj as GameObject).GetComponent<SpriteRenderer>().sprite;
        }
    }

}
