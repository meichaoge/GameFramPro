Shader "Unlit/Common/FB_AlphaTest_MainTex_Diffuse"
{
//适用于需要使用Alpha Test 
//漫反射光照
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_CutOff("Alpha Cut off",Range(0,1.0))=0.5
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
				float3 normal:Normal;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldLightDir: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			fixed _CutOff;

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
                fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a-_CutOff);


				fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 worldLightDir=normalize(i.worldLightDir);

				fixed3 aldebo=col.rgb*_Color.rgb;

				float3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*aldebo;

				float3 diffuseColor=_LightColor0.rgb*aldebo.rgb*saturate(dot(worldNormal,worldLightDir));


                return fixed4(ambient.rgb+diffuseColor.rgb,1.0);
            }

	ENDCG


		Cull off  //避免剔除背面
	
	Tags { 
				"Queue"="AlphaTest"
				"RenderType"="TransparentCutout" 
				"IgnoreProjector"="true"
				}  //Alpha Test 标准Tags
        LOD 100

        Pass
        {
			Tags{
		"LightMode"="ForwardBase"
		}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

       
            ENDCG
        }
    }

	FallBack "Transparent/Cutout/VertexLit"

}
