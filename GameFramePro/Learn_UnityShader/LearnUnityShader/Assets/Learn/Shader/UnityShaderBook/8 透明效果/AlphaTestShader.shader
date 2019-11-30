Shader "UnityShaderBook/8/AlphaTestShader"
{
    Properties
    {
		_ColorTint("Color Tinit",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_CutOff("Cut Off",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "IgnoreProjector"="true"}
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
                float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

				float3 wordlNormal: TEXCOORD1;
				float4 worldPos: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed3 _ColorTint;
			fixed _CutOff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldPos=mul(  unity_ObjectToWorld,v.vertex);
				o.wordlNormal= UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
             
		
			 fixed4 color=tex2D(_MainTex,i.uv);

			clip(color.a-_CutOff);

			  fixed3 worldNormal=normalize(i.wordlNormal);
			 fixed3 lightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
			 fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

			 fixed3 albedo=color.rgb*_ColorTint.rgb;

			 fixed3 Ambient=UNITY_LIGHTMODEL_AMBIENT*albedo;

			 fixed3 diffuseColor=_LightColor0.rgb*albedo*saturate(dot(worldNormal,lightDir));

                return fixed4(Ambient+diffuseColor,1);
            }
            ENDCG
        }
    }

	FallBack "Transparent/Cutout/VertexLit"
}
