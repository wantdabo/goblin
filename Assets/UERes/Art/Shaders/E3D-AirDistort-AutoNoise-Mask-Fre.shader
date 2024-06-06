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
/*ASEBEGIN
Version=16400
37;442;1305;810;6228.865;2103.55;7.275129;True;False
Node;AmplifyShaderEditor.CommentaryNode;69;-4280.64,15.73622;Float;False;2859.746;1059.213;Comment;23;42;19;17;20;18;34;37;9;27;41;32;31;39;36;26;40;38;43;85;87;86;88;89;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;89;-3758.017,673.9073;Float;False;Constant;_Vector0;Vector 0;19;0;Create;True;0;0;False;0;-0.2,0.25;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;36;-3760.386,539.7689;Float;False;Property;_AutoUVDirection;Auto-UV-Direction;9;0;Create;True;0;0;False;0;0.24,0.24;-0.2,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;43;-4157.074,604.27;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;0.1;0.019;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;40;-3783.463,786.1494;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-3526.051,648.1108;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;26;-3723.947,439.3663;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-3389.864,752.4356;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-3545.829,923.5072;Float;False;Property;_TilingU;Tiling-U;7;0;Create;True;0;0;False;0;1;0.02;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-3549.829,1002.507;Float;False;Property;_TilingV;Tiling-V;8;0;Create;True;0;0;False;0;1;0.28;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-3383.505,483.7405;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;68;-1354.318,-596.8051;Float;False;1489.421;642.213;Comment;6;15;16;5;14;6;4;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;-3182.076,923.2327;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;41;-3199.474,749.1481;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;32;-3167.853,465.2303;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;6;-1135.579,-259.634;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1208.258,-507.3714;Float;True;Property;_MaskMap;MaskMap;5;0;Create;True;0;0;False;0;None;39b88e96eba91f74dbfebf2a93775bdb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-2946.42,447.9039;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-2940.341,696.4764;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-667.5511,-400.8332;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1199.23,-70.29508;Float;False;Property;_Alpha;Alpha;6;0;Create;True;0;0;False;0;2;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-2618.511,305.9779;Float;True;Property;_DistortMap;DistortMap;1;0;Create;True;0;0;False;0;None;7abc32cbcbe367844ac624c77a525a2d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;-2586.677,780.6526;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;226eb2b35b741624e88de8f5cec0e4d6;True;0;False;white;Auto;False;Instance;9;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;67;-1237.14,-1561.435;Float;False;1777.752;850.882;Comment;14;52;51;50;65;48;63;64;62;45;47;46;61;59;60;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-384.3725,-386.9268;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1070.55,-1263.726;Float;False;Property;_DisFrePower;Dis-Fre-Power;10;0;Create;True;0;0;False;0;6.318185;30;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-2187.657,402.6797;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-2201.244,637.7996;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-181.3613,478.4606;Float;False;Property;_Distrot;Distrot;3;0;Create;True;0;0;False;0;0.1;0.1;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1054.241,-1361.792;Float;False;Property;_DisFreScale;Dis-Fre-Scale;11;0;Create;True;0;0;False;0;1.430902;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;15;-137.9535,-316.8753;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1927.669,476.1159;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2009.451,730.3741;Float;False;Property;_DistortOffset;DistortOffset;4;0;Create;True;0;0;False;0;-0.8;1.02;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;7;266.6228,39.59514;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;45;-685.4067,-1406.355;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;178.7278,370.3156;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1665.4,543.5513;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;23;421.3514,353.8753;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;538.0176,69.27252;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;48;-236.0858,-1508.449;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2;712.3845,178.6278;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;51;42.09185,-1272.55;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;50;1.211293,-1481.215;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;55;935.0762,-6.13446;Float;False;Property;_FreColor;Fre-Color;13;1;[HDR];Create;True;0;0;False;0;1,1,1,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;52;235.4957,-1376.33;Float;False;Property;_DisFreinvert;Dis-Fre-invert;12;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;1;990.5162,193.6397;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;65;239.6714,-1014.969;Float;False;Property;_ColorFreinvert;Color-Fre-invert;16;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;64;25.38695,-1121.855;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;63;8.267488,-930.1895;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;62;-231.9103,-1147.089;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1050.065,-1000.432;Float;False;Property;_ColorFreScale;Color-Fre-Scale;15;0;Create;True;0;0;False;0;1.430902;1.36;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1065.375,-903.3665;Float;False;Property;_ColorFrePower;Color-Fre-Power;14;0;Create;True;0;0;False;0;6.318185;15.6;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;70;-926.708,811.3486;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;61;-681.2311,-1044.995;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;82;739.3874,686.1719;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-516.522,944.585;Float;False;Property;_Depth;Depth;18;0;Create;True;0;0;False;0;0;0;0;0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;1510.102,75.2532;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;72;-338.0485,813.1019;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;580.2,856.6166;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3.23672,1011.166;Float;False;Property;_Fogalpha;Fogalpha;17;0;Create;True;0;0;False;0;0;0;0;12;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;38;-3812.617,851.8751;Float;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;-0.24,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ClampOpNode;77;263.385,852.6637;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;76;-201.7654,951.334;Float;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0.1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-571.2808,1049.42;Float;False;Property;_Float0;Float 0;19;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;71;-640.665,793.845;Float;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;40.07932,832.7134;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;73;-156.2322,823.8235;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;80;371.085,972.6637;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;1269.491,-70.61674;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;1346.213,-431.9392;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1927.146,-104.2878;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;E3DEffect/AirDistort/AutoNoise-Mask-Fre-Dir;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Overlay;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;43;0
WireConnection;88;0;36;0
WireConnection;88;1;89;0
WireConnection;26;0;43;0
WireConnection;39;0;40;0
WireConnection;39;1;88;0
WireConnection;31;0;26;0
WireConnection;31;1;36;0
WireConnection;85;0;87;0
WireConnection;85;1;86;0
WireConnection;41;0;39;0
WireConnection;32;0;31;0
WireConnection;27;0;85;0
WireConnection;27;1;32;0
WireConnection;37;0;85;0
WireConnection;37;1;41;0
WireConnection;5;0;4;0
WireConnection;5;1;4;4
WireConnection;5;2;6;4
WireConnection;9;1;27;0
WireConnection;34;1;37;0
WireConnection;16;0;5;0
WireConnection;16;1;14;0
WireConnection;18;0;9;1
WireConnection;18;1;34;1
WireConnection;20;0;9;2
WireConnection;20;1;34;1
WireConnection;15;0;16;0
WireConnection;17;0;18;0
WireConnection;17;1;20;0
WireConnection;45;2;46;0
WireConnection;45;3;47;0
WireConnection;22;0;15;0
WireConnection;22;1;3;0
WireConnection;42;0;17;0
WireConnection;42;1;19;0
WireConnection;23;0;22;0
WireConnection;8;0;7;1
WireConnection;8;1;7;2
WireConnection;48;0;45;0
WireConnection;2;0;8;0
WireConnection;2;1;42;0
WireConnection;2;2;23;0
WireConnection;51;0;45;0
WireConnection;50;0;48;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;1;0;2;0
WireConnection;65;0;64;0
WireConnection;65;1;63;0
WireConnection;64;0;62;0
WireConnection;63;0;61;0
WireConnection;62;0;61;0
WireConnection;61;2;59;0
WireConnection;61;3;60;0
WireConnection;82;0;79;0
WireConnection;57;0;55;0
WireConnection;57;1;1;0
WireConnection;72;0;71;0
WireConnection;72;1;70;4
WireConnection;79;0;77;0
WireConnection;79;1;80;4
WireConnection;77;0;74;0
WireConnection;77;2;78;0
WireConnection;76;0;75;0
WireConnection;76;1;83;0
WireConnection;71;0;70;0
WireConnection;74;0;73;0
WireConnection;74;1;76;0
WireConnection;73;0;72;0
WireConnection;66;0;55;0
WireConnection;66;1;65;0
WireConnection;53;0;52;0
WireConnection;53;1;15;0
WireConnection;0;2;57;0
WireConnection;0;9;53;0
ASEEND*/
//CHKSM=E89561E4BD813EA8A4C44F1FA136CA812E79DFCB