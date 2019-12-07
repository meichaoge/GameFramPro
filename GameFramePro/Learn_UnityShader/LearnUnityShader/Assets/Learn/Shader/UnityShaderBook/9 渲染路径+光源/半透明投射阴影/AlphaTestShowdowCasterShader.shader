Shader "UnityShaderBook/9/AlphaTestShowdowCasterShader"
{
    Properties
    {
		_Color("Color Tinit",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Cutoff("Cut Off",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "IgnoreProjector"="true"}
       // LOD 100

        Pass
        {
		Tags{"LightMode"="ForwardBase"}
		Cull Off

            CGPROGRAM
			#pragma multi_compile_fwdbase

            #pragma vertex vert
            #pragma fragment frag

			 #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 wordlNormal: TEXCOORD1;
				float3 worldPos: TEXCOORD2;

					SHADOW_COORDS(3)    //声明阴影纹理
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed3 _Color;   //这里的命名必须是 _Color 否则投射阴影错误
			fixed _Cutoff;

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldPos=mul(  unity_ObjectToWorld,v.vertex).xyz;
				o.wordlNormal= UnityObjectToWorldNormal(v.normal);

				TRANSFER_SHADOW(o);    //计算阴影纹理坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			 fixed4 texColor=tex2D(_MainTex,i.uv);

			clip(texColor.a-_Cutoff);

			  fixed3 worldNormal=normalize(i.wordlNormal);
			 fixed3 lightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
		//	 fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

			 fixed3 albedo=texColor.rgb*_Color.rgb;          

			 fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;

			 fixed3 diffuse=_LightColor0.rgb*albedo*saturate(dot(worldNormal,lightDir));

					UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

              return fixed4(ambient+diffuse*atten,1.0);
            }
            ENDCG
        }
    }

	FallBack "Transparent/Cutout/VertexLit"
}
