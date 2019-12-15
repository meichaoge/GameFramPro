Shader "Unlit/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MotionAmount("Motion Amount",Range(0,0.9))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent","Queue"="Transparent" "IgnoreProjector"="true" }
        LOD 100

		ZTest Always
		ZWrite Off
		Cull Off

		CGINCLUDE
		sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed _MotionAmount;

		   struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			v2f vert(appdata v){
			   v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
			}

			fixed4 fragRGB(v2f i):SV_Target{
			 return fixed4( tex2D(_MainTex,	i.uv).rgb,_MotionAmount);
			}

			fixed4 fragA(v2f i):SV_Target{
				 return  tex2D(_MainTex,	i.uv);
			}

		ENDCG

		Pass{
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask RGB

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragRGB
				ENDCG
		}

			Pass{
				Blend One Zero
				ColorMask A

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragA
				ENDCG
		}

    }
	FallBack Off
}
