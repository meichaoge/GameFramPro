using UnityEngine;
 
public class AddCameraFilter : MonoBehaviour {
 
    private CameraFilterPack_Atmosphere_Snow_8bits _cameraFilterPack;
    // Use this for initialization
    void Start()
    {
        _cameraFilterPack = GetComponent<CameraFilterPack_Atmosphere_Snow_8bits>();
    }
 
    // Update is called once per frame
    void OnGUI()
    {
 
        if (GUI.Button(new Rect(50, 200, 200, 30), "添加雪花效果"))
        {
            if (_cameraFilterPack == null)
                _cameraFilterPack = gameObject.AddComponent<CameraFilterPack_Atmosphere_Snow_8bits>();
            if (_cameraFilterPack != null)
                _cameraFilterPack.enabled = true;
        }
        if (GUI.Button(new Rect(50, 250, 200, 30), "移除雪花效果"))
        {
            if (_cameraFilterPack != null)
                _cameraFilterPack.enabled = false;
        }
    }
} 