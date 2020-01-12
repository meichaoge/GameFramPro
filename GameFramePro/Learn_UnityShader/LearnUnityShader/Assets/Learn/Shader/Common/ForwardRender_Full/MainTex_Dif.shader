// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Common/MainTex_Dif"
{
//适用于使用前向渲染且处理各种逐像素的光源
//采用两个Pass 第一个ForwardBase 第二个ForwardAdd
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
    }
    SubShader
    {	

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		    struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
				float3 worldNormalDir:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldPos=mul(unity_ObjectToWorld,v.vertex);
				o.worldNormalDir=UnityObjectToWorldNormal(v.normal);
                return o;
            }

			//获取光源衰减系数
			fixed getAttenOfLight(float3 worldPos ){
				fixed atten=1.0;
#ifdef USING_DIRECTIONAL_LIGHT
	 atten = 1.0;
#else
	#if defined (POINT)
		// 把点坐标转换到点光源的坐标空间中，_LightMatrix0由引擎代码计算后传递到shader中，这里包含了对点光源范围的计算，具体可参考Unity引擎源码。经过_LightMatrix0变换后，在点光源中心处lightCoord为(0, 0, 0)，在点光源的范围边缘处lightCoord为1
		float3 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
		// 使用点到光源中心距离的平方dot(lightCoord, lightCoord)构成二维采样坐标，对衰减纹理_LightTexture0采样。_LightTexture0纹理具体长什么样可以看后面的内容
		// UNITY_ATTEN_CHANNEL是衰减值所在的纹理通道，可以在内置的HLSLSupport.cginc文件中查看。一般PC和主机平台的话UNITY_ATTEN_CHANNEL是r通道，移动平台的话是a通道
		 atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
#elif defined (SPOT)
		// 把点坐标转换到聚光灯的坐标空间中，_LightMatrix0由引擎代码计算后传递到shader中，这里面包含了对聚光灯的范围、角度的计算，具体可参考Unity引擎源码。
		//经过_LightMatrix0变换后，在聚光灯光源中心处或聚光灯范围外的lightCoord为(0, 0, 0)，在点光源的范围边缘处lightCoord模为1
		float4 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1));
		// 与点光源不同，由于聚光灯有更多的角度等要求，因此为了得到衰减值，除了需要对衰减纹理采样外，还需要对聚光灯的范围、张角和方向进行判断
		// 此时衰减纹理存储到了_LightTextureB0中，这张纹理和点光源中的_LightTexture0是等价的
		// 聚光灯的_LightTexture0存储的不再是基于距离的衰减纹理，而是一张基于张角范围的衰减纹理
		 atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0..w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
	#else
		 atten = 1.0;
	#endif
#endif
				return atten;
			}

			//适用于ForwardBase 片元
            fixed4 frag_Forwardbase (v2f i) : SV_Target
            {
				fixed3 worldNormalDir=normalize(i.worldNormalDir);
				fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 aldedo=col.rgb*_Color.rgb;

				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*aldedo.rgb;

				fixed3 diffuseColor=_LightColor0.rgb*aldedo.rgb*saturate(dot(worldNormalDir,worldLightDir));

				fixed atten=getAttenOfLight(i.worldPos);
                return fixed4(ambient+diffuseColor*atten,1.0);
            }

		//适用于ForwardAdd 片元  不需要计算自发光
			fixed4 frag_Forwardadd (v2f i) : SV_Target
            {
				fixed3 worldNormalDir=normalize(i.worldNormalDir);
				fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 aldedo=col.rgb*_Color.rgb;

				//fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*aldedo.rgb;

				fixed3 diffuseColor=_LightColor0.rgb*aldedo.rgb*saturate(dot(worldNormalDir,worldLightDir));
				fixed atten=getAttenOfLight(i.worldPos);
				

                return fixed4(diffuseColor*atten,1.0);
            }

		ENDCG

        Tags { 
			"RenderType"="Opaque" 
			"IgnoreProjector"="true"
			"Queue"="Geometry"
		}
        LOD 100

		//ForwardBase Pass 处理最亮的平行光和其他  
        Pass
        {
			Tags
			{
				"LightMode"="ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_Forwardbase
			#pragma multi_compile_fwdbase    //标识使用ForwardBase 处理
      
	  	#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

            ENDCG
        }

			//ForwardAdd  Pass 处理其他的逐像素光源
		 Pass
		 {
			Tags
			{
				"LightMode"="ForwardAdd"
			}
			Blend One One  //将当前的光照结果混合之前的结果

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_Forwardadd
			#pragma multi_compile_fwdadd                 //标识使用ForwardAdd 处理
         

		 	#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
            ENDCG
        }


    }
}
