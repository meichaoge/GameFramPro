// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Common/FB_Bump_Diffuse_TangLight_Vertex"
{
//性能较好 在顶点着色器中转换了光源方向 
//适用于前向渲染中主光源关照  切线空间计算光照    
//普通纹理+法线纹理ForwardBase 光照 ，漫反射+环境光 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_Bump("Bump",2D)="bump" {}  //法线纹理
		_BumpScale("BumpScale",Range(0.0,1))=1.0  //法线纹理的系数
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
				float3 tangetLightDir:TEXCOORD1;
            };

			sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _Bump;
			float4 _Bump_ST;

			fixed3 _Color;
			fixed _BumpScale;

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

				//第一种自己计算转换矩阵
				float3 localTangentDir=normalize(v.tangent);
				float3 localNormalDir=normalize(v.normal);
				float3 localBitTangent=cross(localNormalDir,localTangentDir.xyz)*v.tangent.w;
				float3x3 localToTanget=float3x3(localTangentDir,localBitTangent,localNormalDir);

				////第二种 使用内置的宏 TANGENT_SPACE_ROTATION 等同于上面的计算
				//TANGENT_SPACE_ROTATION

			
				float3 localLightDir=ObjSpaceLightDir(v.vertex);

				o.tangetLightDir=mul(localToTanget,localLightDir);
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				float4 packedNormal=tex2D(_Bump,i.uv.zw);//法线纹理存储的数据
				float3 tangetNormal;
				tangetNormal.xy=UnpackNormal(packedNormal);  //获取法线纹理数值
				tangetNormal.xy*=_BumpScale;
				tangetNormal.z=sqrt(1-dot(tangetNormal.xy,tangetNormal.xy));

				  // sample the texture 采样纹理坐标
				fixed3 albedo=tex2D(_MainTex, i.uv.xy).rgb*_Color.rgb;

				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT*albedo.rgb; //环境光
				float3 tangetLightDir=normalize(i.tangetLightDir);

				fixed3 diffuseColor= _LightColor0.rgb*albedo.rgb*saturate(dot(tangetNormal,tangetLightDir));

                return fixed4(ambient.rgb+diffuseColor.rgb,1.0);
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
