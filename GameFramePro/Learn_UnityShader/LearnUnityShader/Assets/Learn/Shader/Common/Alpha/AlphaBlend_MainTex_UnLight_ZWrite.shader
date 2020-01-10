Shader "Unlit/Common/AlphaBlend_MainTex_UnLight_ZWrite"
{

//适用于需要ALpha 混合的情况 无光照  开启深度写入  透明物体本身不会有半透明效果
//原理两个Pass  第一个Pass 写入深度单数不输出 颜色 第二个Pass 正常输出颜色不写入深度
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
              
                return col*_Color;
            }

	ENDCG


        Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="true"
		}//Alpha Blend 标准Tags

        LOD 100

		pass 
		{
			ZWrite On
			ColorMask 0  //不输出颜色
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off  //必须关闭深度写入
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
    }
	FallBack "Transparent/VertexLit"
}
