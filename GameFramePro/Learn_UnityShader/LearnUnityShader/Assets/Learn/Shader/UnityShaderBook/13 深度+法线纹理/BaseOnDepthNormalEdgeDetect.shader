Shader "UnityShaderBook/13/BaseOnDepthNormalEdgeDetect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_EdgeOnly("EdgeOnly",float)=1
		_EdgeColor("Edge Color",COLOR)=(0,0,0,1)
		_Background("Background",COLOR)=(1,1,1,1)
		_Sensity("Sensity",vector)=(1.0,1.0,1.0,1.0)
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
				o.uv[1]=originalUv+_MainTex_TexelSize.xy*half2(-1,1);
				o.uv[2]=originalUv+_MainTex_TexelSize.xy*half2(1,1); //右上角
				o.uv[3]=originalUv+_MainTex_TexelSize.xy*half2(1,-1);
				o.uv[4]=originalUv+_MainTex_TexelSize.xy*half2(-1,-1);
				

                return o;
            }

			  half CheckSame(half4 sampler1,half4 sampler2)
			  {
					float depth1= DecodeFloatRG(sampler1.zw);
					depth1=LinearEyeDepth(depth1); //转换成真正的深度值
					half2 normal1=sampler1.xy;

					float depth2=DecodeFloatRG(sampler2.zw);
						depth2=LinearEyeDepth(depth2); //转换成真正的深度值
					half2 normal2=sampler2.xy;

					int isSameDepth=1;
					if(abs(depth1-depth2)>_Sensity.x)
						isSameDepth=1; //不相同

					int isSameNormal=1;
					half2 normal=normal1-normal2;

					if(abs(normal.x)+abs(normal.y)>_Sensity.y)
					isSameNormal=1;

					if(isSameDepth*isSameNormal >0.5)
						return 1;
					return 0;
			   }


			  fixed4 fragRobert (v2f i) : SV_Target{
				half4 sample1=tex2D(_CameraDepthNormalsTexture,i.uv[1]);
				half4 sample2=tex2D(_CameraDepthNormalsTexture,i.uv[2]);
				half4 sample3=tex2D(_CameraDepthNormalsTexture,i.uv[3]);
				half4 sample4=tex2D(_CameraDepthNormalsTexture,i.uv[4]);

				half edge=1.0;
				edge*=CheckSame(sample1,sample3);
				edge*=CheckSame(sample2,sample4);

				return fixed4(edge,edge,edge,1);

			//	fixed3 edgOrignalColor=lerp(tex2D(_MainTex,i.uv[0]),_EdgeColor.rgb,edge);
				//return fixed4(edgOrignalColor,1);
			//	fixed3 onlyEdgColor=lerp(_Background,_EdgeColor,edge);
			//	return fixed4( lerp(edgOrignalColor,onlyEdgColor,_EdgeOnly),1);
			   }

			 

            ENDCG
        }
    }

	FallBack Off
}
