Shader "UnityShadersBook/SequcenceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_HorizontialAmount("HorizontialAmount",int)=8
		_VerticalAmoumt("VerticalAmount",int)=8
		_Speed("Speed",Range(1,100))=30
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true"  "Queue"="Transparent" }
        LOD 100
		

        Pass
        {
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			int _HorizontialAmount;
			int _VerticalAmoumt;
			int _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			//需要使用取整  floor 否则显示效果会是移动的 因为有小数的偏移
			   half index= 	_Time.y *_Speed%(_HorizontialAmount*_VerticalAmoumt);
			   fixed rowIndex=  ( 1-1* floor(index/_HorizontialAmount))+i.uv.y;
			   fixed columnIndex=  floor(index%_VerticalAmoumt)+i.uv.x;
			 half2  uv=half2(columnIndex/_HorizontialAmount,  rowIndex/_VerticalAmoumt);


				//float time=floor(_Time.y*_Speed);
				//float row=floor(time/_HorizontialAmount);
				//float column=time-row*_VerticalAmoumt;

				//half2 uv=i.uv+half2(column,-row);
				//uv.x=uv.x/_HorizontialAmount;
				//uv.y=uv.y/_VerticalAmoumt;


                // sample the texture
                fixed4 col = tex2D(_MainTex, uv);
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
