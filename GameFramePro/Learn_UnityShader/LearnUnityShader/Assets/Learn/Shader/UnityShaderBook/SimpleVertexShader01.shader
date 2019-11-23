// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnityShaderBook/SimpleVertexShader01"
{
	Properties{
			_DifuseColor("DifuseColor",COLOR)=(1,1,1,1)
		}   


    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			float4 _DifuseColor;

			float4  vert(float4 v:POSITION):SV_POSITION{
				return UnityObjectToClipPos(v);
			};

			fixed4	frag () : SV_TARGET0{
				return _DifuseColor;
				};
          
            ENDCG
        }
    }
}
