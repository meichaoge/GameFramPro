Shader "UnityShaderBook/13/MotionBlurWithDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurSize("Blur Size",Range(0,0.5))=0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent" "IgnoreProjector"="true" }
    //    LOD 100
	ZWrite Off
	ZTest Always
	Cull Off

		CGINCLUDE
		   #include "UnityCG.cginc"
		 sampler2D _MainTex;
         float4 _MainTex_ST;
		 half4 _MainTex_TexelSize;

		 sampler2D _CameraDepthTexture;

		 half _BlurSize;
		 float4x4 currentVPInvert;  //当前视角投影逆矩阵
		 float4x4 previousVP;//上一帧的视角投影矩阵

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

			 v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
			   o.uv.zw =v.uv;

			   #if UNITY_UV_STARTS_AT_TOP
			    if(_MainTex_TexelSize.y<0)
					o.uv.w=1- o.uv.w;
			   #endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half depth=SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv.zw); //采样深度纹理

				//映射到[-1,1]的立方体中
				half4 ndcPos=half4(i.uv.z*2-1,i.uv.w*2-1,depth*2-1,1); //返现映射得到归一化之前的 投影坐标
				float4 viewPos=mul(currentVPInvert,ndcPos); //视口坐标

				float4  worldPos=viewPos/viewPos.w; //世界坐标

				float4 previousProjectPos=mul(previousVP,worldPos);
				previousProjectPos /=previousProjectPos.w;

				half2 velocity=(ndcPos.xy- previousProjectPos.xy)/2.0;

                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);

				  half2 uv=i.uv;
				for(int dex=1;dex<3;++dex){
					uv+=velocity*_BlurSize;
					col+=tex2D(_MainTex,uv);
				}
				col /=3;

				return fixed4(col.rgb,1);
            }

		ENDCG


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
    }
}
