Shader "Unlit/FB_GridentTex_Diffuse"
{
	//适用于使用渐变纹理控制漫反射光照结果
//普通纹理+渐变纹理 

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_GradientTexture("Gradient",2D)="white" {}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldLightDir:TEXCOORD2;
            };

			 sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			sampler2D _GradientTexture;
			float4 _GradientTexture_ST;

			  v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
             
			  float3 worldPos=mul(unity_ObjectToWorld,v.vertex);

				o.worldNormal=UnityObjectToWorldNormal(v.normal);
				o.worldLightDir=UnityWorldSpaceLightDir(worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 albedo=col.rgb*_Color.rgb;

				float3 worldNormal=normalize(i.worldNormal);
				float3 worldLightDir=normalize(i.worldLightDir);

				fixed3 ambientColor=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo;

				float halfLambert=0.5*(dot(worldNormal,worldLightDir))+0.5;

				fixed3 diffuseColor=tex2D(_GradientTexture, float2(halfLambert,halfLambert)).rgb*albedo;
              
                return fixed4(ambientColor+diffuseColor,1.0);
            }

	ENDCG


        Tags {
		"RenderType"="Opaque"
		"LightMode"="ForwardBase"
		"IgnoreProjector"="true"
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

      
          
            ENDCG
        }
    }
}
