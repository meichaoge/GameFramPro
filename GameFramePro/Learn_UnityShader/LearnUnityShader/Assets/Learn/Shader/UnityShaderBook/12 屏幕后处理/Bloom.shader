Shader "UnityShaderBook/12/GausssianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurSize("EffectRange",float)=1
		_Boom("Boom",2D)="black" {}
		_LuminanceThreshold("luminanceTheshold",Range(0,1))=0.5
    }
    SubShader
    {
	CGINCLUDE
		   #include "UnityCG.cginc"

		    sampler2D _MainTex;
            float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			float _BlurSize;
			half _LuminanceThreshold;//亮度阈值

		   struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f_Gaussian
            {
                float2 uv[5] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
			   struct v2f_Boom
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			   struct v2f_BoomBlend
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

		 v2f vert_Horizontial (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				half2 uv= TRANSFORM_TEX(v.uv, _MainTex);

				o.uv[0]=uv;
				o.uv[1]=uv+float2(_MainTex_TexelSize.x*1*_BlurSize,0);
				o.uv[2]=uv-float2(_MainTex_TexelSize.x*1*_BlurSize,0);
				o.uv[3]=uv+float2(_MainTex_TexelSize.x*2*_BlurSize,0);
				o.uv[4]=uv-float2(_MainTex_TexelSize.x*2*_BlurSize,0);
			
                return o;
            }
        
		   v2f vert_Vertical (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				half2 uv= TRANSFORM_TEX(v.uv, _MainTex);

				o.uv[0]=uv;
				o.uv[1]=uv+float2(0,_MainTex_TexelSize.y*1*_BlurSize);
				o.uv[2]=uv-float2(0,_MainTex_TexelSize.y*1*_BlurSize);
				o.uv[3]=uv+float2(0,_MainTex_TexelSize.y*2*_BlurSize);		
				o.uv[4]=uv-float2(0,_MainTex_TexelSize.y*2*_BlurSize);
                return o;
            }

			v2f_Boom vert_Bloom(appdata v)
			{
				 v2f_Boom o;
				 o.uv=TRANSFORM_TEX(v.uv,_MainTex);
				 o.vertex= UnityObjectToClipPos(v.vertex);
			}

		v2f_BoomBlend vert_BoomBlend(appdata v)
			{
			  v2f_BoomBlend o;
				 o.uv.xy=TRANSFORM_TEX(v.uv,_MainTex);
				 o.uv.zw=TRANSFORM_TEX(v.uv,_Boom);
				 o.vertex= UnityObjectToClipPos(v.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize.y<0)
						o.uv.w=1-o.uv.w;
				#endif
			}

		  fixed4 frag_bloomBlend (v2f_Boom i) : SV_Target
			{
					fixed4 color=tex2D(_MainTex,i.uv);
					return tex2D(_MainTex,i.uv.xy)+tex2D(_Boom,i.uv.zw);
			   }

		  fixed4 frag_bloom (v2f_Boom i) : SV_Target
			   {
					fixed4 color=tex2D(_MainTex,i.uv);
					fixed luminance=0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 

					fixed thehold= clamp(luminance-_LuminanceThreshold,0,1.0);
					return color*thehold;
			   }
			
		   fixed4 frag (v2f_Gaussian i) : SV_Target
            {
				half weight[3]={0.4026 , 0.2442, 0.0545}; //一维5x1 高斯核

				fixed3 col=tex2D(_MainTex, i.uv[0]).rgb*weight[0];

				for(int dex=1;dex<3;++dex)
			 {
				col+=tex2D(_MainTex, i.uv[dex*2-1]).rgb*weight[dex];
				col+=tex2D(_MainTex, i.uv[dex*2]).rgb*weight[dex];
			 }

				 return fixed4(col,1);
            }

		ENDCG


		ZWrite Off
		ZTest Always
		Cull Off

		Pass
		{
			CGPROGRAM
            #pragma vertex vert_Bloom
            #pragma fragment frag_bloom
			
            ENDCG
		}

		UsePass "UnityShaderBook/12/GausssianBlur/GAUSSUANBLUR_HORIZONTIAL" 

		UsePass "UnityShaderBook/12/GausssianBlur/GAUSSUANBLUR_VERTICAL" 

		Pass
		{
		  CGPROGRAM
            #pragma vertex vert_BoomBlend
            #pragma fragment frag_bloomBlend
            ENDCG
		}
    }

		FallBack Off
}
