// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/10/FeiNeiRefract"
{
//菲涅尔反射

    Properties
    {
		_Color("Color",COLOR)=(1,1,1,1)
		_RefractAmount ("Reflect Amount", Range(0, 1)) = 1  //折射百分百
		_RefractColor ("Refraction Color", Color) = (1, 1, 1, 1)
		_Cubemap("Cub Map",Cube)="_Skybox" {}
    }
    SubShader
    {
       	Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 100

        Pass
        {

		Tags{"LightMode"="ForwardBase"}

            CGPROGRAM
				#pragma multi_compile_fwdbase	


            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				fixed3 worldNormal: TEXCOORD1;
				fixed4 worldPos: TEXCOORD2;
					SHADOW_COORDS(3)
            };

			samplerCUBE _Cubemap;
			float4 _Cubemap_ST;
			fixed4 _Color;
			fixed _RefractAmount;
			fixed _RefractRadi;
			fixed4 _RefractColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal=UnityObjectToWorldNormal(v.vertex);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);
					TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 worldLight=normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));


				fixed3 albedo= _Color.rgb;
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo;


				fixed3 diffuse=_LightColor0.rgb*albedo.rgb*saturate(dot(worldNormal,worldLight));

				fixed3 reflectDir=reflect(-1*viewDir,worldNormal);
				fixed3 reflectColor=texCUBE(_Cubemap,reflectDir).rgb*_RefractColor.rgb;

					UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

					fixed feiniere=saturate( _RefractAmount+(1-_RefractAmount)*pow((1-dot(viewDir,worldNormal)),5));

				return fixed4(ambient+  lerp(   diffuse,reflectColor,feiniere)*atten,1.0);
            }
            ENDCG
        }
    }
		FallBack "Reflective/VertexLit"
}
