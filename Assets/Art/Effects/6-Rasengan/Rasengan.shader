// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rasengan"
{
	Properties
	{
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 5
		_NegBias("NegBias", Float) = 0
		_NegScale("NegScale", Float) = 1
		_NegPower("NegPower", Float) = 5
		_Texture2("Texture2", 2D) = "white" {}
		_Texture1("Texture1", 2D) = "white" {}
		_DepthDist("Depth Dist", Float) = 0
		_TextureSpeed2("Texture Speed 2", Vector) = (-4,8,0,0)
		_TextureSpeed1("Texture Speed 1", Vector) = (4,8,0,0)
		_DepthIntensity("Depth Intensity", Float) = 0
		_FallOff("Fall Off", Float) = 0
		_TextureIntensivty("Texture Intensivty", Float) = 1
		_TextureColor("Texture Color", Color) = (0.6603774,0.258544,0.258544,0)
		_NegFresnelColor("Neg Fresnel Color", Color) = (0.09620863,0.8460232,0.8867924,0)
		_Borderintensity("Border intensity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 viewDir;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float4 _TextureColor;
		uniform sampler2D _Texture1;
		uniform float2 _TextureSpeed1;
		uniform sampler2D _Texture2;
		uniform float2 _TextureSpeed2;
		uniform float _TextureIntensivty;
		uniform float4 _NegFresnelColor;
		uniform float _NegBias;
		uniform float _NegScale;
		uniform float _NegPower;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Borderintensity;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthDist;
		uniform float _DepthIntensity;
		uniform float _FallOff;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner431 = ( 1.0 * _Time.y * _TextureSpeed1 + i.uv_texcoord);
			float2 panner599 = ( 1.0 * _Time.y * _TextureSpeed2 + i.uv_texcoord);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float Vertex553 = saturate( ( ( ( 1.0 - ase_vertex3Pos.y ) * ( 1.0 - -ase_vertex3Pos.y ) ) - 0.65 ) );
			float4 Texture556 = saturate( ( ( 1.0 - tex2D( _Texture1, panner431 ) ) * ( 1.0 - tex2D( _Texture2, panner599 ).r ) * _TextureIntensivty * Vertex553 ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV367 = dot( ase_worldNormal, -i.viewDir );
			float fresnelNode367 = ( _NegBias + _NegScale * pow( 1.0 - fresnelNdotV367, _NegPower ) );
			float NegFresnel611 = fresnelNode367;
			float fresnelNdotV41 = dot( ase_worldNormal, i.viewDir );
			float fresnelNode41 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV41, _FresnelPower ) );
			float PosFresnel612 = fresnelNode41;
			float4 Emission577 = ( ( _TextureColor * Texture556 ) + ( _NegFresnelColor * NegFresnel611 ) + ( PosFresnel612 * _Borderintensity ) );
			o.Emission = saturate( Emission577 ).rgb;
			float Fresnel335 = ( NegFresnel611 * PosFresnel612 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth451 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth451 = abs( ( screenDepth451 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDist ) );
			float Depth461 = saturate( pow( ( saturate( ( 1.0 - distanceDepth451 ) ) * _DepthIntensity ) , _FallOff ) );
			o.Alpha = saturate( ( Texture556 + Fresnel335 + Depth461 ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
0;540;1587;459;2033.062;-77.97839;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;552;-1854.07,917.6529;Inherit;False;1137.775;298.0431;Vertex Mask;8;553;562;564;566;541;536;540;483;Vertex Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;483;-1836.09,964.4419;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;540;-1642.368,1059.63;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;536;-1591.202,958.2245;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;541;-1509.368,1060.63;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;555;-1806.074,-707.6678;Inherit;False;1486.463;579.9724;Texture Mask;15;558;556;563;481;468;601;482;442;600;599;431;447;598;597;435;Texture Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;566;-1352.689,993.7826;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;598;-1791.361,-273.4155;Inherit;False;Property;_TextureSpeed2;Texture Speed 2;9;0;Create;True;0;0;0;False;0;False;-4,8;-4,8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;597;-1788.936,-408.1796;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;447;-1798.723,-666.3278;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;564;-1207.695,1013.775;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;435;-1788.607,-543.6306;Inherit;False;Property;_TextureSpeed1;Texture Speed 1;10;0;Create;True;0;0;0;False;0;False;4,8;0,-8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;460;-1855.572,591.4557;Inherit;False;1334.254;262.7628;Depth Mask;10;461;459;457;455;458;456;454;453;451;452;Depth Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;562;-1068.571,999.386;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;431;-1575.699,-620.4548;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;599;-1572.848,-368.2683;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;600;-1380.975,-442.1993;Inherit;True;Property;_Texture2;Texture2;6;0;Create;True;0;0;0;False;0;False;-1;ba05b29cd060d204da7b8297f5a3ee4f;ba05b29cd060d204da7b8297f5a3ee4f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;43;-1845.731,14.04999;Inherit;False;1286.461;503.0462;Fresnel Mask;14;366;370;369;368;367;41;39;40;38;372;335;30;611;612;Fresnel Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;452;-1841.826,679.0121;Inherit;False;Property;_DepthDist;Depth Dist;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;553;-903.7859,994.4688;Inherit;False;Vertex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;442;-1380.457,-645.8748;Inherit;True;Property;_Texture1;Texture1;7;0;Create;True;0;0;0;False;0;False;-1;ba05b29cd060d204da7b8297f5a3ee4f;ba05b29cd060d204da7b8297f5a3ee4f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;451;-1691.156,670.0834;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;468;-1073.047,-568.8639;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;558;-1066.971,-244.5011;Inherit;False;553;Vertex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;601;-1092.975,-442.1993;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;482;-1097.061,-325.3992;Inherit;False;Property;_TextureIntensivty;Texture Intensivty;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;30;-1811.415,95.78616;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;368;-1582.366,128.6359;Inherit;False;Property;_NegBias;NegBias;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;370;-1569.57,277.9353;Inherit;False;Property;_NegPower;NegPower;5;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;481;-836.4868,-356.7949;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NegateNode;366;-1595.368,56.13586;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;369;-1572.968,204.136;Inherit;False;Property;_NegScale;NegScale;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1839.194,323.0309;Inherit;False;Property;_FresnelScale;Fresnel Scale;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1837.553,397.1711;Inherit;False;Property;_FresnelPower;Fresnel Power;2;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;453;-1453.869,672.216;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1835.155,248.9046;Inherit;False;Property;_FresnelBias;Fresnel Bias;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;563;-678.4929,-336.9497;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;454;-1278.87,673.8699;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;456;-1340.869,743.2164;Inherit;False;Property;_DepthIntensity;Depth Intensity;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;367;-1398.774,65.33577;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;41;-1574.877,351.1718;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;576;-491.4819,61.93359;Inherit;False;873.078;857.589;Emission;11;571;574;570;577;573;567;557;469;613;615;616;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;611;-1148.252,163.9368;Inherit;False;NegFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;556;-517.3554,-339.1213;Inherit;False;Texture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;458;-1132.87,769.2164;Inherit;False;Property;_FallOff;Fall Off;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;455;-1130.216,673.9346;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;612;-1228.086,340.6205;Inherit;False;PosFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;615;-463.7651,656.0338;Inherit;False;612;PosFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;457;-991.6,672.8585;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;469;-467.2177,110.2978;Inherit;False;Property;_TextureColor;Texture Color;14;0;Create;True;0;0;0;False;0;False;0.6603774,0.258544,0.258544,0;0.6603774,0.258544,0.258544,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;616;-481.3894,761.1129;Inherit;False;Property;_Borderintensity;Border intensity;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;570;-464.3169,391.3852;Inherit;False;Property;_NegFresnelColor;Neg Fresnel Color;15;0;Create;True;0;0;0;False;0;False;0.09620863,0.8460232,0.8867924,0;0.09620863,0.8460232,0.8867924,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;557;-448.9241,292.2899;Inherit;False;556;Texture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;574;-460.4458,569.0961;Inherit;False;611;NegFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;372;-921.6188,240.8053;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;571;-252.6672,467.8071;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;567;-241.1365,211.4064;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;459;-841.5996,676.3335;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;613;-203.7444,690.3389;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-743.5771,285.7354;Inherit;False;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;461;-710.7917,673.226;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;573;-87.80877,441.6395;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;577;54.29251,435.9906;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;572;490.7153,80.6934;Inherit;False;556;Texture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;463;487.6965,240.8011;Inherit;False;461;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;464;489.962,159.0346;Inherit;False;335;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;578;877.8597,-46.14444;Inherit;False;577;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;462;695.3214,131.3985;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;561;907.0569,161.3915;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;610;1178.757,36.30217;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1453.728,39.99855;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Rasengan;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;540;0;483;2
WireConnection;536;0;483;2
WireConnection;541;0;540;0
WireConnection;566;0;536;0
WireConnection;566;1;541;0
WireConnection;564;0;566;0
WireConnection;562;0;564;0
WireConnection;431;0;447;0
WireConnection;431;2;435;0
WireConnection;599;0;597;0
WireConnection;599;2;598;0
WireConnection;600;1;599;0
WireConnection;553;0;562;0
WireConnection;442;1;431;0
WireConnection;451;0;452;0
WireConnection;468;0;442;0
WireConnection;601;0;600;1
WireConnection;481;0;468;0
WireConnection;481;1;601;0
WireConnection;481;2;482;0
WireConnection;481;3;558;0
WireConnection;366;0;30;0
WireConnection;453;0;451;0
WireConnection;563;0;481;0
WireConnection;454;0;453;0
WireConnection;367;4;366;0
WireConnection;367;1;368;0
WireConnection;367;2;369;0
WireConnection;367;3;370;0
WireConnection;41;4;30;0
WireConnection;41;1;40;0
WireConnection;41;2;39;0
WireConnection;41;3;38;0
WireConnection;611;0;367;0
WireConnection;556;0;563;0
WireConnection;455;0;454;0
WireConnection;455;1;456;0
WireConnection;612;0;41;0
WireConnection;457;0;455;0
WireConnection;457;1;458;0
WireConnection;372;0;611;0
WireConnection;372;1;612;0
WireConnection;571;0;570;0
WireConnection;571;1;574;0
WireConnection;567;0;469;0
WireConnection;567;1;557;0
WireConnection;459;0;457;0
WireConnection;613;0;615;0
WireConnection;613;1;616;0
WireConnection;335;0;372;0
WireConnection;461;0;459;0
WireConnection;573;0;567;0
WireConnection;573;1;571;0
WireConnection;573;2;613;0
WireConnection;577;0;573;0
WireConnection;462;0;572;0
WireConnection;462;1;464;0
WireConnection;462;2;463;0
WireConnection;561;0;462;0
WireConnection;610;0;578;0
WireConnection;0;2;610;0
WireConnection;0;9;561;0
ASEEND*/
//CHKSM=6F258A05EACCCC4E96F5EF0D845D515E2D202FC3