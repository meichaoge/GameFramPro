Shader "Unlit/Common/SequenceOfFrames"
{
//序列帧动画
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RowCount("Row Count",int)=8  //垂直方向行数
		_ColumnCount("Column Count",int)=8  //水平方向列数
		_Speed("Speed",float)=24
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent"   }
        LOD 100
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			int _RowCount;
			int _ColumnCount;
			float _Speed;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				 int amount=floor(	_Time.y*_Speed);
				 int rowIndex=amount/_ColumnCount;
				 int columnIndex=amount%_ColumnCount;

				float2 uv=float2((i.uv.x+columnIndex)/_ColumnCount,(i.uv.y-rowIndex)/_RowCount);
                fixed4 col = tex2D(_MainTex,uv);
                return col;
            }
            ENDCG
        }
    }
}
