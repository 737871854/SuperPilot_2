Shader "Custom/Aircraft" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Illum ("Illumin (A)", 2D) = "white" {}
		_Emission ("Emission", Float) = 1
		_Brightness ("Brightness", Float) = 1
		_Glossness ("Glossness", Float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 300
	
	CGPROGRAM
	#pragma surface surf BlinnPhong

	sampler2D _MainTex;
	sampler2D _Illum;
	fixed4 _Color;
	half _Shininess;
	half _Emission;
	half _Brightness;
	half _Glossness;

	struct Input 
	{
		float2 uv_MainTex;
		float2 uv_Illum;
	};

	void surf (Input IN, inout SurfaceOutput o) 
	{
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 c = tex * _Color;
		o.Albedo = c.rgb * _Brightness;
		o.Emission = c.rgb * (tex2D(_Illum, IN.uv_Illum).a * _Emission);
		o.Gloss = tex.a * _Glossness;
		o.Alpha = c.a;
		o.Specular = _Shininess;
	}
	ENDCG
	}
	FallBack "Self-Illumin/Diffuse"
}
