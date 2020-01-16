Shader "Unlit/Common/Simple_FrostedGlass"
{
//毛玻璃

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",COLOR)=(1,1,1,1)
		_Bump("Bump",2D)="bump" {}
		_SampleAmount("Sample Amount",int)=5   //采样偏移
		_RefractionAmount("Refraction Amount",Range(0,1.0))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"   "Queue"="Transparent"}  //确保不透明物体已经被渲染
        LOD 100

		GrabPass {"_RefractionTex"}  //抓取屏幕

        Pass
        {
			Tags
			{
				"LightMode"="ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				float4 tangent: TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;

				float4 tangetToWorld1:TEXCOORD3;
				float4 tangetToWorld2:TEXCOORD4;
				float4 tangetToWorld3:TEXCOORD5;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _Bump;
			float4 _Bump_ST;

			sampler2D _RefractionTex;
			float4 _RefractionTex_ST;
			float4 _RefractionTex_TexelSize;  //纹素
			int _SampleAmount;
			half _RefractionAmount;

			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				fixed3 worldPos=	mul(unity_ObjectToWorld,v.vertex);
				fixed3 worldNormal=normalize(UnityObjectToWorldNormal(	o.worldPos));
				fixed3 worldTangent=normalize(UnityObjectToWorldDir(v.tangent));
				fixed3 worldBitTangent=normalize(cross(worldNormal,worldTangent)*v.tangent.w);

				o.tangetToWorld1=fixed4(worldTangent.x,worldBitTangent.x,worldNormal.x,worldPos.x);
				o.tangetToWorld2=fixed4(worldTangent.y,worldBitTangent.y,worldNormal.y,worldPos.y);
				o.tangetToWorld3=fixed4(worldTangent.z,worldBitTangent.z,worldNormal.z,worldPos.z);


                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldPos=fixed3(i.tangetToWorld1.x,i.tangetToWorld2.x,i.tangetToWorld3.x);

				//fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(worldPos));

				fixed3 normalValue;
				normalValue.xy=	UnpackNormal(tex2D(_Bump,i.uv));
				normalValue.z=1-dot(	normalValue.xy,	normalValue.xy);

                // sample the texture
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb*_Color.rgb;

				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.rgb*albedo;
				fixed3 diffuseColor=_LightColor0.rgb*albedo*saturate(dot(worldNormal,worldLightDir));

				//fixed3 refractDir=refract(-1*worldViewDir,worldNormal,)
			//	fixed4 screenpos=ComputeScreenPos(fixed4(i.worldPos,1));
			//	screenpos.xy+=_RefractionTex_TexelSize.xy*_SampleAmount;

				float2 uv2= i.uv;
				#ifdef UNITY_UV_STARTS_AT_TOP
					if(_RefractionTex_ST.y<0)
						uv2.y=1-uv2.y;
				#endif

				return fixed4(tex2D(_RefractionTex, i.uv).rgb,1.0);
				//fixed3 refractColor=tex2D(_RefractionTex,screenpos.xy/screenpos.w).rgb;

				//fixed3 finalColor=lerp(diffuseColor,refractColor,_RefractionAmount);

    //            return fixed4(ambient+finalColor,1);
            }
            ENDCG
        }
    }
}
