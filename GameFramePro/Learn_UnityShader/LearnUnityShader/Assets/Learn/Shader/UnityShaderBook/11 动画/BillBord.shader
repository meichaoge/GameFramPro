// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/BillBord"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",COLOR)=(1,1,1,1)
		_NormalFrozen("Normal",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true"  "Queue"="Transparent" "DisableBatching"="true" }
        LOD 100

        Pass
        {
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

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
			fixed3 _Color;
			float _NormalFrozen;

            v2f vert (appdata v)
            {
                v2f o;


				float3 center=float3(0,0,0);
				fixed3 viewDir= normalize( mul(unity_WorldToObject,_WorldSpaceCameraPos)-center);

				fixed3 upDir= viewDir.y>0.99 ? fixed3(1,0,0) : fixed3 (0,1,0);

				fixed3 rightDir=normalize(cross(upDir,viewDir));


				if(_NormalFrozen>0.01)
					upDir=normalize(cross(viewDir,rightDir));
				else
					viewDir=normalize(cross(rightDir,upDir));

				float3 offset= (v.vertex-center);
				float3 localPos=center+rightDir*offset.x+upDir.xyz*offset.y+viewDir.xyz*offset.z;

                o.vertex = UnityObjectToClipPos(localPos);
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
