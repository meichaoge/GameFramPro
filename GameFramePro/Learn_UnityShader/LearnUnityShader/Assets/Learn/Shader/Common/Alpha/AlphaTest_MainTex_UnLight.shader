Shader "Unlit/Common/AlphaTest_MainTex_UnLight"
{
//适用于需要使用Alpha Test   无光照
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
			fixed4 _Color;
			fixed _CutOff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a-_CutOff);

				fixed3 color=col.rgb*_Color.rgb;

                return fixed4(color.rgb,col.a);
            }

	ENDCG


		Cull off  //避免剔除背面

        LOD 100

        Pass
        {
			Tags { 
				"Queue"="AlphaTest"
				"RenderType"="TransparentCutout" 
				"IgnoreProjector"="true"
				}  //Alpha Test 标准Tags

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
