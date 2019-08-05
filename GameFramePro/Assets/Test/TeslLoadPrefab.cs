using GameFramePro;
using GameFramePro.ResourcesEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslLoadPrefab : MonoBehaviour
{
    public string prefabPath;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResourcesManager.InstantiateGameObjectByPathSync( parent, prefabPath,false);
        }
    }
}
