﻿Shader "Unlit/Common/AlphaBlend_MainTex_UnLight_DoubleSide"
{

//适用于需要ALpha 混合的情况 无光照  双面渲染
//原理 采用两个Pass 先渲染背面然后渲染正面
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



		ZWrite Off  //必须关闭深度写入
		Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
			Cull Front 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
         
            ENDCG
        } //渲染背面

		   Pass
        {
			Cull Back 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
		
         
            ENDCG
        } //渲染正面
    }
	FallBack "Transparent/VertexLit"
}
