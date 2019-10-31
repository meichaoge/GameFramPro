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
            LoadAssetResult<Sprite> loadAssetResult = ResourcesManager.LoadAssetSync<Sprite>(assetPath);
            if(loadAssetResult==null)
            {
                mTarget.sprite = null;
                return;
            }
            loadAssetResult.ReferenceWithComponent(mTarget, (sprite) =>
            {
                ResourcesManager.ReleaseComponentReferenceAsset<Sprite>(mTarget, mTarget.sprite);
                mTarget.sprite = sprite;
            });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            mTarget.sprite = mTarget2.sprite;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
//            var reference = ResourcesManager.SetImageSpriteByPathSync(mTarget, assetPath, false);
//            if (reference != null)
//                reference.SetSprite(mTarget);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
//            var reference = ResourcesManager.SetImageSpriteByPathSync(mTarget, assetPath, false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ResourcesManager.LoadAssetAsync<Sprite>(assetPath, (spriteResult) =>
            {
                if (spriteResult == null || spriteResult.IsLoadAssetEnable == false)
                {
                    ResourcesManager.ReleaseComponentReferenceAsset<Sprite>(mTarget, mTarget.sprite);
                    mTarget.sprite = null;
                    return;
                }

                spriteResult.ReferenceWithComponent(mTarget, (sprite) =>
                {
                    ResourcesManager.ReleaseComponentReferenceAsset<Sprite>(mTarget, mTarget.sprite);
                    mTarget.sprite = sprite;
                });

            });
        }
    }
}