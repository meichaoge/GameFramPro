﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Common/FB_MainTex_Diffuse_Specular"
{
//适用于前向渲染中主光源关照
//只有一个纹理ForwardBase 光照 ，漫反射+环境光
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_Specular("Specular Color",COLOR)=(1,1,1,1)
		_Gloss("Gloss",Range(5,50))=8
    }
    SubShader
    {

	CGINCLUDE
	   #include "UnityCG.cginc"
	   #include "Lighting.cginc"

		 struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
            };
		 struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
            };

			sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed3 _Color;
			fixed3 _Specular;
			half _Gloss;

			v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);
				//第一种 自己解析纹理坐标
				//	o.uv=v.uv*_MainTex_ST.xy+_MainTex_ST.zw;

				//第二种  使用Unity 宏 TRANSFORM_TEX
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//第一种 使用Unity 的宏
				o.worldNormal=UnityObjectToWorldNormal(v.normal);

				//第二种是 使用模型到世界矩阵的逆矩阵的转置矩阵变换法线
			//	o.worldNormal=mul(v.normal,unity_WorldToObject);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			      // sample the texture 采样纹理坐标
				fixed3 albedo=tex2D(_MainTex, i.uv).rgb*_Color.rgb;

				float3 worldNormal=normalize(i.worldNormal);
				float3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos)); //光线方向
				float3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos)); //视角方向
				  

				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT*albedo.rgb; //环境光

				fixed3 diffuseColor= _LightColor0.rgb*albedo.rgb*saturate(dot(worldNormal,worldLightDir));

				//第一种 计算反射方向然后点乘视角方向
				//fixed3 reflectDir=normalize(reflect(-1*worldLightDir,worldNormal));
				//fixed3 specularColor= _LightColor0.rgb*_Specular*pow(saturate(dot(reflectDir,worldViewDir)),_Gloss);

				//第二种 去视角方向和入射方向的一半然后点乘法线
				fixed3 halfDir=normalize(worldViewDir+worldLightDir);
				fixed3 specularColor= _LightColor0.rgb*_Specular*pow(saturate(dot(halfDir,worldNormal)),_Gloss);

                return fixed4(ambient.rgb+diffuseColor+specularColor,1.0);
            }

	ENDCG

        LOD 100

        Pass
        {
		Tags{
			"LightMode"="ForwardBase"
			"RenderType"="Opaque"
			"IgnoreProjector"="true"
		}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            ENDCG
        }
    }


	FallBack Off
}
