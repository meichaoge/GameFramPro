Shader "Unlit/Effect/OutLineZOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ZOffsetFactor("Zoffset factor",Range(-100,100))=1
        _ZOffsetUnit("Zoffset Unit",float)=1
        _OutlineColor("Outline Color",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        //---第一个Pass 渲染背面
        Pass
        {
            Cull Front
         //   ZWrite Off
            Offset [_ZOffsetFactor],[_ZOffsetUnit] //ZOffset 偏移

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ZOffsetFactor;
            float _ZOffsetUnit;
            fixed3 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_OutlineColor,1.0);
            }
            ENDCG
        }


          //----第二个Pass 只渲染正面
        Pass
        {
            Cull Back

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
            fixed4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv=TRANSFORM_TEX(v.uv,_MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                 fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }

    }
}
