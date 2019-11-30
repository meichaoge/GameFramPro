Shader "Unlit/SpcularMaskShader"
{
    Properties
    {
		_ColorTint("Color Tint",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Bump("Bump",2D)= "white" {}
		_BumpScale("BumpScale",Range(-1,1))=1

		_SpecularColor("Specular Color",COLOR)=(1,1,1,1)
		_SpecularMask("Spcular Mask",2D)="white" {}
		_Glass("Glass",Range(8,256))=50
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
				float4 tangentDir: TANGENT;
				float3 normalDir: NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

				half3 tangetView:TEXCOORD1;
				half3 tangetLightDir:TEXCOORD2;
				half3 tangetNormalDir:TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
			sampler2D _Bump;
			float4 _Bump_ST;

			sampler2D _SpecularMask;
			float4 _SpecularMask_ST;

			fixed3 _ColorTint;
			fixed3 _SpecularColor;
			half _BumpScale;
			half _Glass;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw=TRANSFORM_TEX(v.uv,_SpecularMask);


				half3 bitTangetDir=cross(v.normalDir,v.tangentDir.xyz)*v.tangentDir.w; //副切线
				float3x3 objectToTanget=float3x3(v.tangentDir.xyz,bitTangetDir.xyz,v.normalDir.xyz); //模型到切线

				o.tangetView=mul(objectToTanget,ObjSpaceViewDir(v.vertex)); 
				o.tangetLightDir=mul(objectToTanget,ObjSpaceLightDir(v.vertex));  


                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              
			  fixed3 viewDir=normalize(i.tangetView);
			  fixed3 lightDir=normalize(i.tangetLightDir);

			  fixed3 tangentNormal= UnpackNormal(tex2D(_Bump,i.uv.zw));
			  tangentNormal.xy*=_BumpScale;
			  tangentNormal.z=sqrt(1-dot(  tangentNormal.xy,  tangentNormal.xy));


			  fixed3 ambedo= tex2D(_MainTex,i.uv.xy).rgb*_ColorTint.rgb;  //漫反射

			  fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb*ambedo;

			  fixed3 diffuseColor= _LightColor0.rgb*ambedo.rgb* saturate(dot(tangentNormal,viewDir));

			  fixed3 halfDir=normalize(viewDir+lightDir);
			  fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb* pow(saturate(dot(halfDir,tangentNormal)),_Glass);
			 fixed3 spacularMask=tex2D(_SpecularMask,i.uv.zw);
			 specularColor*=spacularMask;


			  return fixed4(ambientColor+diffuseColor+specularColor,1.0);
            }
            ENDCG
        }
    }
}
