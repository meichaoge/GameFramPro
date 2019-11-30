Shader "Unlit/BlendOperate"
{
    Properties
    {
		_ColorTint("ColorTint",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaScale("Alpha",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="true" }
        LOD 100

        Pass
        {
			Tags{"LightMode"="ForwardBase"}

			Blend SrcAlpha OneMinusSrcAlpha


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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _ColorTint;
			fixed _AlphaScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
              
			  fixed3 albedo=_ColorTint.rgb* col.rgb;

                return fixed4(albedo.rgb,_AlphaScale);
            }
            ENDCG
        }
    }
}
