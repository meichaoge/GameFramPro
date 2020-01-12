// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/9/projectShadowAndLightShader"
{
		//阴影+光线衰减

    Properties
    {
		_ColorTint("ColorTint",COLOR)=(1,1,1,1)
    //    _MainTex ("Texture", 2D) = "white" {}
		_SpecularColor("Specular Color",COLOR)=(1,1,1,1)
		_Glass("Glass",Range(5,256))=20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue"="Geometry" }
        LOD 100

		//Base Pass 
        Pass
        {
			Tags{"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				float3 worldNormal: TEXCOORD0;
				float4 worldPos: TEXCOORD1;
				SHADOW_COORDS(2)     //声明阴影纹理
            };

			fixed3 _ColorTint;
			fixed3 _SpecularColor;
			half _Glass;


            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

				o.worldNormal=UnityObjectToWorldNormal(v.normalDir);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);

				TRANSFER_SHADOW(o);    //计算阴影纹理坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 lightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 ambedo=_ColorTint;//* col.rgb;
				fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed3 diffuseColor= _LightColor0.rgb*ambedo.rgb*saturate(dot(worldNormal,lightDir));
				fixed3 halfDir=normalize(viewDir+lightDir);
				fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate( dot(halfDir, worldNormal)),_Glass);

				//fixed attent=1;  //Base Pass 处理的是平行光 衰减为1
				//fixed shadow=SHADOW_ATTENUATION(i);  //计算阴影值

				UNITY_LIGHT_ATTENUATION(attent,i,i.worldPos);


                return fixed4(ambientColor.rgb+(diffuseColor.rgb+specularColor.rgb)*attent,1);
            }
            ENDCG
        }

		//////Additionnal pass
	   Pass
        {
			Tags{"LightMode"="ForwardAdd"}
			Blend One One  //确保其他光源效果叠加

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
		//	#include "UnityDeferredLibrary.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normalDir: NORMAL;
            };

              struct v2f
            {
                float4 pos : SV_POSITION;
				float3 worldNormal: TEXCOORD0;
				float3 worldPos: TEXCOORD1;
				SHADOW_COORDS(2)     //声明阴影纹理
            };

			fixed3 _ColorTint;
			fixed3 _SpecularColor;
			half _Glass;


            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

				o.worldNormal=UnityObjectToWorldNormal(v.normalDir);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);

					TRANSFER_SHADOW(o);    //计算阴影纹理坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal=normalize(i.worldNormal);

				//****这里可能处理各种类型的光源(平行光+点光源+聚光灯等)
				#ifdef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir=normalize(_WorldSpaceLightPos0.xyz);    //贫血光
				#else
					fixed3 lightDir=normalize(_WorldSpaceLightPos0.xyz-i.worldPos.xyz); //其他类型的光源
				#endif

				fixed3 ambedo=_ColorTint;//* col.rgb;
				fixed3 diffuseColor= _LightColor0.rgb*ambedo.rgb*saturate(dot(worldNormal,lightDir));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				fixed3 halfDir=normalize(viewDir+lightDir);
				fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate( dot(halfDir, worldNormal)),_Glass);


					UNITY_LIGHT_ATTENUATION(attent,i,i.worldPos);

                return fixed4((diffuseColor.rgb+specularColor.rgb)*attent,1);
            }
            ENDCG
        }
    }
		FallBack "Specular"
}
