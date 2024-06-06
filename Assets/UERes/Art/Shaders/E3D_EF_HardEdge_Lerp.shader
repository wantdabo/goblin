// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "E3DEffect/C3/Add-HardEdge-Lerp"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Base("Base", 2D) = "white" {}
		_Shape("Shape", 2D) = "white" {}
		[HDR]_BaseColor("Base-Color", Color) = (0,0,0,0)
		[HDR]_AddColor("Add-Color", Color) = (0,0,0,0)
		_BaseAlpha("BaseAlpha", Range( 0 , 1)) = 0.5411765
		_EdgeClip("EdgeClip", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _AddColor;
		uniform float4 _BaseColor;
		uniform sampler2D _Base;
		uniform float4 _Base_ST;
		uniform sampler2D _Shape;
		uniform float4 _Shape_ST;
		uniform float _BaseAlpha;
		uniform float _EdgeClip;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_Base = i.uv_texcoord * _Base_ST.xy + _Base_ST.zw;
			float4 tex2DNode14 = tex2D( _Base, uv0_Base );
			float2 uv0_Shape = i.uv_texcoord * _Shape_ST.xy + _Shape_ST.zw;
			float4 tex2DNode15 = tex2D( _Shape, uv0_Shape );
			float4 lerpResult21 = lerp( _AddColor , _BaseColor , ( tex2DNode14 * tex2DNode15 * i.vertexColor ));
			o.Emission = lerpResult21.rgb;
			o.Alpha = 1;
			clip( ( ( ( ( tex2DNode14 + _BaseAlpha ) * tex2DNode15 ) * _EdgeClip ) * i.vertexColor.a ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
1933;96;1906;1004;1538.428;916.0731;1.382784;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1957.404,-510.5973;Float;False;0;14;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1600.514,-278.2899;Float;False;Property;_BaseAlpha;BaseAlpha;5;0;Create;True;0;0;False;0;0.5411765;0.5411765;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1598.896,-550.1072;Float;True;Property;_Base;Base;1;0;Create;True;0;0;False;0;None;ccbfa8a432d319d46b5b62f2b2fb705d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1953.945,-105.3345;Float;False;0;15;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;-1641.983,-115.2863;Float;True;Property;_Shape;Shape;2;0;Create;True;0;0;False;0;None;a53a0681ad7b7d749bb73405efcc5f19;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1244.449,-371.0455;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1259.035,101.7422;Float;False;Property;_EdgeClip;EdgeClip;6;0;Create;True;0;0;False;0;0;1.57;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;5;-707.2003,148.2;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1034.54,-198.8895;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;8;-382.2581,-900.3436;Float;False;Property;_AddColor;Add-Color;4;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0.5034484,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-496.4014,-461.2546;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-808.0347,-78.25781;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;23;-403.8753,-698.4812;Float;False;Property;_BaseColor;Base-Color;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.7647059,0.8442191,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-389.0129,89.97817;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;21;-22.10386,-504.7611;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;601.2848,-251.3447;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;E3DEffect/C3/Add-HardEdge-Lerp;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;1;10;0
WireConnection;15;1;11;0
WireConnection;16;0;14;0
WireConnection;16;1;17;0
WireConnection;18;0;16;0
WireConnection;18;1;15;0
WireConnection;7;0;14;0
WireConnection;7;1;15;0
WireConnection;7;2;5;0
WireConnection;19;0;18;0
WireConnection;19;1;20;0
WireConnection;3;0;19;0
WireConnection;3;1;5;4
WireConnection;21;0;8;0
WireConnection;21;1;23;0
WireConnection;21;2;7;0
WireConnection;2;2;21;0
WireConnection;2;10;3;0
ASEEND*/
//CHKSM=9F6B91EABA38721FEAE9AACBD22C35650CFA2BB7