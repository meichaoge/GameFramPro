﻿Shader "Unlit/Common/Simple_Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_SampleAmount("Sample Amount",int)=5   //采样偏移
		_RefractionAmount("Refraction Amount",Range(0,1.0))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"   "Queue"="Transparent"}  //确保不透明物体已经被渲染
        LOD 100

		GrabPass {"_RefractionTex"}  //抓取屏幕

        Pass
        {
			Tags
			{
				"LightMode"="ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float3 worldNormal:TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _RefractionTex;
			float4 _RefractionTex_ST;
			float4 _RefractionTex_TexelSize;  //纹素
			int _SampleAmount;
			half _RefractionAmount;

			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);
				o.screenPos=ComputeGrabScreenPos(  o.pos);
				o.worldNormal=UnityObjectToWorldNormal(	o.worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal=normalize(i.worldNormal);
				//fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));


                // sample the texture
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb*_Color.rgb;

				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo;
				fixed3 diffuseColor=_LightColor0.rgb*albedo*saturate(dot(worldNormal,worldLightDir));

				fixed3 grabPassColor=tex2D(_RefractionTex,  i.screenPos.xy/i.screenPos.w +_RefractionTex_TexelSize.xy* _SampleAmount).rgb;

				return fixed4(ambient+lerp(diffuseColor,grabPassColor,_RefractionAmount),1.0);
            }
            ENDCG
        }
    }
}
