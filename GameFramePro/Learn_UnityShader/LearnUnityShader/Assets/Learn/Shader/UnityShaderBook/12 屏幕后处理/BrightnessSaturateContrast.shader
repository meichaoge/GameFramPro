Shader "UnityShaderBook/12/BrightnessSaturateContrast"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Brightness("亮度",float)=1
		_Saturation("饱和度",float)=1
		_Contrast("对比度",float)=1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

		ZTest Always 
		Cull Off
		ZWrite Off

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
			half _Brightness;
			half _Saturation;
			half _Contrast;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
             fixed4 renderTexture=tex2D(_MainTex,i.uv);

			 fixed3 finalColor=renderTexture.rgb* _Brightness; //亮度


			 //饱和度
			 fixed luminace=0.2125*renderTexture.r+0.7154*renderTexture.g+0.0721*renderTexture.b;
			 fixed3 luminaceColor=fixed3(luminace,luminace,luminace);

			 finalColor=lerp(luminaceColor,finalColor,_Saturation);  //TODO 这里的插值大于0

			 //对比度
			 fixed3 avgColor=fixed3(0.5,0.5,0.5);
			 finalColor=lerp(avgColor,finalColor,_Contrast);

			 return fixed4(finalColor,renderTexture.a);
            }
            ENDCG
        }
    }
}
