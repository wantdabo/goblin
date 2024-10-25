// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "E3DEffect/AirDistort/AutoNoise-Mask-Fre-Dir"
{
	Properties
	{
		_DistortMap("DistortMap", 2D) = "white" {}
		_Speed("Speed", Range( 0 , 1)) = 0.1
		_Distrot("Distrot", Range( 0 , 0.1)) = 0.1
		_DistortOffset("DistortOffset", Range( -2 , 2)) = -0.8
		_MaskMap("MaskMap", 2D) = "white" {}
		_Alpha("Alpha", Range( 0 , 5)) = 2
		_TilingU("Tiling-U", Range( 0 , 3)) = 1
		_TilingV("Tiling-V", Range( 0 , 3)) = 1
		_AutoUVDirection("Auto-UV-Direction", Vector) = (0.24,0.24,0,0)
		_DisFrePower("Dis-Fre-Power", Range( 0 , 30)) = 6.318185
		_DisFreScale("Dis-Fre-Scale", Range( 0 , 2)) = 1.430902
		[Toggle]_DisFreinvert("Dis-Fre-invert", Float) = 0
		[HDR]_FreColor("Fre-Color", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _FreColor;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _DistortMap;
		uniform float _TilingU;
		uniform float _TilingV;
		uniform float _Speed;
		uniform float2 _AutoUVDirection;
		uniform float _DistortOffset;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float _Alpha;
		uniform float _Distrot;
		uniform float _DisFreinvert;
		uniform float _DisFreScale;
		uniform float _DisFrePower;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 appendResult8 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
			float2 appendResult85 = (float2(_TilingU , _TilingV));
			float mulTime26 = _Time.y * _Speed;
			float2 panner32 = ( 1.0 * _Time.y * float2( 0,0 ) + ( mulTime26 * _AutoUVDirection ));
			float2 uv_TexCoord27 = i.uv_texcoord * appendResult85 + panner32;
			float4 tex2DNode9 = tex2D( _DistortMap, uv_TexCoord27 );
			float mulTime40 = _Time.y * _Speed;
			float2 panner41 = ( 1.0 * _Time.y * float2( 0,0 ) + ( mulTime40 * ( _AutoUVDirection * float2( -0.2,0.25 ) ) ));
			float2 uv_TexCoord37 = i.uv_texcoord * appendResult85 + panner41;
			float4 tex2DNode34 = tex2D( _DistortMap, uv_TexCoord37 );
			float2 appendResult17 = (float2(( tex2DNode9.r + tex2DNode34.r ) , ( tex2DNode9.g + tex2DNode34.r )));
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode4 = tex2D( _MaskMap, uv_MaskMap );
			float4 clampResult15 = clamp( ( ( tex2DNode4 * tex2DNode4.a * i.vertexColor.a ) * _Alpha ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float2 lerpResult2 = lerp( appendResult8 , ( appendResult17 * _DistortOffset ) , saturate( ( clampResult15 * _Distrot ) ).rg);
			float4 screenColor1 = tex2D( _GrabTexture, lerpResult2 );
			o.Emission = ( _FreColor + screenColor1 ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV45 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode45 = ( 0.0 + _DisFreScale * pow( 1.0 - fresnelNdotV45, _DisFrePower ) );
			o.Alpha = ( lerp(saturate( ( 1.0 - fresnelNode45 ) ),saturate( fresnelNode45 ),_DisFreinvert) * clampResult15 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}