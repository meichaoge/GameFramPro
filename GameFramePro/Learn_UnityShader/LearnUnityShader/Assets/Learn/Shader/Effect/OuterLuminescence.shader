Shader "Unlit/Effect/OuterLuminescence"
{
    //适用于需要外发光的情况
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint",Color)=(1,1,1,1) 
        _OutLumineseColor("Luminese Color",Color)=(1,1,1,1) //外发光颜色
        _OutLumiceseWidth("Luminese Width",Range(0.01,2))=1.0  //外发光范围
        _OurLuminesePower("Luminese Power",Range(0.1,10))=2 //外发光系数
        _OurLumineseStrength("Luminese Strength",Range(0.1,1))=1  //外发光强度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        //----第一个Pass 只渲染背面
        Pass
        {
            Cull front
            Blend SrcAlpha One
            ZWrite Off
            //OneMinusSrcAlpha
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
                float3 worldPos : TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
            };

             uniform   sampler2D _MainTex;
             uniform   fixed4    _Color;
             uniform   float4    _MainTex_ST;
             uniform   fixed4    _OutLumineseColor;
             uniform   half      _OutLumiceseWidth;
             uniform   half      _OurLuminesePower;
             uniform   half      _OurLumineseStrength;


            v2f vert (appdata v)
            {
                //fixed3 localViewDir=normalize(ObjSpaceViewDir(v.vertex));
              // if(dot(localViewDir,localNormal)<0.1)
                v.vertex.xyz+=v.normal*_OutLumiceseWidth;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos= mul(unity_ObjectToWorld,v.vertex);
                o.worldNormal=mul(v.normal,unity_WorldToObject);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.worldNormal=normalize(i.worldNormal);
                fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

                fixed4 finalColor=_OutLumineseColor;
                half viewNormalValue=0.5- dot(worldViewDir, i.worldNormal)*0.5;
                finalColor.a=pow(viewNormalValue,_OurLuminesePower);
                finalColor.a *=viewNormalValue*_OurLumineseStrength;

                return finalColor;
               // return fixed4(viewNormalValue,viewNormalValue,viewNormalValue,1.0);
            }
            ENDCG
        }

         //----第二个Pass 只渲染正面
        Pass
        {
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                //float3 normal :NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float3 worldPos : TEXCOORD1;
                //float3 worldNormal:TEXCOORD2;
            };

             uniform   sampler2D _MainTex;
             uniform   fixed4    _Color;
             uniform   float4    _MainTex_ST;
             uniform   fixed4    _OutLumineseColor;
             uniform   half      _OutLumiceseWidth;
             uniform   half      _OurLuminesePower;
             uniform   half      _OurLumineseStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv=TRANSFORM_TEX(v.uv,_MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
               // float3 worldPos= mul(unity_ObjectToWorld,v.vertex);
              //  fixed3 localViewDir=normalize(ObjSpaceViewDir(v.vertex));
                
                //fixed3 localNormal=normalize(v.normal);
                // if(dot(localViewDir,localNormal)<0.1)
                //     o.vertex+=localNormal*_OutlineWidth;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                 fixed3 col = tex2D(_MainTex, i.uv).rgb*_Color.rgb;
                return fixed4(col,1.0);
            }
            ENDCG
        }
    }
}
