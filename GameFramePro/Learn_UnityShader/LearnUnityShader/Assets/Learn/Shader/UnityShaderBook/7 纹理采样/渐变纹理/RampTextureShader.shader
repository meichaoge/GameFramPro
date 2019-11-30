Shader "Unlit/RampTextureShader"
{
    Properties
    {
       _RampTexture("RampTeture",Rect)="white" {}
	   _ColorTint("Color Tint",COLOR)=(1,1,1,1)
	   _SpcularColor("SpcularColor",COLOR)=(1,1,1,1)
	   _Glass("Glass",Range(5,250))=50
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
                float2 uv : TEXCOORD0;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half3 normalDir: TEXCOORD1;
				half3 lightDir : TEXCOORD2;
				half3 viewDir:TEXCOORD3;
            };

         sampler2D _RampTexture;
		 float4  _RampTexture_ST;

		 fixed3 _ColorTint;
		 fixed3 _SpcularColor;
		 half _Glass;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
			 o.uv=TRANSFORM_TEX(v.uv,_RampTexture);

			 o.normalDir=UnityObjectToWorldNormal(v.normalDir);  //法线
			 o.lightDir=WorldSpaceLightDir(v.vertex); //光线
			 o.viewDir=WorldSpaceViewDir(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
					
				fixed3 wordlNormal=normalize(i.normalDir);
				fixed3 worldLight=normalize(i.lightDir);
				fixed3 viewDir=normalize(i.viewDir);


				fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT*_ColorTint.rgb;

				half halfLambert=0.5+0.5* dot(wordlNormal,viewDir);

				fixed3 diffuseColor= _LightColor0.rgb* tex2D(_RampTexture,fixed2(halfLambert,halfLambert)).rgb*_ColorTint.rgb;

				fixed3 halfDir=normalize(worldLight+viewDir);

				fixed3 specularColor=_LightColor0.rgb* _SpcularColor.rgb* pow(saturate(dot(halfDir,wordlNormal)),_Glass );


                return fixed4(ambientColor+diffuseColor+specularColor,1.0f);
            }
            ENDCG
        }
    }

	FallBack "Specular"
}
