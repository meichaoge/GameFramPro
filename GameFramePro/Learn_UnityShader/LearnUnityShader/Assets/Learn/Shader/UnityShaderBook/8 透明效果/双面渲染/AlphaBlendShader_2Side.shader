﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/8/AlphaBlendShader_2Side"
{
//Alpha Blend 双面渲染

    Properties
    {
		_ColorTint("Color Tint",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaScale("AlphaScale",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true" "Queue"="Transparent"}
        LOD 100

		//渲染后面
		 Pass
        {
		  Tags{"LightMode"="ForwardBase"}
		  Cull Front
		  ZWrite Off
		  Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

				float3 worldNormal: TEXCOORD1;
				float3 worldPosition: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed4 _ColorTint;
			fixed _AlphaScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal=UnityObjectToWorldNormal(v.vertex);
				o.worldPosition=mul( unity_ObjectToWorld,v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

			fixed3 worldNormal=normalize(i.worldNormal);
			fixed3 llightDir=normalize(UnityWorldSpaceLightDir(i.worldPosition));

                fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 aldebo=col.rgb*_ColorTint.rgb;

			   fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*aldebo;

			   fixed3 diffuse=_LightColor0.rgb*aldebo*saturate(dot(worldNormal,llightDir));

                return fixed4(ambient+diffuse,col.a*_AlphaScale);
            }
            ENDCG
        }

			//渲染正面
        Pass
        {
		  Tags{"LightMode"="ForwardBase"}
		  Cull Back

		  ZWrite Off

		  Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

				float3 worldNormal: TEXCOORD1;
				float3 worldPosition: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed4 _ColorTint;
			fixed _AlphaScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal=UnityObjectToWorldNormal(v.vertex);
				o.worldPosition=mul( unity_ObjectToWorld,v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

			fixed3 worldNormal=normalize(i.worldNormal);
			fixed3 llightDir=normalize(UnityWorldSpaceLightDir(i.worldPosition));

                fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 aldebo=col.rgb*_ColorTint.rgb;

			   fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*aldebo;

			   fixed3 diffuse=_LightColor0.rgb*aldebo*saturate(dot(worldNormal,llightDir));

                return fixed4(ambient+diffuse,col.a*_AlphaScale);
            }
            ENDCG
        }
    }

	FallBack "Transparent/VertexLit"
	//FallBack "Transparent/Cutout/VertexLit"
}
