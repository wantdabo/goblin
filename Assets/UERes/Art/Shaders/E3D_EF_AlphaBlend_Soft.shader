// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "E3DEffect/C2/Blend-Soft"
{
	Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_BaseMaskMap("BaseMaskMap", 2D) = "white" {}
		[HDR]_BaseColor("BaseColor", Color) = (1,1,1,1)
		_NoiseMap("NoiseMap", 2D) = "white" {}
		_Diss("Diss", Range( 0 , 1)) = 1
		_Power("Power", Range( 0.5 , 50)) = 0.5
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
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _NoiseMap;
		uniform float4 _NoiseMap_ST;
		uniform float _Diss;
		uniform float _Power;
		uniform sampler2D _BaseMap;
		uniform float4 _BaseMap_ST;
		uniform sampler2D _BaseMaskMap;
		uniform float4 _BaseMaskMap_ST;
		uniform float4 _BaseColor;
		uniform float _Glow;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NoiseMap = i.uv_texcoord * _NoiseMap_ST.xy + _NoiseMap_ST.zw;
			float blendOpSrc16 = tex2D( _NoiseMap, uv_NoiseMap ).r;
			float blendOpDest16 = ( _Diss * i.vertexColor.a );
			float temp_output_27_0 = pow( ( saturate( ( blendOpDest16 / blendOpSrc16 ) )) , _Power );
			float2 uv_BaseMap = i.uv_texcoord * _BaseMap_ST.xy + _BaseMap_ST.zw;
			float4 tex2DNode39 = tex2D( _BaseMap, uv_BaseMap );
			float2 uv_BaseMaskMap = i.uv_texcoord * _BaseMaskMap_ST.xy + _BaseMaskMap_ST.zw;
			float temp_output_92_0 = ( tex2DNode39.a * tex2D( _BaseMaskMap, uv_BaseMaskMap ).r );
			o.Emission = ( ( ( 1.0 - temp_output_27_0 ) * temp_output_92_0 * i.vertexColor ) + ( tex2DNode39 * _BaseColor * i.vertexColor ) ).rgb;
			o.Alpha = ( temp_output_27_0 * temp_output_92_0 * _BaseColor.a * _Glow );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
1927;147;1906;1004;1908.366;1047.5;1.794095;True;False
Node;AmplifyShaderEditor.VertexColorNode;33;-1516.135,-157.0583;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1534.247,-465.7994;Float;False;Property;_Diss;Diss;4;0;Create;True;0;0;False;0;1;0.929;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1333.294,-790.0872;Float;True;Property;_NoiseMap;NoiseMap;3;0;Create;True;0;0;False;0;None;e5e4f8d6f0dfeca45a8fcc8174dfa252;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1206.626,-464.6401;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;16;-932.4313,-656.1973;Float;True;Divide;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-898.4233,-343.8718;Float;False;Property;_Power;Power;5;0;Create;True;0;0;False;0;0.5;0.5;0.5;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;-1172.631,95.33989;Float;True;Property;_BaseMaskMap;BaseMaskMap;1;0;Create;True;0;0;False;0;None;e5e4f8d6f0dfeca45a8fcc8174dfa252;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;39;-1186.146,-238.8986;Float;True;Property;_BaseMap;BaseMap;0;0;Create;True;0;0;False;0;None;6886505889fcfbc4fb38cc8feaba76ef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;27;-534.0601,-628.5351;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-651.8464,-4.771089;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;43;-621.1237,179.3719;Float;False;Property;_BaseColor;BaseColor;2;1;[HDR];Create;True;0;0;False;0;1,1,1,1;5.835001,2.388443,1.372941,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;30;41.81827,-609.4948;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-110.3874,474.8765;Float;False;Property;_Glow;Glow;6;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;75.74352,-172.673;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;431.6822,-362.436;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;672.4241,-264.1855;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;360.1761,65.8168;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;50;905.789,-286.0342;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;E3DEffect/C2/Blend-Soft;False;False;False;False;False;False;True;True;True;False;False;True;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;False;False;False;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;24;0
WireConnection;51;1;33;4
WireConnection;16;0;1;1
WireConnection;16;1;51;0
WireConnection;27;0;16;0
WireConnection;27;1;29;0
WireConnection;92;0;39;4
WireConnection;92;1;91;1
WireConnection;30;0;27;0
WireConnection;41;0;39;0
WireConnection;41;1;43;0
WireConnection;41;2;33;0
WireConnection;38;0;30;0
WireConnection;38;1;92;0
WireConnection;38;2;33;0
WireConnection;44;0;38;0
WireConnection;44;1;41;0
WireConnection;45;0;27;0
WireConnection;45;1;92;0
WireConnection;45;2;43;4
WireConnection;45;3;94;0
WireConnection;50;2;44;0
WireConnection;50;9;45;0
ASEEND*/
//CHKSM=EC1070883970FBA28DC353A3CFD9BB9081DEC2A3