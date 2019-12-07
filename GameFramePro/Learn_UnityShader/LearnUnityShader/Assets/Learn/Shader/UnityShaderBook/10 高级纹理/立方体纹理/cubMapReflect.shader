// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/10/cubMapReflect"
{
    Properties
    {
		_Color("Color",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
			_ReflectAmount ("Reflect Amount", Range(0, 1)) = 1
		_Cubemap("Cub Map",Cube)="_Skybox" {}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				fixed3 worldNormal: TEXCOORD1;
				fixed4 worldPos: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			samplerCUBE _Cubemap;
			float4 _Cubemap_ST;
			fixed4 _Color;
			fixed _ReflectAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal=UnityObjectToWorldNormal(v.vertex);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 worldLight=normalize(UnityWorldSpaceLightDir(i.worldPos));


                fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 albedo= col.rgb*_Color.rgb;
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo;


				fixed3 diffuse=_LightColor0.rgb*albedo.rgb*saturate(dot(worldNormal,worldLight));

				fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 reflectDir=reflect(-1*viewDir,worldNormal);

				fixed3 reflectColor=texCUBE(_Cubemap,reflectDir);
				return fixed4(ambient+  lerp(   diffuse,reflectColor,_ReflectAmount),1.0);
            }
            ENDCG
        }
    }
		FallBack "VertexLit"
}
