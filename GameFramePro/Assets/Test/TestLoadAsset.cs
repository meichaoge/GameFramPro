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
            ResourcesManager.SetImageSpriteByPathSync(mTarget,assetPath,false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResourcesManager.CloneImageSprite(mTarget, mTarget2);
        }

      

        if (Input.GetKeyDown(KeyCode.D))
        {
            //      ResourcesManager.LoadSpriteAssetAsync(mTarget, mTarget2);
        }



        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    GameObject obj = ResourcesManager.LoadAssetSync<GameObject>(assetPath);
        //    Debug.Log(obj.GetInstanceID());
        //    var sp = obj .GetComponent<SpriteRenderer>().sprite;

        //    Debug.Log(sp.GetInstanceID());

        //    ResourcesManager.SetSprite(mTarget, sp);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    ResourcesManager.LoadAssetAsync(assetPath, (asset) =>
        //    {
        //        if (asset != null)
        //        {
        //            var sp = (asset as GameObject).GetComponent<SpriteRenderer>().sprite;
        //            ResourcesManager.SetSprite(mTarget2, sp);
        //        }
        //    });
        //}

    }

}
