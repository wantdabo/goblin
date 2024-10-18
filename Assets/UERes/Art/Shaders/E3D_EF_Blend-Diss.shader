// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "E3DEffect/C3/Add-Diss"
{
	Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_BaseMaskMap("BaseMaskMap", 2D) = "white" {}
		[HDR]_BaseColor("BaseColor", Color) = (1,1,1,1)
		_FaceBaseColor("FaceBaseColor", Range( 0 , 1)) = 1
		_NoiseMap("NoiseMap", 2D) = "white" {}
		[HDR]_EdgeColor("EdgeColor", Color) = (1,1,1,1)
		_FaceEdgeColor("FaceEdgeColor", Range( 0 , 1)) = 1
		_EdgeIntensity("EdgeIntensity", Range( 0 , 1)) = 0.2
		_Diss("Diss", Range( 0 , 1.01)) = 1.01
		_Glow("Glow", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma exclude_renderers xbox360 xboxone ps4 psp2 n3ds wiiu 
		#pragma surface surf Unlit alpha:fade keepalpha noshadow nolightmap  nodynlightmap nodirlightmap noforwardadd 
		struct Input
		{
			half ASEVFace : VFACE;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _BaseColor;
		uniform float _FaceBaseColor;
		uniform sampler2D _BaseMap;
		uniform float4 _BaseMap_ST;
		uniform sampler2D _NoiseMap;
		uniform float4 _NoiseMap_ST;
		uniform float _Diss;
		uniform float _Glow;
		uniform float4 _EdgeColor;
		uniform float _FaceEdgeColor;
		uniform float _EdgeIntensity;
		uniform sampler2D _BaseMaskMap;
		uniform float4 _BaseMaskMap_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 switchResult91 = (((i.ASEVFace>0)?(_BaseColor):(( _BaseColor * _FaceBaseColor ))));
			float2 uv_BaseMap = i.uv_texcoord * _BaseMap_ST.xy + _BaseMap_ST.zw;
			float4 tex2DNode1 = tex2D( _BaseMap, uv_BaseMap );
			float2 uv_NoiseMap = i.uv_texcoord * _NoiseMap_ST.xy + _NoiseMap_ST.zw;
			float4 tex2DNode2 = tex2D( _NoiseMap, uv_NoiseMap );
			float blendOpSrc71 = ( tex2DNode2.r * tex2DNode2.a );
			float blendOpDest71 = ( 1.0 - ( _Diss * i.vertexColor.a ) );
			float temp_output_71_0 = ( saturate( ( 1.0 - ( ( 1.0 - blendOpDest71) / blendOpSrc71) ) ));
			float temp_output_3_0 = step( temp_output_71_0 , 0.0 );
			float4 switchResult93 = (((i.ASEVFace>0)?(_EdgeColor):(( _EdgeColor * _FaceEdgeColor ))));
			float temp_output_69_0 = ( step( temp_output_71_0 , _EdgeIntensity ) - temp_output_3_0 );
			float2 uv_BaseMaskMap = i.uv_texcoord * _BaseMaskMap_ST.xy + _BaseMaskMap_ST.zw;
			float4 tex2DNode97 = tex2D( _BaseMaskMap, uv_BaseMaskMap );
			float temp_output_98_0 = ( ( tex2DNode97.r * tex2DNode97.a ) * ( tex2DNode1.r * tex2DNode1.a ) );
			o.Emission = ( ( switchResult91 * tex2DNode1 * i.vertexColor * temp_output_3_0 * _Glow ) + ( switchResult93 * temp_output_69_0 * i.vertexColor * temp_output_98_0 * _Glow ) ).rgb;
			o.Alpha = ( ( temp_output_69_0 + temp_output_3_0 ) * _Glow * temp_output_98_0 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
7;29;1906;1004;1840.058;914.1414;1.825229;True;False
Node;AmplifyShaderEditor.VertexColorNode;88;-1657.945,277.3997;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1666.615,199.9089;Float;False;Property;_Diss;Diss;8;0;Create;True;0;0;False;0;1.01;0.525;0;1.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1315.835,229.2719;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1695.726,11.58333;Float;True;Property;_NoiseMap;NoiseMap;4;0;Create;True;0;0;False;0;None;906cbb19528dcd44691c09da6f8a3019;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;101;-1172.426,136.2349;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-1156.713,34.48979;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;50;-936.8892,-662.1435;Float;False;Property;_BaseColor;BaseColor;2;1;[HDR];Create;True;0;0;False;0;1,1,1,1;0.6617648,1.361258,1.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-959.7862,-246.3657;Float;False;Property;_EdgeColor;EdgeColor;5;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1.014706,1.94087,3.000001,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1687.701,-195.5728;Float;True;Property;_BaseMap;BaseMap;0;0;Create;True;0;0;False;0;None;5879cd2a9fd90b749a306578c9370245;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;92;-972.1776,-475.9919;Float;False;Property;_FaceBaseColor;FaceBaseColor;3;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-982.7137,-66.97407;Float;False;Property;_FaceEdgeColor;FaceEdgeColor;6;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-862.7641,340.3136;Float;False;Constant;_0;0;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1001.485,251.0037;Float;False;Property;_EdgeIntensity;EdgeIntensity;7;0;Create;True;0;0;False;0;0.2;0.904;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;71;-977.1528,26.86603;Float;True;ColorBurn;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;97;-1683.249,-385.123;Float;True;Property;_BaseMaskMap;BaseMaskMap;1;0;Create;True;0;0;False;0;None;fb483d4f3ba4ccf45b30c59096632327;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-660.425,-491.1668;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-673.3249,-151.1487;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;59;-628.4166,78.58192;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;3;-629.637,177.2129;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-1356.152,-317.6967;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;-1359.539,-157.077;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;93;-497.117,-217.6464;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;-413.458,56.26053;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;91;-455.1021,-513.7931;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-549.5982,287.1422;Float;False;Property;_Glow;Glow;9;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1160.003,-180.8063;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-223.1416,-462.5755;Float;True;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-229.7763,-200.2873;Float;True;5;5;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-232.0135,108.6477;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;242.3964,26.15308;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;85.16978,-269.8486;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;704.4276,-150.6121;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;E3DEffect/C3/Add-Diss;False;False;False;False;False;False;True;True;True;False;False;True;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;False;False;False;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;89;0;5;0
WireConnection;89;1;88;4
WireConnection;101;0;89;0
WireConnection;103;0;2;1
WireConnection;103;1;2;4
WireConnection;71;0;103;0
WireConnection;71;1;101;0
WireConnection;96;0;50;0
WireConnection;96;1;92;0
WireConnection;95;0;23;0
WireConnection;95;1;94;0
WireConnection;59;0;71;0
WireConnection;59;1;20;0
WireConnection;3;0;71;0
WireConnection;3;1;72;0
WireConnection;139;0;97;1
WireConnection;139;1;97;4
WireConnection;140;0;1;1
WireConnection;140;1;1;4
WireConnection;93;0;23;0
WireConnection;93;1;95;0
WireConnection;69;0;59;0
WireConnection;69;1;3;0
WireConnection;91;0;50;0
WireConnection;91;1;96;0
WireConnection;98;0;139;0
WireConnection;98;1;140;0
WireConnection;49;0;91;0
WireConnection;49;1;1;0
WireConnection;49;2;88;0
WireConnection;49;3;3;0
WireConnection;49;4;102;0
WireConnection;22;0;93;0
WireConnection;22;1;69;0
WireConnection;22;2;88;0
WireConnection;22;3;98;0
WireConnection;22;4;102;0
WireConnection;63;0;69;0
WireConnection;63;1;3;0
WireConnection;41;0;63;0
WireConnection;41;1;102;0
WireConnection;41;2;98;0
WireConnection;24;0;49;0
WireConnection;24;1;22;0
WireConnection;0;2;24;0
WireConnection;0;9;41;0
ASEEND*/
//CHKSM=BA5248AE05A718CB0BE8A2A2A84456EC5D683551