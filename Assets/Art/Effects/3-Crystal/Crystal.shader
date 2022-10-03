// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Crystal"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_CrystalColor("Crystal Color", Color) = (0.4425507,0.6132076,0.445711,0)
		_NormalScale("Normal Scale", Float) = 1
		_ShinySpeed("Shiny Speed", Float) = -2
		_ShineWidth("Shine Width", Range( 1 , 1.2)) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_DepthDistance("Depth Distance", Float) = 0
		_ShineMax("Shine Max", Range( 0 , 1)) = 0.7
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		GrabPass{ }

		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardUtils.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _Albedo;
			uniform float4 _Albedo_ST;
			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			uniform float4 _CrystalColor;
			uniform float _ShinySpeed;
			uniform float _ShineWidth;
			uniform float _ShineMax;
			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform sampler2D _Normal;
			uniform float4 _Normal_ST;
			uniform float _NormalScale;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _DepthDistance;
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_Albedo = i.ase_texcoord1.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				float2 uv_TextureSample0 = i.ase_texcoord1.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float2 temp_cast_0 = (_ShinySpeed).xx;
				float2 appendResult112 = (float2(WorldPosition.x , WorldPosition.y));
				float2 texCoord114 = i.ase_texcoord1.xy * float2( 1,1 ) + appendResult112;
				float2 panner94 = ( 1.0 * _Time.y * temp_cast_0 + texCoord114);
				float2 CenteredUV15_g18 = ( ( i.ase_texcoord1.xy * float2( 1,1 ) ) - panner94 );
				float2 break17_g18 = CenteredUV15_g18;
				float2 appendResult23_g18 = (float2(( length( CenteredUV15_g18 ) * 1.0 * 2.0 ) , ( atan2( break17_g18.x , break17_g18.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
				float2 break12_g17 = appendResult23_g18;
				float temp_output_15_0_g17 = ( ( break12_g17.y - ( ( 0.5 / 6.28318548202515 ) * break12_g17.x ) ) * 1.0 );
				float temp_output_20_0_g17 = ( abs( ( temp_output_15_0_g17 - round( temp_output_15_0_g17 ) ) ) * _ShineWidth );
				float smoothstepResult22_g17 = smoothstep( 0.45 , 0.55 , temp_output_20_0_g17);
				float4 screenPos = i.ase_texcoord2;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float2 uv_Normal = i.ase_texcoord1.xy * _Normal_ST.xy + _Normal_ST.zw;
				float4 screenColor63 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale ) , 0.0 ) ).xy);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth54 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth54 = abs( ( screenDepth54 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDistance ) );
				float4 lerpResult58 = lerp( ( ( ( tex2D( _Albedo, uv_Albedo ) + tex2D( _TextureSample0, uv_TextureSample0 ) ) * _CrystalColor ) + ( _CrystalColor * (0.0 + (smoothstepResult22_g17 - 0.0) * (_ShineMax - 0.0) / (1.0 - 0.0)) ) ) , screenColor63 , ( 1.0 - saturate( distanceDepth54 ) ));
				
				
				finalColor = saturate( lerpResult58 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
0;540;1643;459;1815.264;73.59605;1.185104;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;98;-1662.729,6.780519;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;112;-1491.287,29.6637;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1301.633,102.7308;Inherit;False;Property;_ShinySpeed;Shiny Speed;4;0;Create;True;0;0;0;False;0;False;-2;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;114;-1338.168,-16.81706;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;1,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;152;-1301.648,176.2109;Inherit;False;Property;_ShineWidth;Shine Width;5;0;Create;True;0;0;0;False;0;False;1;0;1;1.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;94;-1105.766,-16.01175;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;122;-912.7884,-41.06059;Inherit;True;Whirl;-1;;17;7d75aee9e4d352a4299928ac98404afc;2,26,0,25,1;6;27;FLOAT2;0,0;False;1;FLOAT2;1,1;False;7;FLOAT2;0.5,0.5;False;16;FLOAT;1;False;21;FLOAT;1;False;10;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-926.0336,-605.0087;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;b1c0f46495f91df448545fa3ed68403b;b1c0f46495f91df448545fa3ed68403b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;161;-925.6367,172.7325;Inherit;False;Property;_ShineMax;Shine Max;8;0;Create;True;0;0;0;False;0;False;0.7;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;154;-938.7338,-410.2281;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;0;False;0;False;-1;812508435bbb28a40936aba574e22200;812508435bbb28a40936aba574e22200;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;160;-750.8904,611.0399;Inherit;False;Property;_DepthDistance;Depth Distance;7;0;Create;True;0;0;0;False;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1037.04,460.4424;Inherit;False;Property;_NormalScale;Normal Scale;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;64;-820.3583,251.7729;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;60;-834.8358,-221.7812;Inherit;False;Property;_CrystalColor;Crystal Color;2;0;Create;True;0;0;0;False;0;False;0.4425507,0.6132076,0.445711,0;0.4425507,0.6132076,0.445711,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;65;-867.1982,422.2858;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;8871ac26183ea694da6c65336eb3f027;c09c20f04466ce049a0d45eb05d909b6;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;54;-556.3035,489.3792;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;-579.5161,-363.0255;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;144;-645.077,-2.887506;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-476.8302,-243.2387;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;55;-314.3034,503.0792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-468.9204,-61.47067;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-482.247,311.2472;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-308.9817,-121.3394;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;56;-144.4052,470.3802;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;63;-327.4113,306.0745;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;58;-0.1151679,301.1528;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;158;289.1105,306.836;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;23;600.8924,193.6542;Float;False;True;-1;2;ASEMaterialInspector;100;1;Crystal;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;112;0;98;1
WireConnection;112;1;98;2
WireConnection;114;1;112;0
WireConnection;94;0;114;0
WireConnection;94;2;113;0
WireConnection;122;7;94;0
WireConnection;122;21;152;0
WireConnection;65;5;70;0
WireConnection;54;0;160;0
WireConnection;163;0;8;0
WireConnection;163;1;154;0
WireConnection;144;0;122;0
WireConnection;144;4;161;0
WireConnection;28;0;163;0
WireConnection;28;1;60;0
WireConnection;55;0;54;0
WireConnection;151;0;60;0
WireConnection;151;1;144;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;150;0;28;0
WireConnection;150;1;151;0
WireConnection;56;0;55;0
WireConnection;63;0;66;0
WireConnection;58;0;150;0
WireConnection;58;1;63;0
WireConnection;58;2;56;0
WireConnection;158;0;58;0
WireConnection;23;0;158;0
ASEEND*/
//CHKSM=B5AADE3EC1751EF3B559E6454E0013DC8EAC2567