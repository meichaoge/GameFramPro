Shader "UnityShaderBook/13/BaseOnHeightFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FogColor("Fog Color",COLOR)=(1,1,1,1)
		_FogStart("Start",Range(0.1,5))=2
		_FogEnd("End",Range(0.1,5))=0.1
		_FogDensity("Density",Range(0.1,10))=3 //浓度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true" "Queue"="Transparent" }
      //  LOD 100

        Pass
        {
		ZWrite Off
		ZTest Always   
		Cull Off 

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
                float4 uv : TEXCOORD0;  
				float3 depthLinear:TEXCOORD1;  //相机到像素的插值射线
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			fixed4 _FogColor;
			half _FogStart;
			half _FogEnd;
			half _FogDensity;
			float4x4 _FrustumCornersRay; //四个点的坐标


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                o.uv.zw = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if(_MainTex_TexelSize.y<0)
					o.uv.w=1-o.uv.w;
				#endif

				int index=0;
				if(o.uv.x<0.5&& o.uv.y<0.5)
					index=0;
				else	if(o.uv.x>=0.5&& o.uv.y<0.5)
					index=1;
              	else	if(o.uv.x>=0.5&& o.uv.y>=0.5)
					index=2;
				else
					index=3;

				o.depthLinear=_FrustumCornersRay[index];
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
             
				float depth=SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv.zw);
			float	linearDepth=LinearEyeDepth(depth); //转换成真正的深度值
				
				float3 worldPos=_WorldSpaceCameraPos+linearDepth* i.depthLinear.xyz;

				half fogDensity= saturate((_FogEnd-worldPos.y)/  (_FogEnd-_FogStart)*_FogDensity); //浓度值


				col.rgb=lerp(col.rgb,_FogColor.rgb,fogDensity);

				return col;
            }
            ENDCG
        }
    }
	FallBack Off
}
