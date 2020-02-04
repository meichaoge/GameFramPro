Shader "Unlit/Effect/OutLine2Pass_ViewNormal"
{
    // 适用于在视角空间法线方向拓展法线
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color",Color)=(1,1,1,1)
        _OutlineWidth("Outline Width",Range(0.01,2))=1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

    //----第一个Pass 只渲染背面
        Pass
        {
            Cull front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal :NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            half    _OutlineWidth;

            v2f vert (appdata v)
            {
                float4 viewPos=mul(UNITY_MATRIX_MV,v.vertex);
                float3 viewNormal=mul(UNITY_MATRIX_IT_MV,v.normal);
                viewNormal.z=-0.5;
                viewPos.xy+=viewNormal.xy*_OutlineWidth;

                v2f o;
                o.vertex =mul(UNITY_MATRIX_P,viewPos);
                // float3 worldPos= mul(unity_ObjectToWorld,v.vertex);
                // fixed3 localViewDir=normalize(ObjSpaceViewDir(v.vertex));
                // fixed3 localNormal=;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                return fixed4(_OutlineColor.rgb,1.0);
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
            half    _OutlineWidth;

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
