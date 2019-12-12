Shader "Unlit/EdgeDetect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_EdgeOnly("EdgeOnly",float)=1
		_EdgeColor("Edge Color",COLOR)=(0,0,0,1)
		_Background("Background",COLOR)=(1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			ZTest Always
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[9] : TEXCOORD0; //当前位置9个相邻的像素点
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			half _EdgeOnly;
			fixed3 _EdgeColor;
			fixed3 _Background;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

				half2 originalUv=TRANSFORM_TEX(v.uv, _MainTex);

				o.uv[0]=originalUv+_MainTex_TexelSize.xy*half2(-1,1);
				o.uv[1]=originalUv+_MainTex_TexelSize.xy*half2(0,1);
				o.uv[2]=originalUv+_MainTex_TexelSize.xy*half2(1,1);
				o.uv[3]=originalUv+_MainTex_TexelSize.xy*half2(-1,0);
				o.uv[4]=originalUv ;//+_MainTex_TexelSize.xy*half2(1,1);
				o.uv[5]=originalUv+_MainTex_TexelSize.xy*half2(1,0);
				o.uv[6]=originalUv+_MainTex_TexelSize.xy*half2(-1,-1);
				o.uv[7]=originalUv+_MainTex_TexelSize.xy*half2(0,-1);
				o.uv[8]=originalUv+_MainTex_TexelSize.xy*half2(1,-1);


           //     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

					//计算亮度
			fixed GetLuninance(fixed3 color)
			{
				return 0.2125*color.r+0.7154*color.g+0.0721*color.b;
			}
			//计算梯度
			half Sobel(v2f i)
			{
				const int Gx[9]={-1,-2,-1,0,0,0,1,2,1};
				const int Gy[9]={-1,0,1,-2,0,2,-1,0,1};
 
				half gx=0;
				half gy=0;
				half gradient=0;

				for(int dex=0;dex<9;++dex)
				{
					 fixed luminance=GetLuninance(tex2D(_MainTex,i.uv[dex]));
					gx +=luminance*Gx[dex];
					gy +=luminance*Gy[dex];
				}
				gradient=abs(gx)+abs(gy);
				return gradient;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				half gradient=Sobel(i); //计算梯度

				fixed3 originalColor=tex2D(_MainTex,i.uv[4]);

				fixed3 edgOrignalColor=lerp(originalColor,_EdgeColor.rgb,gradient);

				//return fixed4(edgOrignalColor,1);
				fixed3 onlyEdgColor=lerp(_Background,_EdgeColor,gradient);
            
                return fixed4( lerp(edgOrignalColor,onlyEdgColor,_EdgeOnly),1);
            }


            ENDCG
        }
    }

	FallBack Off
}
