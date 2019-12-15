using UnityEngine;
using System.Collections;

public class Bloom : BasePostProcessEffect
{
	// Blur iterations - larger number means more blur.
	[Range(0, 4)]
	public int iterations = 3;
	
	// Blur spread for each iteration - larger value means more blur
	[Range(0.2f, 3.0f)]
	public float blurSpread = 0.6f;

	[Range(1, 8)]
	public int downSample = 2;

	[Range(0.0f, 4.0f)]
	public float luminanceThreshold = 0.6f;

    protected override void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (mTargetMaterial != null) {
            mTargetMaterial.SetFloat("_LuminanceThreshold", luminanceThreshold);

			int rtW = src.width/downSample;
			int rtH = src.height/downSample;
			
			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;
			
			Graphics.Blit(src, buffer0, mTargetMaterial, 0);
			
			for (int i = 0; i < iterations; i++) {
                mTargetMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);
				
				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
				// Render the vertical pass
				Graphics.Blit(buffer0, buffer1, mTargetMaterial, 1);
				
				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
				// Render the horizontal pass
				Graphics.Blit(buffer0, buffer1, mTargetMaterial, 2);
				
				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
			}

            mTargetMaterial.SetTexture ("_Bloom", buffer0);  
			Graphics.Blit (src, dest, mTargetMaterial, 3);  

			RenderTexture.ReleaseTemporary(buffer0);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
