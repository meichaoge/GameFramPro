Shader "Unlit/Common/Simple_Mirror"
{
	
	//适用于使用渲染纹理的镜子
    Properties
    {
	   _Color( "Color Tint",COLOR)=(1,1,1,1)
	     _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv:TEXCOORD0;
            };

			sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color; 

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv .x=1-	o.uv .x;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			 fixed4 col = tex2D(_MainTex, i.uv);
				return fixed4(col.rgb,1.0);
            }
            ENDCG
        }
    }
}
