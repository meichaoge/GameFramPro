Shader "Unlit/Common/Billboard"
{
//广告牌
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NormalFrezze("Freeze Normal",Range(0,1))=1 //是否冻结法线
		_CenterPoint("Center",vector)=(0,0,0)  //中心点
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "DisableBatch"="true" "IgnoreProjector"="true"}
        LOD 100
		Cull Off


        Pass
        {
		Tags{
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			int _NormalFrezze;
			float3 _CenterPoint;

            v2f vert (appdata v)
            {
			//	float3 worldPos=mul(unity_ObjectToWorld,v.vertex);
				fixed3 viewDir=normalize(ObjSpaceLightDir(v.vertex));

				fixed3 upDir=fixed3(0,1,0);
					if(viewDir.y>=0.99)
						upDir=fixed3(1,0,0); //避免法线和向上方向重合
				fixed	rightDir=normalize(cross(viewDir,upDir));

				if(_NormalFrezze==1){
					upDir=normalize(cross(rightDir,viewDir));
				}//法线固定为朝向摄像机
				else	{
					viewDir=normalize(cross(upDir,rightDir));
				}//向上方向固定


				float3 vertexOffset=v.vertex.xyz-_CenterPoint;

				float3 newPos=_CenterPoint+float3(vertexOffset.x*rightDir+vertexOffset.y*upDir+vertexOffset.z*viewDir);

                v2f o;
				 o.vertex = UnityObjectToClipPos(fixed4(newPos,1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
