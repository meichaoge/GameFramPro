// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Common/ForwardBaseHalfDiffuse"
{
//适用于前向渲染中主光源 只有漫反射颜色+环境光 
//使用半兰伯特模型计算光照 避免背光面完全黑暗的情况
    Properties
    {
		_Diffuse("Diffuse",COLOR)=(1,1,1,1)
    }
    SubShader
    {
        Tags {  
		"LightMode"="ForwardBase"
		"RenderType"="Opaque" 
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				fixed4 normal: NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				fixed3 normal: TEXCOORD; //法线
				fixed4 worldPos: TEXCOORD1; //世界坐标
            };

			fixed4 _Diffuse;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal=UnityObjectToWorldNormal(v.normal);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex); //世界坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb; //环境光

				fixed3 worldLightDir=normalize( WorldSpaceLightDir(i.worldPos));// 世界坐标空间下光源方向
				fixed3 worldNormal=normalize(i.normal);

				fixed3 diffuseColor=_LightColor0.rgb*_Diffuse.rgb*(0.5*dot(worldLightDir,worldNormal)+0.5);

				return fixed4(ambientColor.rgb+diffuseColor.rgb,1);
            }
            ENDCG
        }
    }
}
