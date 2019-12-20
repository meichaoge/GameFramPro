using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//运动模糊
public class MotionBlurWithDepthTexture : BasePostProcessEffect
{
    [SerializeField]
    [Range(0,0.5f)]
    private float mBlurSize = 0.5f;

    private Matrix4x4 mPreviousVPMaterial; //上一帧的视角投影矩阵

    private void OnEnable()
    {
        mPreviousVPMaterial = mTargetCamera.projectionMatrix * mTargetCamera.cameraToWorldMatrix;
    }


    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            mTargetMaterial.SetFloat("_BlurSize", mBlurSize);
            Matrix4x4 current = mTargetCamera.projectionMatrix * mTargetCamera.cameraToWorldMatrix;

            mTargetMaterial.SetMatrix("currentVPInvert", current.inverse);
            mTargetMaterial.SetMatrix("previousVP", mPreviousVPMaterial);
            Graphics.Blit(src, dest, mTargetMaterial);
            mPreviousVPMaterial = current;
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
