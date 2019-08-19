using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramePro;
using GameFramePro.ResourcesEx;

public class TestLoadAsset : MonoBehaviour
{
    public string assetPath;
    public Image mTarget;
    public Image mTarget2;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResourcesManager.SetImageSpriteByPathSync(mTarget, assetPath);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesManager.CloneImageSprite(mTarget, mTarget2);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            var reference = ResourcesManager.SetImageSpriteByPathSync(mTarget, assetPath, false);
            if (reference != null)
                reference.SetSprite(mTarget);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var reference = ResourcesManager.SetImageSpriteByPathSync(mTarget, assetPath, false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ResourcesManager.SetImageSpriteByPathAsync(mTarget, assetPath, null);
        }
    }
}
