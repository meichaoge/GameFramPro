using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//边缘检测
public class EdgeDetect : BasePostProcessEffect
{
    [SerializeField]
    [Range(0.1f, 1)]
    private float mEdgeWidth = 1; //边缘宽度

    [SerializeField]
    private Color mEdgeColor = Color.black; //边缘颜色

    [SerializeField]
    private Color mBackgroundColor = Color.black; //背景色
    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            mTargetMaterial.SetFloat("_EdgeOnly", mEdgeWidth);
            mTargetMaterial.SetColor("_EdgeColor", mEdgeColor);
            mTargetMaterial.SetColor("Background", mBackgroundColor);

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
