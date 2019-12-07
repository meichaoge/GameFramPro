// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShaderBook/GrabPassShader"
{
    Properties
    {
		_Color("Color",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Bump("Bump",2D) ="bump" {}
		_Enviroment("Enviroment",Cube)="" {}
		_Amount("Amount",Range(0,300))=100
		_RefractAcount("_RefractAcount",Range(0,1))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue"="Transparent"  }

        LOD 100

		GrabPass {"_BackgroundText"}  //抓取屏幕

        Pass
        {
			Tags {"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed4 _Color;
			sampler2D _Bump;
			float4 _Bump_ST;

			samplerCUBE _Enviroment;
			float4 _Enviroment_ST;

			half _Amount;
			half _RefractAcount;

			sampler2D _BackgroundText;
			float4 _BackgroundText_TexelSize; //纹素

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normalDir:NORMAL;
				float4 tangentDir: TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 pos : SV_POSITION;

				float4 screenPos: TEXCOORD1;

				float4 w1:TEXCOORD2;
				float4 w2:TEXCOORD3;
				float4 w3:TEXCOORD4;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _Bump);

				o.screenPos=ComputeScreenPos(  o.pos);

				fixed3 worldPos=mul(unity_ObjectToWorld,v.vertex);

				fixed3 worldNormal= UnityObjectToWorldNormal(v.normalDir);
				fixed3 wordlTanget=UnityObjectToWorldDir(v.tangentDir.xyz)* v.tangentDir.w;

				fixed3 bigTangetDir=normalize( cross(wordlTanget,worldNormal) );

				o.w1=fixed4(wordlTanget.x,bigTangetDir.x,worldNormal.x,worldPos.x);
				o.w2=fixed4(wordlTanget.y,bigTangetDir.y,worldNormal.y,worldPos.y);
				o.w3=fixed4(wordlTanget.z,bigTangetDir.z,worldNormal.z,worldPos.z);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldPos=fixed3(i.w1.w,i.w2.w,i.w3.w);
				fixed3 worldViewDir= normalize(UnityWorldSpaceViewDir(worldPos));

			
				float3 bump=UnpackNormal( tex2D( _Bump,i.uv.zw));

				float offset=bump.xy*_Amount*_BackgroundText_TexelSize.xy;
				i.screenPos.xy=offset+i.screenPos.xy;
				
				fixed3 refractColor=tex2D(_BackgroundText,	i.screenPos.xy/i.screenPos.w).rgb;  //折射

				bump=normalize(fixed3( dot(i.w1.xyz,bump),dot(i.w2.xyz,bump),dot(i.w3.xyz,bump)  ));

				fixed3 reflectDir=reflect(-1*worldViewDir,bump);
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed3 refletColor=texCUBE(_Enviroment,reflectDir).rgb*col.rgb; //反射颜色

				return fixed4(   refletColor*(1-_RefractAcount)+refractColor*  _RefractAcount,1  );

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
