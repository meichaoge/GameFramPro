Shader "UnityShaderBook/DiffuseOfFragmentShader2"
{

// 在顶点着色器中计算世界坐标的法线
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
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normalDir:COLOR;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               // o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			   //在顶点着色器中计算法线能 提高计算效率，也可以不用归一化
			   o.normalDir=normalize(mul(v.normalDir,(float3x3) unity_WorldToObject)); 

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			  fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT; //环境光

			  fixed3 worldLightDir= normalize(_WorldSpaceLightPos0.xyz);

			  fixed3 diffuseColor= _DiffuseColor.rgb *  _LightColor0.rgb*saturate(dot( normalize(i. normalDir) ,worldLightDir));

                return fixed4(diffuseColor.rgb+ambientColor.rgb,1.0);
            }
            ENDCG
        }
    }


	//	FallBack "Diffuse"

}
