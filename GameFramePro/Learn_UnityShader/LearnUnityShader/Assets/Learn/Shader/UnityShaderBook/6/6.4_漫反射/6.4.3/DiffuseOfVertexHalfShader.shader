// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "UnityShaderBook/6.4.3/DiffuseOfVertexHalfShader"
{
    Properties
    {
	//	_DiffuseColor("DifusseColor",COLOR)=(1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
		Tags{"LightMode"="ForwardBase"}

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members normal)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
           //     float2 uv : TEXCOORD0;
			//	float3 normal ：NORMAL;
            };

            struct v2f
            {
             //   float2 uv : TEXCOORD0;
			//	fixed3 color: COLOR ;
                float4 vertex : SV_POSITION;
            };

			fixed4 _DiffuseColor;
			half  Diffuse;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
           //     o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//漫反射颜色
	//			fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT; 
					
				//法线的转换矩阵 是模型到世界的逆矩阵的转置 
	//			fixed3 worldNormal=normalize( mul(v.normal,(float3x3)unity_WorldToObject));// 世界坐标下的法线

				//主光源的方向
	//			fixed3 worldLightDir= normalize(_WorldSpaceLightPos0.xyz); 

				//根据BlinPhong 漫反射公式计算漫反射颜色
	//			fixed3 diffuseColor=_LightColor0.rgb*_DiffuseColor.rgb*saturate(dot(worldNormal,worldLightDir)); 

	//			o.color=ambient+diffuseColor;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			return fixed4(1,1,1,1)
             //   return fixed4(i.color,1.0);
            }
            ENDCG
        }
    }

		FallBack "Diffuse"
}
