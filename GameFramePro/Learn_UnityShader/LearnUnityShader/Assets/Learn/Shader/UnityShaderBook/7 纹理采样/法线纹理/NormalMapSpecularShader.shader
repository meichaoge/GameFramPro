Shader "UnityShaderBook/7/NormalMapSpecularShader"
{
//切线空间法线贴图+普通纹理+高光+漫反射+环境光

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
				float3 tagentLightDir:TEXCOORD1; //切线空间中的光线
				float3 tagentViewDir:TEXCOORD2; //切线空间中的视角

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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _NormalMap);


				fixed3 normalDir=(v.normalDir);
				fixed3 tangentDir=(v.tangentDir.xyz);
				fixed3 bitTangeDir=( cross(normalDir,tangentDir)*v.tangentDir.w); //副切线

				//手动计算模型到切线的矩阵 按照切线+副切线+法线行展开
				float3x3 rotation=float3x3(tangentDir,bitTangeDir,normalDir); //从模型空间转到切线空间的矩阵
				//或者使用
				//	TANGENT_SPACE_ROTATION; //unityu  内置的宏

				o.tagentLightDir=mul(rotation,ObjSpaceLightDir(v.vertex)).xyz ;// 切线空间的光线方向
				o.tagentViewDir=mul(rotation,ObjSpaceViewDir(v.vertex)).xyz ;// 切线空间的视角方向

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

			fixed3 tagentLightDir=normalize(i.tagentLightDir);
			fixed3 tagentViewDir=normalize(i.tagentViewDir);

			//展开切线法线纹理
			fixed4 packageNormal=tex2D(_NormalMap,i.uv.zw);   //采样获取的法线纹理的数据

			fixed3 normalDir ;  //纹理坐标中的法线
			//手动展开法线纹理
			//normalDir.xy= (packageNormal.xy*2-1)*_BumpScale;

			//或者使用内置的 (当纹理设置成Normal Map 必须使用这个)
			normalDir=UnpackNormal(packageNormal);
			normalDir.xy*=_BumpScale;

			normalDir.z=sqrt(1- saturate( dot(	normalDir.xy,	normalDir.xy))); 



			fixed3 albedo=tex2D(_MainTex,i.uv.xy).rgb*_ColorTint.rgb;
			fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo.rgb;

			fixed3 diffuseColor=_LightColor0.rgb*albedo.rgb*saturate(dot(normalDir,tagentLightDir)); //漫反射

			fixed3 halfDir=normalize(tagentLightDir+tagentViewDir);
			fixed3 specularColor=_LightColor0.rgb*_Specular.rgb*pow(saturate(dot(normalDir,halfDir)),_Glass); //高光系数

			return fixed4(diffuseColor+ambientColor+specularColor,1.0);
            }
            ENDCG
        }
    }

	FallBack "Specular"
}
