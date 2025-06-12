Shader "Hovl/Particles/SwordSlash"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_EmissionTex("EmissionTex", 2D) = "white" {}
		_Opacity("Opacity", Float) = 20
		_Dissolve("Dissolve", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Emission("Emission", Float) = 5
		_Remap("Remap", Vector) = (-2,1,0,0)
		_AddColor("Add Color", Color) = (0,0,0,0)
		_Desaturation("Desaturation", Float) = 0
		[Toggle]_Usesmoothdissolve("Use smooth dissolve", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0

	}

	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 ase_texcoord1 : TEXCOORD1;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
							
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform float _InvFade;
				uniform float4 _AddColor;
				uniform float _Emission;
				uniform sampler2D _EmissionTex;
				uniform float4 _EmissionTex_ST;
				uniform sampler2D _MainTexture;
				uniform float4 _MainTexture_ST;
				uniform float _Desaturation;
				uniform float2 _Remap;
				uniform float4 _SpeedMainTexUVNoiseZW;
				uniform float _Opacity;
				uniform float _Usesmoothdissolve;
				uniform sampler2D _Dissolve;
				uniform float4 _Dissolve_ST;
				uniform fixed _Usedepth;
				
				float3 HSVToRGB( float3 c )
				{
					float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
					float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
					return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
				}
				
				float3 RGBToHSV(float3 c)
				{
					float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
					float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
					float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
					float d = q.x - min( q.w, q.y );
					float e = 1.0e-10;
					return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
				}

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3 = v.ase_texcoord1;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate ((sceneZ-partZ) / _InvFade);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 uv_EmissionTex = i.texcoord.xy * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
					float3 hsvTorgb46 = RGBToHSV( tex2D( _EmissionTex, uv_EmissionTex ).rgb );
					float4 uv2s4_MainTexture = i.ase_texcoord3;
					uv2s4_MainTexture.xy = i.ase_texcoord3.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float3 hsvTorgb47 = HSVToRGB( float3(( hsvTorgb46.x + uv2s4_MainTexture.z ),hsvTorgb46.y,hsvTorgb46.z) );
					float3 desaturateInitialColor42 = hsvTorgb47;
					float desaturateDot42 = dot( desaturateInitialColor42, float3( 0.299, 0.587, 0.114 ));
					float3 desaturateVar42 = lerp( desaturateInitialColor42, desaturateDot42.xxx, _Desaturation );
					float2 _Vector2 = float2(0,1);
					float3 temp_cast_1 = (_Vector2.x).xxx;
					float3 temp_cast_2 = (_Vector2.y).xxx;
					float3 temp_cast_3 = (_Remap.x).xxx;
					float3 temp_cast_4 = (_Remap.y).xxx;
					float3 clampResult37 = clamp( (temp_cast_3 + (desaturateVar42 - temp_cast_1) * (temp_cast_4 - temp_cast_3) / (temp_cast_2 - temp_cast_1)) , float3( 0,0,0 ) , float3( 1,1,1 ) );
					float2 appendResult20 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
					float2 panner31 = ( 1.0 * _Time.y * appendResult20 + uv2s4_MainTexture.xy);
					float clampResult6 = clamp( ( tex2D( _MainTexture, panner31 ).a * _Opacity ) , 0.0 , 1.0 );
					float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
					float4 uvs4_Dissolve = i.texcoord;
					uvs4_Dissolve.xy = i.texcoord.xy * _Dissolve_ST.xy + _Dissolve_ST.zw;
					float2 panner9 = ( 1.0 * _Time.y * appendResult21 + uvs4_Dissolve.xy);
					float2 break55 = panner9;
					float2 appendResult53 = (float2(break55.x , ( uv2s4_MainTexture.w + break55.y )));
					float4 tex2DNode22 = tex2D( _Dissolve, appendResult53 );
					float Step27 = step( ( 1.0 - uvs4_Dissolve.x ) , uvs4_Dissolve.w );
					float W18 = uvs4_Dissolve.z;
					float3 _Vector1 = float3(0.3,0,1);
					float ifLocalVar23 = 0;
					if( tex2DNode22.r >= ( Step27 * (( _Usesmoothdissolve )?( 1.0 ):( W18 )) ) )
					ifLocalVar23 = _Vector1.y;
					else
					ifLocalVar23 = _Vector1.z;
					float4 appendResult3 = (float4(( ( _AddColor * i.color ) + ( _Emission * float4( clampResult37 , 0.0 ) * i.color ) ).rgb , ( i.color.a * clampResult6 * (( _Usesmoothdissolve )?( ( saturate( ( ( tex2DNode22.r + 0.5 ) - W18 ) ) * ifLocalVar23 ) ):( ifLocalVar23 )) )));
					
					fixed4 col = appendResult3;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}	
}
/*ASEBEGIN
Version=18900
7;6;1906;1013;2588.763;-178.937;1.317653;True;False
Node;AmplifyShaderEditor.Vector4Node;19;-3359.506,763.2278;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-2910.647,800.8611;Inherit;False;0;22;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2822.961,959.7175;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;9;-2544.035,953.4323;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-1994.317,122.2001;Inherit;False;1;2;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;55;-2260.898,934.7292;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;36;-2194.33,-590.8586;Inherit;True;Property;_EmissionTex;EmissionTex;1;0;Create;True;0;0;0;False;0;False;-1;776495090b22ebc49af55b62923f4a68;4de1419ac14d16d44b9c82b76ea6d884;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1948.518,865.0703;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;59;-2526.706,867.1593;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;57;-2378.033,869.1517;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2582.555,794.6116;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;46;-1880.505,-584.6264;Float;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;53;-1762.071,928.4369;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-2257.507,863.3974;Float;False;Step;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-1618.658,902.3354;Inherit;True;Property;_Dissolve;Dissolve;3;0;Create;True;0;0;0;False;0;False;-1;1eae6143ce30e2849b37136524e197f0;1eae6143ce30e2849b37136524e197f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;26;-1557.054,1097.294;Inherit;False;18;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1593.059,-462.6141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-2786.74,711.4504;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1248.539,655.66;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;62;-1391.835,1083.89;Inherit;False;Property;_Usesmoothdissolve;Use smooth dissolve;10;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1393.475,-288.961;Float;False;Property;_Desaturation;Desaturation;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;47;-1450.51,-559.5951;Float;True;3;0;FLOAT;1.13;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;29;-1336.7,984.6516;Inherit;False;27;Step;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;38;-1164.232,-37.10035;Float;False;Property;_Remap;Remap;6;0;Create;True;0;0;0;False;0;False;-2,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;31;-1294.28,130.0098;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DesaturateOpNode;42;-1171.636,-302.3656;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-1053.539,655.66;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;25;-1296.803,1179.565;Float;False;Constant;_Vector1;Vector 1;4;0;Create;True;0;0;0;False;0;False;0.3,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1162.574,987.1012;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;50;-1162.205,-151.7991;Float;False;Constant;_Vector2;Vector 2;10;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCRemapNode;34;-914.0506,-103.6616;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ConditionalIfNode;23;-978.4097,931.5033;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1026.006,103.3516;Inherit;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;0;False;0;False;-1;4de1419ac14d16d44b9c82b76ea6d884;4de1419ac14d16d44b9c82b76ea6d884;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;66;-845.2965,655.7073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-880.473,294.4927;Float;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-703.2388,201.5004;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-718.1186,-174.7386;Float;False;Property;_Emission;Emission;5;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;37;-717.3522,-102.925;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;39;-778.4326,-338.6488;Float;False;Property;_AddColor;Add Color;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;7;-737.2988,18.57782;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-710.9966,782.184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;64;-546.4812,810.9857;Inherit;False;Property;_Usesmoothdissolve;Use smooth dissolve;10;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;6;-552.2924,202.0797;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-534.1503,-129.7955;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-531.9833,-265.6357;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-383.782,-246.1724;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-346.0773,109.456;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1915.164,-341.3013;Float;False;Property;_HUE;HUE;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-190.1702,-2.897369;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;True;-1;2;;0;7;Hovl/Particles/SwordSlash;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;21;0;19;3
WireConnection;21;1;19;4
WireConnection;9;0;10;0
WireConnection;9;2;21;0
WireConnection;55;0;9;0
WireConnection;51;0;30;4
WireConnection;51;1;55;1
WireConnection;59;0;10;1
WireConnection;57;0;59;0
WireConnection;57;1;10;4
WireConnection;18;0;10;3
WireConnection;46;0;36;0
WireConnection;53;0;55;0
WireConnection;53;1;51;0
WireConnection;27;0;57;0
WireConnection;22;1;53;0
WireConnection;48;0;46;1
WireConnection;48;1;30;3
WireConnection;20;0;19;1
WireConnection;20;1;19;2
WireConnection;60;0;22;1
WireConnection;62;0;26;0
WireConnection;47;0;48;0
WireConnection;47;1;46;2
WireConnection;47;2;46;3
WireConnection;31;0;30;0
WireConnection;31;2;20;0
WireConnection;42;0;47;0
WireConnection;42;1;43;0
WireConnection;61;0;60;0
WireConnection;61;1;26;0
WireConnection;56;0;29;0
WireConnection;56;1;62;0
WireConnection;34;0;42;0
WireConnection;34;1;50;1
WireConnection;34;2;50;2
WireConnection;34;3;38;1
WireConnection;34;4;38;2
WireConnection;23;0;22;1
WireConnection;23;1;56;0
WireConnection;23;2;25;2
WireConnection;23;3;25;2
WireConnection;23;4;25;3
WireConnection;2;1;31;0
WireConnection;66;0;61;0
WireConnection;4;0;2;4
WireConnection;4;1;5;0
WireConnection;37;0;34;0
WireConnection;67;0;66;0
WireConnection;67;1;23;0
WireConnection;64;0;23;0
WireConnection;64;1;67;0
WireConnection;6;0;4;0
WireConnection;32;0;33;0
WireConnection;32;1;37;0
WireConnection;32;2;7;0
WireConnection;41;0;39;0
WireConnection;41;1;7;0
WireConnection;40;0;41;0
WireConnection;40;1;32;0
WireConnection;8;0;7;4
WireConnection;8;1;6;0
WireConnection;8;2;64;0
WireConnection;3;0;40;0
WireConnection;3;3;8;0
WireConnection;1;0;3;0
ASEEND*/
//CHKSM=A1692E1F9FDFC4332F09E5030E719528DF2810BF