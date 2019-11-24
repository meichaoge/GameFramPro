// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/7/NormalMapSpecularShader2"
{
//切线空间法线贴图+普通纹理+高光+漫反射+环境光 
//光照计算在世界坐标系

    Properties
    {
	
        _MainTex ("Texture", 2D) = "white" {}
		_ColorTint("Color Tint",COLOR)=(1,1,1,1)
		_Specular("Specular",COLOR)=(1,1,1,1)
		_Glass("Glass",Range(5,200))=10  //高光系数
		_NormalMap("Normal Map",2D)="bump" {} //切线空间法线贴图
		_BumpScale("Bump Scale",Range(-1,1))=1 //法线纹理的凹凸程度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 texcoord : TEXCOORD0;

				float3 normalDir: NORMAL; 
				float4 tangentDir: TANGENT; //切线

            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

				float4 uv: TEXCOORD0; //分别存储两个纹理的 uv 其实一张就够了
				//float3 tagentLightDir:TEXCOORD1; //切线空间中的光线
			//	float3 tagentViewDir:TEXCOORD2; //切线空间中的视角

				float4 row1:TEXCOORD1;
				float4 row2:TEXCOORD2;
				float4 row3:TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _NormalMap;
			float4 _NormalMap_ST; 

			fixed3 _ColorTint;

			fixed3 _Specular;
			half _Glass;

			half _BumpScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _NormalMap);
				o.vertex=UnityObjectToClipPos(v.vertex);


                float3 worldPos= mul( unity_ObjectToWorld,v.vertex);

				half3 worldNormalDir=  UnityObjectToWorldNormal(v.normalDir); //世界坐标系下法线
				half3 worldTangeDir=UnityObjectToWorldDir(v.tangentDir); //世界坐标系下切线
				half3 worldBitTangDir=cross(worldNormalDir,worldTangeDir)*v.tangentDir.w; //副切线

				//切线到世界矩阵转换 按照切线空间坐标系的按列展开
				o.row1=float4(worldTangeDir.x,worldBitTangDir.x,worldNormalDir.x,worldPos.x);
				o.row2=float4(worldTangeDir.y,worldBitTangDir.y,worldNormalDir.y,worldPos.y);
				o.row3=float4(worldTangeDir.z,worldBitTangDir.z,worldNormalDir.z,worldPos.z);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


			float3 worldPos=fixed3(i.row1.w,i.row2.w,i.row3.w); 

			fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(worldPos));//光线方向
			fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(worldPos));//视角方向

				//展开切线法线纹理
			fixed3 normalDir=UnpackNormal(tex2D(_NormalMap,i.uv.zw));
			normalDir.xy*=_BumpScale;
			normalDir.z=sqrt(1- saturate( dot(	normalDir.xy,	normalDir.xy))); 

			fixed3	worldNormalDir=normalize(half3(dot(i.row1,normalDir),dot(i.row2,normalDir),dot(i.row3,normalDir))); //世界坐标下的法线

			fixed3 albedo=tex2D(_MainTex,i.uv.xy).rgb*_ColorTint.rgb;
			fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo.rgb;

			fixed3 diffuseColor=_LightColor0.rgb*albedo.rgb*saturate(dot(worldNormalDir,worldLightDir)); //漫反射

			fixed3 halfDir=normalize(worldLightDir+worldViewDir);
			fixed3 specularColor=_LightColor0.rgb*_Specular.rgb*pow(saturate(dot(worldNormalDir,halfDir)),_Glass); //高光系数

			return fixed4(ambientColor.rgb+diffuseColor.rgb+specularColor.rgb,1.0);
            }
            ENDCG
        }
    }

	FallBack "Specular"
}
