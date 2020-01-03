Shader "UnityShaderBook/13/BaseOnDepthNormalEdgeDetect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_EdgeOnly("EdgeOnly",float)=1
		_EdgeColor("Edge Color",COLOR)=(0,0,0,1)
		_Background("Background",COLOR)=(1,1,1,1)
		_Sensity("Sensity",vector)=(1.0,1.0,1.0,1.0)
		_SampleDistance ("Sample Distance", Float) = 1.0
    }

	//基于深度法线纹理的边缘检测

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="true"  }
        LOD 100

        Pass
        {
			ZTest Always
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRobert
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[5] : TEXCOORD0; //当前位置9个相邻的像素点
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			sampler2D _CameraDepthNormalsTexture;

			float _SampleDistance;
			half _EdgeOnly;
			fixed3 _EdgeColor;
			fixed3 _Background;

			half4 _Sensity; //深度灵敏度


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

				half2 originalUv=v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize.y<0)
						originalUv.y=1-originalUv.y;
				#endif

				//Robert 算子
				o.uv[0]=originalUv;
				o.uv[1]=originalUv+_MainTex_TexelSize.xy*half2(-1,1)*_SampleDistance;
				o.uv[2]=originalUv+_MainTex_TexelSize.xy*half2(1,1)*_SampleDistance; //右上角
				o.uv[3]=originalUv+_MainTex_TexelSize.xy*half2(1,-1)*_SampleDistance;
				o.uv[4]=originalUv+_MainTex_TexelSize.xy*half2(-1,-1)*_SampleDistance;

                return o;
            }

			  half CheckSame(half4 sampler1,half4 sampler2)
			  {
					float depth1= DecodeFloatRG(sampler1.zw);
					half2 normal1=sampler1.xy;

					float depth2=DecodeFloatRG(sampler2.zw);
					half2 normal2=sampler2.xy;

					float difDiff=abs(depth1-depth2)*_Sensity.x;


				//	int sameDepth= difDiff<0.1*depth1;
				//	half2 normal=abs(normal1-normal2)*_Sensity.y;
			//		int sameNormal=(normal.x+normal.y)<0.1;

					int isSameDepth=1;
					if(difDiff>depth1*0.1)
						isSameDepth=0; //不相同

					int isSameNormal=1;
					half2 normal=abs(normal1-normal2)*_Sensity.y;
					if((normal.x+normal.y)>0.1)
						isSameNormal=0.0;

					if(isSameNormal*isSameDepth >=1.0)
						return 1.0;
					return 0.0;
					//return sameNormal*sameDepth?1.0 : 0.0;
			   }


			  fixed4 fragRobert (v2f i) : SV_Target{
				half4 sample1=tex2D(_CameraDepthNormalsTexture,i.uv[1]);
				half4 sample2=tex2D(_CameraDepthNormalsTexture,i.uv[2]);
				half4 sample3=tex2D(_CameraDepthNormalsTexture,i.uv[3]);
				half4 sample4=tex2D(_CameraDepthNormalsTexture,i.uv[4]);

				half edge=1.0;
				edge*=CheckSame(sample1,sample3);
				edge*=CheckSame(sample2,sample4);

				fixed3 edgOrignalColor=lerp(_EdgeColor.rgb,tex2D(_MainTex,i.uv[0]),edge);
				fixed3 onlyEdgColor=lerp(_EdgeColor,_Background,edge);
				return fixed4( lerp(edgOrignalColor,onlyEdgColor,_EdgeOnly),1);
			   }

			 

            ENDCG
        }
    }

	FallBack Off
}
