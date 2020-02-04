using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class TestCamera : MonoBehaviour
{
    public Button mReplaceButton;
    public Button mNoReplaceButton;
    public Camera mTargetCamera;

    public Shader mReplaceShader;

    // Start is called before the first frame update
    void Start()
    {
        if (mTargetCamera == null)
            mTargetCamera = GetComponent<Camera>();
        mReplaceButton.onClick.AddListener(OnReplaceButtonClick);
        mNoReplaceButton.onClick.AddListener(OnNoReplaceButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnReplaceButtonClick()
    {
        //mTargetCamera.SetReplacementShader(mReplaceShader, "");
        mTargetCamera.SetReplacementShader(mReplaceShader, "MyTag");
        mTargetCamera.SetReplacementShader(mReplaceShader, "RenderType");

    }

    private void OnNoReplaceButtonClick()
    {
        mTargetCamera.ResetReplacementShader();
    }

}
