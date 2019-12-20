using UnityEngine;
using System.Collections;

public class FogWithDepthTexture : BasePostProcessEffect
{

	[Range(0.0f, 3.0f)]
	public float fogDensity = 1.0f;

	public Color fogColor = Color.white;

	public float fogStart = 0.0f;
	public float fogEnd = 2.0f;

	void OnEnable() {
        mTargetCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

    protected override void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (mTargetMaterial != null) {
			Matrix4x4 frustumCorners = Matrix4x4.identity;

			float fov = mTargetCamera.fieldOfView;
			float near = mTargetCamera.nearClipPlane;
			float aspect = mTargetCamera.aspect;

			float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
			Vector3 toRight = mTargetCamera.transform.right * halfHeight * aspect;
			Vector3 toTop = mTargetCamera.transform.up * halfHeight;

			Vector3 topLeft = mTargetCamera.transform.forward * near + toTop - toRight;
			float scale = topLeft.magnitude / near;

			topLeft.Normalize();
			topLeft *= scale;

			Vector3 topRight = mTargetCamera.transform.forward * near + toRight + toTop;
			topRight.Normalize();
			topRight *= scale;

			Vector3 bottomLeft = mTargetCamera.transform.forward * near - toTop - toRight;
			bottomLeft.Normalize();
			bottomLeft *= scale;

			Vector3 bottomRight = mTargetCamera.transform.forward * near + toRight - toTop;
			bottomRight.Normalize();
			bottomRight *= scale;

			frustumCorners.SetRow(0, bottomLeft);
			frustumCorners.SetRow(1, bottomRight);
			frustumCorners.SetRow(2, topRight);
			frustumCorners.SetRow(3, topLeft);

            mTargetMaterial.SetMatrix("_FrustumCornersRay", frustumCorners);

            mTargetMaterial.SetFloat("_FogDensity", fogDensity);
            mTargetMaterial.SetColor("_FogColor", fogColor);
            mTargetMaterial.SetFloat("_FogStart", fogStart);
            mTargetMaterial.SetFloat("_FogEnd", fogEnd);

			Graphics.Blit (src, dest, mTargetMaterial);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
