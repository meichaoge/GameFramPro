Shader "UnityShaderBook/7/SignalTextureShader"
{
    Properties
    {
		_ColorTint("Color Tint",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Specular("Specular",COLOR)=(1,1,1,1)
		_Glass("Glass",Range(1,40))=3
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				fixed3 wordlNormal: TEXCOORD1; 
				fixed3 viewDir:TEXCOORD2;
				fixed3 lightDir:TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed3 _Specular;
			half _Glass;

			fixed3 _ColorTint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			
				o.wordlNormal=UnityObjectToWorldNormal(v.normalDir) ;
				o.viewDir= 	WorldSpaceViewDir(v.vertex);
				o.lightDir=WorldSpaceLightDir(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 ambedo = tex2D(_MainTex, i.uv).rgb *_ColorTint;

			fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT*ambedo; 

			  fixed3 worldViewDir=normalize(i. viewDir) ;//世界坐标下视角向量

			  fixed3 worldNormalDir=normalize(i.wordlNormal ); //世界坐标下法线向量

			  fixed3 lightDir=normalize( i.lightDir); //光线向量


			  fixed3 diffuseColor= _LightColor0.rgb*ambedo*saturate(dot(worldNormalDir,lightDir));

			  fixed3 refelectDir=normalize( reflect(-1*lightDir,worldNormalDir));// 反射向量
			  fixed3 speculerColor= _LightColor0.rgb*_Specular.rgb*pow(saturate(dot(worldViewDir,worldNormalDir)),_Glass);


                return fixed4( ambientColor+diffuseColor+speculerColor  ,1.0f);
            }
            ENDCG
        }
    }

	FallBack "Specular"
}
