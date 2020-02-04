Shader "Unlit/Effect/OutLine"
{  
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color",Color)=(1,1,1,1)
        _OutlineWidth("Outline Width",Range(0.01,0.5))=0.02
        _OutLineScale("Outline Scale",Range(0,1.0))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal :NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            half    _OutlineWidth;
            fixed _OutLineScale;

            v2f vert (appdata v)
            {
                v2f o;
                fixed3 objectViewDir=normalize( ObjSpaceViewDir(v.vertex));
                //if(dot(objectViewDir,normalize(v.normal))<=0.1)
                    v.vertex.xyz +=_OutlineWidth*v.normal;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex);
                o.worldNormal=mul(v.normal,unity_WorldToObject);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed3 worldNormal=  normalize(i.worldNormal);
                fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

                fixed worldValue=dot(worldNormal,worldViewDir);

                fixed3 col = tex2D(_MainTex, i.uv).rgb;
                if(worldValue<=0.1){
                    col=lerp(col,_OutlineColor.rgb,_OutLineScale);
                }


                return fixed4(col,1.0);
            }
            ENDCG
        }
    }
}
