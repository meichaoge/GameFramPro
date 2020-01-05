// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Common/ForwardBaseDiffuse_Specular"
{
//适用于前向渲染中主光源 只有漫反射颜色+环境光
    Properties
    {
		_Diffuse("Diffuse",COLOR)=(1,1,1,1)
		_SpecularColor("Specular Color",COLOR)=(1,1,1,1)
		_Gloss("Gloss",Range(8,100))=10  //高光系数
    }
    SubShader
    {
        Tags {  "LightMode"="ForwardBase" "RenderType"="Opaque" }
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
			fixed4 _SpecularColor;
			half _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
			//	o.normal=UnityObjectToWorldNormal(v.normal);
			o.normal=mul(v.normal,(float3x3)unity_WorldToObject);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex); //世界坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb; //环境光

				fixed3 worldLightDir=normalize( WorldSpaceLightDir(i.worldPos));// 世界坐标空间下光源方向
				fixed3 worldNormal=normalize(i.normal);
				fixed3 woldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));  //世界坐标系下视角方向

				fixed3 diffuseColor=_LightColor0.rgb*_Diffuse.rgb*saturate(dot(worldLightDir,worldNormal));

				//第一种不计算反射方向 取视角方向和光线方向的和的一半然后与法线点乘积
				fixed3 halfDir=normalize(woldViewDir+worldLightDir);
				fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(halfDir,worldNormal)),_Gloss);  //高光颜色

				//第二种 根据入射方向和法线计算反射方向然后与视角方向点乘    r=2(N*I)N-I;  //N为法线单位向量 I为入射光线向量
			//	fixed3 refletDir=reflect(-1*worldLightDir,worldNormal);
			//	fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(refletDir,woldViewDir)),_Gloss);  //高光颜色

				return fixed4(ambientColor.rgb+diffuseColor.rgb+specularColor.rgb,1);
            }
            ENDCG
        }
    }
}
