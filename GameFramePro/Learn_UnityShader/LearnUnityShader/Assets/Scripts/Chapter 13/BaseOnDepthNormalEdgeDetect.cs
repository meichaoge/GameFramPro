using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 基于深度和法线纹理的边缘检测
/// </summary>
[ImageEffectOpaque]
public class BaseOnDepthNormalEdgeDetect : BasePostProcessEffect
{
    [SerializeField]
    [Range(0.1f, 1)]
    private float mEdgeWidth = 1; //边缘宽度

    [SerializeField]
    private Color mEdgeColor = Color.black; //边缘颜色

    [SerializeField]
    private Color mBackgroundColor = Color.black; //背景色

    [SerializeField]
    [Range(0.001f, 1)]
    private float DepthSensity = 0.2f;

    [SerializeField]
    [Range(0.001f, 1)]
    private float NormalSensity = 0.2f;

    [SerializeField]
    [Range(0.1f, 1)]
    private float SampleDistance = 0.2f;

    private void OnEnable()
    {
        mTargetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
    }
    //需要深度值大于一个阈值 并且法线大于阈值
    [ImageEffectOpaque]
    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            mTargetMaterial.SetFloat("_EdgeOnly", mEdgeWidth);
            mTargetMaterial.SetFloat("_SampleDistance", SampleDistance);

            mTargetMaterial.SetColor("_EdgeColor", mEdgeColor);
            mTargetMaterial.SetColor("Background", mBackgroundColor);


            mTargetMaterial.SetVector("_Sensity", new Vector4(DepthSensity, NormalSensity, 0, 0));
            //mTargetMaterial.SetFloat("_EdgeOnly", mEdgeWidth);
            //mTargetMaterial.SetColor("_EdgeColor", mEdgeColor);
            //mTargetMaterial.SetColor("_BackgroundColor", mBackgroundColor);

            Graphics.Blit(src, dest, mTargetMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

}