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
    public Image mTarget2;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResourcesManager.LoadSpriteAssetSync(assetPath, mTarget);
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
