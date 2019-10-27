using GameFramePro;
using GameFramePro.ResourcesEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslLoadPrefab : MonoBehaviour
{
    public string prefabPath;
    public Transform parent;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResourcesManager.InstantiateAssetSync( prefabPath,parent, false);
        }
    }
}
