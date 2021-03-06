﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/6.5/SpecularReflectOfFragmentShader"
{
    Properties
    {
    		_Diffuse("Diffuse",COLOR) =(1,1,1,1) //漫反射
		_Specular("Specular",COLOR)=(1,1,1,1)  //高光反射颜色
		_Glass("Glass",Range(1,50))=3  //高光反射系数
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

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
               // float2 uv : TEXCOORD0;
			   float3 normalDir : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
			   float3 normalDir : NORMAL;
			   float3 worldPos :COLOR;
            };

			fixed3 _Diffuse;
			fixed3 _Specular;
			half _Glass;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
              //  o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			  o.normalDir= mul(v.normalDir,(float3x3) unity_WorldToObject);

			  o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
             
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT; 

				float3 worldNormal= normalize(i.normalDir); //法线
 
				float3 worldLightDir=normalize(_WorldSpaceLightPos0);  //主光源

				half halfLambert=0.5* dot(worldNormal,worldLightDir)+0.5; //半兰伯特系数

				fixed3 diffuseColor=_LightColor0.rgb*_Diffuse.rgb* halfLambert; //漫反射颜色


				fixed3 reflectDir=  normalize( reflect(-1*worldLightDir,worldNormal)); //反射向量

				fixed3 viewDir=normalize( _WorldSpaceCameraPos.xyz -   i.worldPos.xyz );

				fixed3 specularColor=_LightColor0.rgb*_Specular.rgb *pow( saturate(dot(viewDir,reflectDir)),_Glass); //高光颜色

                return fixed4(ambient.rgb+diffuseColor.rgb+specularColor.rgb,1  );
            }
            ENDCG
        }
    }

	FallBack "Specular"
}
