// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Common/FB_Bump_Diffuse_Specular_TangLight"
{
//适用于前向渲染中主光源关照  切线空间计算光照
//普通纹理+法线纹理ForwardBase 光照 ，漫反射+环境光 +高光
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_Bump("Bump",2D)="bump" {}  //法线纹理
		_BumpScale("BumpScale",Range(0.0,1))=1.0  //法线纹理的系数
		_Specular("Specular Color",COLOR)=(1,1,1,1)
		_Gloss("Gloss",Range(5,50))=10  //高光系数
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
				float4 tangent: TANGENT;
            };
		 struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 w1:TEXCOORD1;
				float4 w2:TEXCOORD2;
				float4 w3:TEXCOORD3;
            };

			sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _Bump;
			float4 _Bump_ST;

			fixed3 _Color;
			fixed _BumpScale;
			fixed3 _Specular;
			half _Gloss;

			v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
				//第一种 自己解析纹理坐标
				//	o.uv.xy=v.uv*_MainTex_ST.xy+_MainTex_ST.zw;

				//第二种  使用Unity 宏 TRANSFORM_TEX
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
			    o.uv.zw = TRANSFORM_TEX(v.uv, _Bump);


				//第一种 使用Unity 的宏
				float3 worldNormal=UnityObjectToWorldNormal(v.normal);

				//第二种是 使用模型到世界矩阵的逆矩阵的转置矩阵变换法线
				//	float3 worldNormal=mul(v.normal,unity_WorldToObject);

				float3 worldTangent=UnityObjectToWorldDir(v.tangent);  //切线
				float3 worldBigTangent=cross(worldNormal,worldTangent)*v.tangent.w;  //福切线

				//存储的是世界空间到切线空间的变换矩阵
				o.w1=fixed4(worldTangent.xyz, worldPos.x);
				o.w2=fixed4(worldBigTangent.xyz, worldPos.y);
				o.w3=fixed4(worldNormal.xyz, worldPos.z);


				//切线到世界的转换矩阵
					//o.w1=fixed4(worldTangent.x , worldBigTangent.x , worldNormal.x , worldPos.x);
				//o.w2=fixed4(worldTangent.y , worldBigTangent.y , worldNormal.y , worldPos.y);
				//o.w3=fixed4(worldTangent.z, worldBigTangent.z , worldNormal.z , worldPos.z);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			  
				float3 worldPos=float3(i.w1.w , i.w2.w , i.w3.w);

				float3 worldLightDir=UnityWorldSpaceLightDir(worldPos); //光线方向
				float3 worldViewDir=UnityWorldSpaceViewDir(worldPos);//视角方向

				float4 packedNormal=tex2D(_Bump,i.uv.zw); //法线纹理存储的数据
				fixed3 tangetNormal;
				tangetNormal.xy=UnpackNormal(packedNormal);  //获取法线纹理数值
				tangetNormal.xy*=_BumpScale;
				tangetNormal.z=sqrt(1-dot(tangetNormal.xy,tangetNormal.xy));

				float3 tangentLightDir=normalize( float3(dot(i.w1,worldLightDir),dot(i.w2,worldLightDir),dot(i.w3,worldLightDir)));
				float3 tangentViewDir= normalize(float3(dot(i.w1,worldViewDir),dot(i.w2,worldViewDir),dot(i.w3,worldViewDir)));

				    // sample the texture 采样纹理坐标
				fixed3 albedo=tex2D(_MainTex, i.uv.xy).rgb*_Color.rgb;
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT*albedo.rgb; //环境光

				fixed3 diffuseColor= _LightColor0.rgb*albedo.rgb*saturate(dot(tangetNormal,tangentLightDir));

				fixed3 halfDir=normalize(tangentLightDir+tangentViewDir);
				fixed3 specularColor= _LightColor0.rgb*_Specular* pow(saturate(dot(halfDir,tangetNormal)),_Gloss);

                return fixed4(ambient.rgb+diffuseColor.rgb+specularColor,1.0);
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
