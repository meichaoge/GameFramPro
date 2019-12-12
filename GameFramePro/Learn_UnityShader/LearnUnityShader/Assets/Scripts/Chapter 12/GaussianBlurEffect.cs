using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//高斯模糊
public class GaussianBlurEffect : BasePostProcessEffect
{
    [SerializeField]
    [Range(1, 5)]
    private int LoopTime = 1;  //递归次数
    [SerializeField]
    [Range(1, 5)]
    private int EffectRange = 1; //采样周围元素的半径

    [SerializeField]
    [Range(1, 4)]
    private int DownSample = 0;//降低采样的幂次方



    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            int width = (int)(Screen.width / (Mathf.Pow(2, DownSample)));
            int height = (int)(Screen.height / (Mathf.Pow(2, DownSample)));

            RenderTexture temp0= RenderTexture.GetTemporary(width, height,0);
            temp0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, temp0);  //拷贝到临时


            for (int dex = 0; dex < LoopTime; dex++)
            {
                mTargetMaterial.SetFloat("_BlurSize", 1.0f+EffectRange* dex);
                RenderTexture temp1 = RenderTexture.GetTemporary(width, height,0);

                Graphics.Blit(temp0, temp1, mTargetMaterial,0);  //水平方向处理
                RenderTexture.ReleaseTemporary(temp0);
                temp0 = temp1;
                temp1 = RenderTexture.GetTemporary(width, height, 0);

                Graphics.Blit(temp0, temp1, mTargetMaterial, 1);  //垂直方向上处理
                RenderTexture.ReleaseTemporary(temp0);
                temp0 = temp1;
            }

            Graphics.Blit(temp0, dest);
            RenderTexture.ReleaseTemporary(temp0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }



    }
}
