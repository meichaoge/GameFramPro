Shader "UnityShaderBook/DiffuseOfFragmentShader"
{
    Properties
    {
		_DiffuseColor("Diffuse Color",COLOR)= (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags{"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed3 _DiffuseColor;

            struct appdata
            {
                float4 vertex : POSITION;
          //      float2 uv : TEXCOORD0;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
            //    float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normalDir:COLOR;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			   o.normalDir=v.normalDir;

               // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              
			  fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT; //环境光

			  fixed3 worldNormal= normalize(mul(i.normalDir,(float3x3) unity_WorldToObject));

			  fixed3 worldLightDir= normalize(_WorldSpaceLightPos0.xyz);

			  fixed3 diffuseColor= _DiffuseColor.rgb *  _LightColor0.rgb*saturate(dot(worldNormal,worldLightDir));

                return fixed4(diffuseColor.rgb+ambientColor.rgb,1.0);
            }
            ENDCG
        }
    }
}
