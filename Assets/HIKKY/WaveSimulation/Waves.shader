Shader "Custom/Waves"
{
    Properties
	{
		_Albedo ("Albedo", Color) = (1, 1, 1, 1)
		
		[Header(Wave1)]
		[MaterialToggle] _Active1 ("Active1", Float) = 1
		_Direction1X ("Direction1X", Range(-1, 1)) = 1
		_Direction1Z ("Direction1Z", Range(-1, 1)) = 0
		_Amplitude1 ("Amplitude1", Float) = 0.1
		_WaveLength1 ("WaveLength1", Float) = 3
		_Speed1 ("Speed1", Float) = 0.2
		_QRatio1 ("Q Ratio1", Range(0, 1)) = 0.25
		
		[Header(Wave2)]
		[MaterialToggle] _Active2 ("Active2", Float) = 1
		_Direction2X ("Direction2X", Range(-1, 1)) = 0.8
		_Direction2Z ("Direction2Z", Range(-1, 1)) = 0.3
		_Amplitude2 ("Amplitude2", Float) = 0.07
		_WaveLength2 ("WaveLength2", Float) = 1.5
		_Speed2 ("Speed2", Float) = 0.5
		_QRatio2 ("Q Ratio2", Range(0, 1)) = 0.34
		
		[Header(Wave3)]
		[MaterialToggle] _Active3 ("Active3", Float) = 1
		_Direction3X ("Direction3X", Range(-1, 1)) = 0.1
		_Direction3Z ("Direction3Z", Range(-1, 1)) = 0.08
		_Amplitude3 ("Amplitude3", Float) = 0.04
		_WaveLength3 ("WaveLength3", Float) = 0.9
		_Speed3 ("Speed3", Float) = 0.6
		_QRatio3 ("Q Ratio3", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
			"UniversalMaterialType" = "Lit"
			"IgnoreProjector" = "True"
		}
		LOD 300

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
		float4 _Albedo;

		float _Active1;
		float _Direction1X;
		float _Direction1Z;
		float _Amplitude1;
		float _WaveLength1;
		float _Speed1;
		float _QRatio1;

		float _Active2;
		float _Direction2X;
		float _Direction2Z;
		float _Amplitude2;
		float _WaveLength2;
		float _Speed2;
		float _QRatio2;

		float _Active3;
		float _Direction3X;
		float _Direction3Z;
		float _Amplitude3;
		float _WaveLength3;
		float _Speed3;
		float _QRatio3;
		CBUFFER_END

		ENDHLSL

		Pass
		{
			Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}

			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			ZWrite On
			Cull Back

			HLSLPROGRAM
			#pragma vertex ProcessVertex
			#pragma fragment ProcessFragment
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
				float4 normalOS : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : NORMAL;
				float3 positionWS : TEXCOORD0;
			};

			struct Param
			{
				float Q;
				float A;
				float WA;
				float sinTheta;
				float cosTheta;
				float2 D;
			};

			Param InitParam(const float2 xy, const half QRatio, const half A, const float2 D, const float t, const float L, const float S)
			{
				Param param = (Param)0;

				const float w = 2 * PI / L;
				param.Q = (1 / (w * A)) * QRatio;
				param.A = A;
				param.WA = w * A;
				param.D = normalize(D);

				const float phi = S * w;
				const float theta = w * dot(param.D, xy) + phi * t;
				param.sinTheta = sin(theta);
				param.cosTheta = cos(theta);

				return param;
			}

			/// <summary>
			/// 位置ずれ
			/// </summary>
			float3 CalculateSumTermOfShiftPosition(const in Param param)
			{
				return float3(
					param.Q * param.A * param.D.x * param.cosTheta,
					param.Q * param.A * param.D.y * param.cosTheta,
					param.A * param.sinTheta
				);
			}

			float3 CalculateSumTermOfNormal(const in Param param)
			{
				return float3(
					param.D.x * param.WA * param.cosTheta,
					param.D.y * param.WA * param.cosTheta,
					param.Q * param.WA * param.sinTheta
				);
			}

			float3 CalculateSumTermOfBitangent(const in Param param)
			{
				return float3(
					param.Q * param.D.x * param.D.x * param.WA * param.sinTheta,
					param.Q * param.D.x * param.D.y * param.WA * param.sinTheta,
					param.D.x * param.WA * param.cosTheta
				);
			}

			float3 CalculateSumTermOfTangent(const in Param param)
			{
				return float3(
					param.Q * param.D.x * param.D.y * param.WA * param.sinTheta,
					param.Q * param.D.y * param.D.y * param.WA * param.sinTheta,
					param.D.y * param.WA * param.cosTheta
				);
			}

			float3 P(in float2 xy, const in Param param1, const in Param param2, const in Param param3)
			{
				return float3(xy, 0)
					+ CalculateSumTermOfShiftPosition(param1) * _Active1
					+ CalculateSumTermOfShiftPosition(param2) * _Active2
					+ CalculateSumTermOfShiftPosition(param3) * _Active3;
			}

			float3 N(const in Param param1, const in Param param2, const in Param param3)
			{
				const float3 bitangentTerm1 = CalculateSumTermOfBitangent(param1) * _Active1;
				const float3 bitangentTerm2 = CalculateSumTermOfBitangent(param2) * _Active2;
				const float3 bitangentTerm3 = CalculateSumTermOfBitangent(param3) * _Active3;
				const float3 bitangent = normalize(float3(
					1 - (bitangentTerm1.x + bitangentTerm2.x + bitangentTerm3.x),
					- (bitangentTerm1.y + bitangentTerm2.y + bitangentTerm3.y),
					(bitangentTerm1.z + bitangentTerm2.z + bitangentTerm3.z)
				));
				const float3 tangentTerm1 = CalculateSumTermOfTangent(param1) * _Active1;
				const float3 tangentTerm2 = CalculateSumTermOfTangent(param2) * _Active2;
				const float3 tangentTerm3 = CalculateSumTermOfTangent(param3) * _Active3;
				const float3 tangent = normalize(float3(
					- (tangentTerm1.x + tangentTerm2.x + tangentTerm3.x),
					1 - (tangentTerm1.y + tangentTerm2.y + tangentTerm3.y),
					(tangentTerm1.z + tangentTerm2.z + tangentTerm3.z)
				));
				// 法線の式だけから計算するより接線と従接線の外積で法線計算した方が綺麗になる模様
				return normalize(cross(bitangent, tangent));
			}

			Varyings ProcessVertex(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

				// Gemsに合わせて記号は水平平面をxyとしている。実際はxz平面なので渡すのはxz。
				const float2 xy = positionWS.xz;
				const float t = _Time.y;
				const Param param1 = InitParam(xy, _QRatio1, _Amplitude1, float2(_Direction1X, _Direction1Z), t, _WaveLength1, _Speed1);
				const Param param2 = InitParam(xy, _QRatio2, _Amplitude2, float2(_Direction2X, _Direction2Z), t, _WaveLength2, _Speed2);
				const Param param3 = InitParam(xy, _QRatio3, _Amplitude3, float2(_Direction3X, _Direction3Z), t, _WaveLength3, _Speed3);

				Varyings output = (Varyings)0;
				output.positionWS = P(xy, param1, param2, param3).xzy;
				output.positionCS = TransformWorldToHClip(output.positionWS);
				// output.normalWS = N(param1, param2, param3).xzy;
				return output;
			}

			half4 ProcessFragment(Varyings input) : SV_Target
			{
				// 頂点シェーダーで計算した法線を使う
				// const half3 surfaceNormal = normalize(input.normalWS);

				// ピクセルごとの座標で計算した法線を使う (頂点シェーダー計算より綺麗だけど重くなる)
				const float2 xy = input.positionWS.xz;
				const float t = _Time.y;
				const Param param1 = InitParam(xy, _QRatio1, _Amplitude1, float2(_Direction1X, _Direction1Z), t, _WaveLength1, _Speed1);
				const Param param2 = InitParam(xy, _QRatio2, _Amplitude2, float2(_Direction2X, _Direction2Z), t, _WaveLength2, _Speed2);
				const Param param3 = InitParam(xy, _QRatio3, _Amplitude3, float2(_Direction3X, _Direction3Z), t, _WaveLength3, _Speed3);
				const half3 surfaceNormal = normalize(N(param1, param2, param3).xzy);

				const float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				const Light light = GetMainLight(shadowCoord);
				const float NoL = saturate(dot(surfaceNormal, light.direction));
				const half3 diffuse = _Albedo.rgb * light.color * NoL;
				return half4(diffuse, 1);
			}
			ENDHLSL
		}
	}
	FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
