using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Bloom (亮色扩散)
public class BloomEffect : BasePostProcessEffect
{
    [SerializeField]
    [Range(0,1f)]
    private float luminanceTheshold = 0.6f;

    [SerializeField]
    [Range(1, 5)]
    private int LoopTime = 2;  //递归次数
    [SerializeField]
    [Range(1, 5)]
    private float blurSpread = 1; //采样周围元素的半径

    [SerializeField]
    [Range(1, 4)]
    private int DownSample = 0;//降低采样的幂次方

    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            int width = (int)(Screen.width / Mathf.Pow(2, DownSample));
            int height = (int)(Screen.height / Mathf.Pow(2, DownSample));

            RenderTexture temp0= RenderTexture.GetTemporary(width, height, 0);
            temp0.filterMode = FilterMode.Bilinear;

            mTargetMaterial.SetFloat("_LuminanceThreshold", luminanceTheshold);
            Graphics.Blit(src, temp0, mTargetMaterial, 0);

            for (int dex = 0; dex < LoopTime; dex++)
            {
                mTargetMaterial.SetFloat("_BlurSize", 1f + dex * blurSpread);

                RenderTexture temp1 = RenderTexture.GetTemporary(width, height, 0);
                Graphics.Blit(temp0, temp1, mTargetMaterial, 1);

                RenderTexture.ReleaseTemporary(temp0);
                temp0 = temp1;
                temp1 = RenderTexture.GetTemporary(width, height, 0);

                Graphics.Blit(temp0, temp1, mTargetMaterial, 2);

                RenderTexture.ReleaseTemporary(temp0);
                temp0 = temp1;
            }

            mTargetMaterial.SetTexture("_Boom", temp0);

            Graphics.Blit(src, dest, mTargetMaterial,3);
            RenderTexture.ReleaseTemporary(temp0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }

    }
}
