Shader "Hovl/Particles/Blend_TwoSides"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Float) = 0.5
        _MainTex("Main Tex", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _FrontFacesColor("Front Faces Color", Color) = (0,0.2313726,1,1)
        _BackFacesColor("Back Faces Color", Color) = (0.1098039,0.4235294,1,1)
        _Emission("Emission", Float) = 2
        [Toggle]_UseFresnel("Use Fresnel?", Float) = 1
        [Toggle]_SeparateFresnel("SeparateFresnel", Float) = 0
        _SeparateEmission("Separate Emission", Float) = 2
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _Fresnel("Fresnel", Float) = 1
        _FresnelEmission("Fresnel Emission", Float) = 1
        [Toggle]_UseCustomData("Use Custom Data?", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="TransparentCutout"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma shader_feature _USEFRESNEL_ON
            #pragma shader_feature _SEPARATEFRESNEL_ON
            #pragma shader_feature _USECUSTOMDATA_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float4 uv2        : TEXCOORD1; // xy noise uv, z custom data, w noise offset
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD2;
                float4 uv2        : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Cutoff;
                float4 _MainTex_ST;
                float4 _Mask_ST;
                float4 _Noise_ST;
                float4 _FrontFacesColor;
                float4 _BackFacesColor;
                float4 _FresnelColor;
                float _Emission;
                float _Fresnel;
                float _FresnelEmission;
                float _SeparateEmission;
                float4 _SpeedMainTexUVNoiseZW;
                float _UseFresnel;
                float _SeparateFresnel;
                float _UseCustomData;
            CBUFFER_END

            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_Mask);     SAMPLER(sampler_Mask);
            TEXTURE2D(_Noise);    SAMPLER(sampler_Noise);

            Varyings Vert (Attributes v)
            {
                Varyings o;
                VertexPositionInputs vp = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = vp.positionCS;
                o.positionWS = vp.positionWS;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                // Prepare second uv (noise/mask)
                float2 uvMask = v.uv * _Mask_ST.xy + _Mask_ST.zw;
                float2 uvNoise = v.uv2.xy * _Noise_ST.xy + _Noise_ST.zw;
                o.uv2 = float4(uvNoise, v.uv2.z, v.uv2.w);
                return o;
            }

            float4 ComputeFrontColor(float fresnelTerm)
            {
                float4 baseFront = _FrontFacesColor;
                if (_UseFresnel > 0.5)
                {
                    float4 fresnelMix = (baseFront * (1.0 - fresnelTerm)) + (_FresnelEmission * _FresnelColor * fresnelTerm);
                    return fresnelMix;
                }
                return baseFront;
            }

            float Fresnel(float3 n, float3 v, float p)
            {
                float ndotv = saturate(dot(n, v));
                return pow(1.0 - ndotv, p);
            }

            float4 Frag (Varyings i
                #if defined(SHADER_STAGE_FRAGMENT)
                , float faceSign : VFACE
                #endif
            ) : SV_Target
            {
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(i.positionWS));
                float3 nWS = normalize(i.normalWS);

                // Fresnel
                float fresnelTerm = Fresnel(nWS, viewDirWS, _Fresnel);

                // Front/back selection (replicating original sign(dot) mapping)
                float facingVal = sign(dot(nWS, viewDirWS));
                float faceLerp = saturate( (facingVal + 1.0) * 0.5 ); // front=1 -> 1, back=-1 -> 0
                float4 frontCol = ComputeFrontColor(fresnelTerm);
                float4 backCol  = _BackFacesColor;
                float4 baseCol = lerp(backCol, frontCol, faceLerp);

                // Animated main texture
                float2 uvMain = i.uv + _SpeedMainTexUVNoiseZW.xy * _Time.y;
                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvMain);

                // Optional separate fresnel emission path
                float4 emissionColA = baseCol * _Emission * i.color * i.color.a * mainTex;
                float4 emissionColB = (baseCol + (_FresnelColor * mainTex * _SeparateEmission)) * _Emission * i.color * i.color.a;
                float4 finalEmission = lerp(emissionColA, emissionColB, _SeparateFresnel);

                // Mask & Noise alpha clip
                float2 uvMask = i.uv * _Mask_ST.xy + _Mask_ST.zw;
                float2 uvNoise = i.uv2.xy + _SpeedMainTexUVNoiseZW.zw * _Time.y + i.uv2.w;
                float4 maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uvMask);
                float4 noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, uvNoise);
                float customFactor = lerp(1.0, i.uv2.z, _UseCustomData);
                float alphaSample = (maskTex.r * noiseTex.r * customFactor);

                clip(alphaSample - _Cutoff);

                float4 outColor = float4(finalEmission.rgb, 1.0);
                // Fog (optional)
                #if defined(FOG_ANY)
                    float fogFactor = ComputeFogFactor(i.positionCS.z);
                    outColor.rgb = MixFog(outColor.rgb, fogFactor);
                #endif
                return outColor;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}