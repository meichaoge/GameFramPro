using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//运动模糊
public class MotionBlurEffect : BasePostProcessEffect
{
    [SerializeField]
    [Range(0,0.9f)]
    private float mMotionBlur = 0.5f;

    private RenderTexture mMotionRenderTexture;

    private void OnDisable()
    {
        DestroyImmediate(mMotionRenderTexture);
    }


    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            if(mMotionRenderTexture==null|| mMotionRenderTexture.width!=Screen.width|| mMotionRenderTexture.height != Screen.height)
            {
                if(mMotionRenderTexture!=null)
                    DestroyImmediate(mMotionRenderTexture);
                mMotionRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);
                Graphics.Blit(src, mMotionRenderTexture);
            }
            mTargetMaterial.SetFloat("_MotionAmount", mMotionBlur);
            Graphics.Blit(src, mMotionRenderTexture, mTargetMaterial);
            Graphics.Blit(mMotionRenderTexture, dest);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
