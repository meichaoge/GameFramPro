Shader "Unlit/Common/FB_HalfDiffuse"
{
//适用于前向渲染中主光源关照
//只有一个纹理 无光照
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

			v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				//第一种 自己解析纹理坐标
			//	o.uv=v.uv*_MainTex_ST.xy+_MainTex_ST.zw;

				//第二种  使用Unity 宏 TRANSFORM_TEX
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture 采样纹理坐标
                fixed4 textColor = tex2D(_MainTex, i.uv);  
                return textColor;
            }

	ENDCG

        LOD 100

        Pass
        {
		Tags{
			"LightMode"="ForwardBase"
			"RenderType"="Opaque"
			"IgnoreProjector"="true"
		}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            ENDCG
        }
    }


	FallBack Off
}
