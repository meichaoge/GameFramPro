using UnityEngine;
using System.Collections;

public class EdgeDetectNormalsAndDepth : BasePostProcessEffect
{

    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f;

    public Color edgeColor = Color.black;

    public Color backgroundColor = Color.white;

    public float sampleDistance = 1.0f;

    public float sensitivityDepth = 1.0f;

    public float sensitivityNormals = 1.0f;

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    [ImageEffectOpaque]
    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial != null)
        {
            mTargetMaterial.SetFloat("_EdgeOnly", edgesOnly);
            mTargetMaterial.SetColor("_EdgeColor", edgeColor);
            mTargetMaterial.SetColor("_BackgroundColor", backgroundColor);
            mTargetMaterial.SetFloat("_SampleDistance", sampleDistance);
            mTargetMaterial.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));

            Graphics.Blit(src, dest, mTargetMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
