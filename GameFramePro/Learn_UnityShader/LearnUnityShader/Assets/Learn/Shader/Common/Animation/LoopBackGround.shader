Shader "Unlit/Common/LoopBackGround"
{
//背景循环
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Background("Background Texture",2D)="" {}
		_backgroundSpeed("Background Speed",Range(1,50))=20
		_foregroundSpeed("Foreground Speed",Range(1,50))=10  //前景速度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			 sampler2D _Background;
            float4 _Background_ST;
			float4 _Background_TexelSize;

			half _backgroundSpeed;
			half _foregroundSpeed;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _Background);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float2 foregroundUV= i.uv.xy+float2(_MainTex_TexelSize.x*_foregroundSpeed*_Time.y,0);
                fixed4 forgroundColor = tex2D(_MainTex, foregroundUV);
				  //return fixed4(forgroundColor.rgb,1.0);
				float2 backgroundUV= i.uv.zw+float2(_Background_TexelSize.x*_backgroundSpeed*_Time.y,0);
                fixed4 backgroundColor = tex2D(_Background, backgroundUV);

				fixed3 finalColor=lerp(backgroundColor.rgb,forgroundColor.rgb,forgroundColor.a);

                return fixed4(finalColor,1.0);
            }
            ENDCG
        }
    }
}
