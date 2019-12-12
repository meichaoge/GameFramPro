Shader "Unlit/GausssianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurSize("EffectRange",float)=1
    }
    SubShader
    {
		CGINCLUDE
		   #include "UnityCG.cginc"

		    sampler2D _MainTex;
            float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			float _BlurSize;

		   struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[5] : TEXCOORD0;
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


		   fixed4 frag (v2f i) : SV_Target
            {
			half weight[3]={0.4026 , 0.2442, 0.0545}; //高斯核

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

		//水平方向
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_Horizontial
            #pragma fragment frag
          
            ENDCG
        }

		//垂直方向
		   Pass
        {
            CGPROGRAM
            #pragma vertex vert_Vertical
            #pragma fragment frag
            ENDCG
        }
    }
}
