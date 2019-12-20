using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基于深度的雾效
public class BaseOnDepthFog : BasePostProcessEffect
{
    public Color FogColor = Color.white;

    [Range(0,10)]
    public float FogStart = 0.1f;
    [Range(0, 10)]
    public float FogEnd = 2f;
    [Range(0, 10)]
    public float FogDensity = 3f;

    private void OnEnable()
    {
        mTargetCamera.depthTextureMode |= DepthTextureMode.Depth;

    }

    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mTargetMaterial)
        {
            float fov = mTargetCamera.fieldOfView;
            float near = mTargetCamera.nearClipPlane;
            float aspect = mTargetCamera.aspect;

            float halfHeight = near * Mathf.Tan((fov * Mathf.Deg2Rad * 0.5f));
            Vector3 nearPlaneUp = mTargetCamera.transform.up * halfHeight;
            Vector3 nearPlaneRight = mTargetCamera.transform.right * aspect * halfHeight;



            Vector3 LeftTop = mTargetCamera.transform.forward* near + nearPlaneUp - nearPlaneRight;
            float scale = LeftTop.magnitude / near;
            LeftTop.Normalize();
            LeftTop = LeftTop* scale;

            Vector3 RightTop = mTargetCamera.transform.forward * near + nearPlaneUp + nearPlaneRight;
            RightTop.Normalize();
            RightTop = RightTop * scale;

            Vector3 RightBottom = mTargetCamera.transform.forward * near - nearPlaneUp +nearPlaneRight;
            RightBottom.Normalize();
            RightBottom = RightBottom * scale;

            Vector3 LeftBottom = mTargetCamera.transform.forward * near - nearPlaneUp - nearPlaneRight;
            LeftBottom.Normalize();
            LeftBottom = LeftBottom * scale;

            Matrix4x4 matrix4 =  Matrix4x4.identity;
            matrix4.SetRow(0, LeftBottom);
            matrix4.SetRow(1, RightBottom);
            matrix4.SetRow(2, RightTop);
            matrix4.SetRow(3, LeftTop);


            mTargetMaterial.SetColor("_FogColor", FogColor);
            mTargetMaterial.SetFloat("_FogStart", FogStart);
            mTargetMaterial.SetFloat("_FogEnd", FogEnd);
            mTargetMaterial.SetFloat("_FogDensity", FogDensity);

            mTargetMaterial.SetMatrix("_FrustumCornersRay", matrix4);

            Graphics.Blit(src, dest, mTargetMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }

    }
}
