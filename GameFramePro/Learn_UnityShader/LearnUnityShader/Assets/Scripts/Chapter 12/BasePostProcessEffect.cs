using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ImageEffectOpaque]   //类似的还有几个 用于在不同时刻得到不同的效果
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public abstract class BasePostProcessEffect : MonoBehaviour
{
    public bool IsEffectSupport { get; protected set; }
    [SerializeField]
    protected Shader mEffectShader;
    public Material mTargetMaterial { get; protected set; }

    private void Awake()
    {
        IsEffectSupport = CheckIfSupportEffect();
        if (IsEffectSupport == false)
            this.enabled = false;
        else
            mTargetMaterial = CheckMaterial(mEffectShader, mTargetMaterial);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected abstract void OnRenderImage(RenderTexture src, RenderTexture dest);

    //判断是否支持当前需要的后处理效果
    protected virtual bool CheckIfSupportEffect()
    {
        if (SystemInfo.supportsRenderTextures == false || SystemInfo.supportsImageEffects == false)
            return false;
        return true;
    }

    //检测shader 是否合法
    protected virtual Material CheckMaterial(Shader shader, Material material)
    {
        if (shader == null)
            return null;

        if (shader.isSupported == false)
            return null;

        if (material != null && material.shader == shader)
            return material;

        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }


}
