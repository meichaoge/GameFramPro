using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DisallowMultipleComponent]
[AddComponentMenu("")]
public class UIObjectPoolManager : MonoBehaviour
{
    //obj pool
    private Dictionary<string, UIObjectPool> poolDict = new Dictionary<string, UIObjectPool>();

    private static UIObjectPoolManager mInstance = null;

    public static UIObjectPoolManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject GO = new GameObject("ResourceManager", typeof(UIObjectPoolManager));
                // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                // However we should keep using this in Play mode only!
                mInstance = GO.GetComponent<UIObjectPoolManager>();
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                else
                {
                    Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                }
            }

            return mInstance;
        }
    }
    public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
    {
        if (poolDict.ContainsKey(poolName))
        {
            return;
        }
        else
        {
            GameObject pb = Resources.Load<GameObject>(poolName);
            if (pb == null)
            {
                Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                return;
            }
            poolDict[poolName] = new UIObjectPool(poolName, pb, gameObject, size, type);
        }
    }

    public void InitPool(GameObject prefab, int size, PoolInflationType type = PoolInflationType.DOUBLE)
    {
        if (prefab == null)
        {
            Debug.LogError("[ResourceManager] Invalide prefab  for pooling :" + prefab);
            return;
        }

        if (poolDict.ContainsKey(prefab.name))
        {
            return;
        }
        else
        {
            poolDict[prefab.name    ] = new UIObjectPool(prefab.name, prefab, gameObject, size, type);
        }
    }


    /// <summary>
    /// Returns an available object from the pool 
    /// OR null in case the pool does not have any object available & can grow size is false.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
    {
        GameObject result = null;

        if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
        {
            InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
        }

        if (poolDict.ContainsKey(poolName))
        {
            UIObjectPool pool = poolDict[poolName];
            result = pool.NextAvailableObject(autoActive);
            //scenario when no available object is found in pool
#if UNITY_EDITOR
            if (result == null)
            {
                Debug.LogError("[ResourceManager]:No object available in " + poolName);
            }
#endif
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
        }
#endif
        return result;
    }

    public void InitPool(GameObject itemPrefab, int size)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("[ResourceManager] InitPool prefab is Null:");
            return;
        }
        if (poolDict.ContainsKey(itemPrefab.name))
            return;

        poolDict[itemPrefab.name] = new UIObjectPool(itemPrefab.name, itemPrefab, gameObject, size, PoolInflationType.INCREMENT);
    }



    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="go"></param>
    public void ReturnObjectToPool(GameObject go)
    {
        PoolObject po = go.GetComponent<PoolObject>();
        if (po == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Specified object is not a pooled instance: " + go.name);
#endif
        }
        else
        {
            UIObjectPool pool = null;
            if (poolDict.TryGetValue(po.poolName, out pool))
            {
                pool.ReturnObjectToPool(po);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("No pool available with name: " + po.poolName);
            }
#endif
        }
    }

    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="t"></param>
    public void ReturnTransformToPool(Transform t)
    {
        if (t == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
            return;
        }
        //set gameobject active flase to avoid a onEnable call when set parent
        t.gameObject.SetActive(false);
        t.SetParent(null, false);
        ReturnObjectToPool(t.gameObject);
    }
}
