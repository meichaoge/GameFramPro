using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可以调节亮度 、饱和度、 对比度、
public class BrighnessSaturationContrast : BasePostProcessEffect
{
    [SerializeField]
    [Range(0, 3f)]
    private float mBrightness = 1.0f;

    [SerializeField]
    [Range(0, 3f)]
    private float mSaturation = 1.0f;

    [SerializeField]
    [Range(0, 3f)]
    private float mContrast = 1.0f;


    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // base.OnRenderImage(src, dest);
        if (mTargetMaterial != null)
        {
            mTargetMaterial.SetFloat("_Brightness", mBrightness);
            mTargetMaterial.SetFloat("_Saturation", mSaturation);
            mTargetMaterial.SetFloat("_Contrast", mContrast);

            Graphics.Blit(src, dest, mTargetMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

}
