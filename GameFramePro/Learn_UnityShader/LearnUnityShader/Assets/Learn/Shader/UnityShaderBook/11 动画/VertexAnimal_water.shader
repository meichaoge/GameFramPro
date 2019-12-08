Shader "UnityShadersBook/VertexAnimal_water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",COLOR)=(1,1,1,1)
		_Frequence("Frequence",Range(0,200))=1
		_Magnituede("Magnitude",Range(0.1,3))=1 // 震动幅度
		_Speed("Speed",Range(0,1))=0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true" "Queue"="Transparent"  "DisableBatching"="true" }
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
			half _Frequence;
			half _Magnituede;
			half _Speed;

            v2f vert (appdata v)
            {
                v2f o;

				fixed4 offset=fixed4(0,0,0,0);
				offset.x=sin(_Time.y*_Frequence+v.vertex.y+v.vertex.z+v.vertex.x)*_Magnituede;

                o.vertex = UnityObjectToClipPos(v.vertex+offset);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv+=fixed2(0,_Time.y*_Speed);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return fixed4(col.rgb*_Color.rgb,1);
            }
            ENDCG
        }
    }
}
