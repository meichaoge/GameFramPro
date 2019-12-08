Shader "UnityShadersBook/LoopBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FrontTexure("Front Texture",Rect)="" {}
	//	_TotalCount("TotalSpeed",Range(1,100))=50
		_BackGroundSpeed("Background Speed",Range(0,1))=0.2
		_FrontSpeed("Front Speed",Range(0,1))=0.1

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true" "Queue"="Transparent" }
        LOD 100

        Pass
        {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _FrontTexure;
			float4 _FrontTexure_ST;

		//		half _TotalCount;
			half _BackGroundSpeed;
			half _FrontSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//half2 backUV= i.uv+ half2( (_Time.y*_BackGroundSpeed%_TotalCount)/_TotalCount,0);
			//	half2 frontUV= i.uv+ half2((_Time.y*_FrontSpeed%_TotalCount)/_TotalCount,0);

					half2 backUV= i.uv+ half2( frac(_Time.y*_BackGroundSpeed),0);
				half2 frontUV= i.uv+ half2( frac(_Time.y*_FrontSpeed),0);

                // sample the texture
                fixed4 back = tex2D(_MainTex, backUV);
                fixed4  front = tex2D(_FrontTexure, frontUV);

				fixed3 col=lerp(back,front,front.a);

                // apply fog
                
                return fixed4(col,1);
            }
            ENDCG
        }
    }
}
