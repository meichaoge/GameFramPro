Shader "CSND/BasicDiffuse"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {} //贴图
		_EmissiveColor("EmissiveColor",Color) = (1,1,1,1) //颜色
		_Smoth("Smoth",Range(0.1,3))=1
		_Rect("TestRect",Rect)="gray" {}
		_Cube("cube",Cube)="bump" {}
		_testFloat("floatValue",float)=1
		_testVector("vectorVlue",vector)=(0.1,0.2,0.3,0.4)

	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
