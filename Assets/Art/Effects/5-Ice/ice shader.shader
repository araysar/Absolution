// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ice shader"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Ice2("Ice 1", 2D) = "white" {}
		_Vertexfall1("Vertex fall", Range( -2 , 0)) = -0.5705734
		_IceColow1("Ice Colow", Color) = (0.3160377,0.9413746,1,0)
		_Normal1("Normal", 2D) = "white" {}
		_Metallic1("Metallic", Range( 0 , 1)) = 0.1926593
		_EmissionPower1("Emission Power", Float) = 2
		_FresnelPower1("Fresnel Power", Float) = 0
		_EmissionColor1("Emission Color", Color) = (1,1,1,0)
		_IceShardTexture1("Ice Shard Texture", 2D) = "white" {}
		_IceShardmax1("Ice Shard max", Range( 0 , 1)) = 0.8705882
		_Ice3("Ice 2", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _IceShardTexture1;
		uniform float _IceShardmax1;
		uniform float _Vertexfall1;
		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform float4 _IceColow1;
		uniform sampler2D _Ice2;
		uniform float4 _Ice2_ST;
		uniform sampler2D _Ice3;
		uniform float4 _Ice3_ST;
		uniform float4 _EmissionColor1;
		uniform float _EmissionPower1;
		uniform float _FresnelPower1;
		uniform float _Metallic1;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float2 appendResult4 = (float2(ase_worldNormal.x , ase_worldNormal.z));
			float VertexY10 = saturate( ( _Vertexfall1 * ase_worldNormal.y ) );
			float3 ase_vertexNormal = v.normal.xyz;
			float4 VertexOffset34 = ( ( tex2Dlod( _IceShardTexture1, float4( (( _IceShardmax1 * appendResult4 )*1.0 + 0.5), 0, 0.0) ) * VertexY10 ) * float4( ase_vertexNormal , 0.0 ) );
			v.vertex.xyz += VertexOffset34.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			o.Normal = tex2D( _Normal1, uv_Normal1 ).rgb;
			float2 uv_Ice2 = i.uv_texcoord * _Ice2_ST.xy + _Ice2_ST.zw;
			float2 uv_Ice3 = i.uv_texcoord * _Ice3_ST.xy + _Ice3_ST.zw;
			float4 tex2DNode19 = tex2D( _Ice3, uv_Ice3 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float VertexY10 = saturate( ( _Vertexfall1 * ase_worldNormal.y ) );
			float4 lerpResult24 = lerp( ( _IceColow1 * tex2D( _Ice2, uv_Ice2 ) ) , tex2DNode19 , VertexY10);
			float4 lerpResult31 = lerp( lerpResult24 , tex2DNode19 , VertexY10);
			float4 Albedo32 = lerpResult31;
			o.Albedo = Albedo32.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV27 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode27 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV27, _FresnelPower1 ) );
			float4 Emission33 = ( ( VertexY10 * _EmissionColor1 * _EmissionPower1 ) * fresnelNode27 );
			o.Emission = Emission33.rgb;
			o.Metallic = _Metallic1;
			o.Smoothness = VertexY10;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
0;535;1586;464;2226.387;39.44262;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-2029.541,3.310433;Inherit;False;1696.306;490.0703;;14;34;29;28;25;22;13;10;8;7;6;5;4;3;2;Vertex Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;2;-2023.845,249.6398;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3;-1668.51,260.9624;Inherit;False;Property;_Vertexfall1;Vertex fall;6;0;Create;True;0;0;0;False;0;False;-0.5705734;0;-2;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1373.113,273.5256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1986.853,130.5453;Inherit;False;Property;_IceShardmax1;Ice Shard max;14;0;Create;True;0;0;0;False;0;False;0.8705882;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-1827.025,240.6367;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1678.371,132.5808;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;8;-1177.656,275.8439;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1974.903,-689.9404;Inherit;False;1152.415;593.3251;Albedo;9;32;31;24;23;21;20;19;14;11;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1050.894,270.1683;Inherit;False;VertexY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;12;-2019.888,551.5018;Inherit;False;796.972;493.6;Emission;8;33;30;27;26;18;17;16;15;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;14;-1873.207,-656.8956;Inherit;False;Property;_IceColow1;Ice Colow;7;0;Create;True;0;0;0;False;0;False;0.3160377,0.9413746,1,0;0.3160377,0.9413746,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;13;-1518.833,131.7413;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;11;-1954.296,-476.1146;Inherit;True;Property;_Ice2;Ice 1;5;0;Create;True;0;0;0;False;0;False;-1;28228e6875c0c8741922cfd91346a7ac;28228e6875c0c8741922cfd91346a7ac;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-2002.312,860.9933;Inherit;False;Property;_EmissionPower1;Emission Power;10;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;15;-2005.012,686.4935;Inherit;False;Property;_EmissionColor1;Emission Color;12;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-1322.194,80.42302;Inherit;True;Property;_IceShardTexture1;Ice Shard Texture;13;0;Create;True;0;0;0;False;0;False;-1;8cdb61b90efa9f34aae895af6fc0b617;8cdb61b90efa9f34aae895af6fc0b617;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-2000.902,946.5818;Inherit;False;Property;_FresnelPower1;Fresnel Power;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1634.27,-571.9968;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;19;-1950.103,-289.6728;Inherit;True;Property;_Ice3;Ice 2;15;0;Create;True;0;0;0;False;0;False;-1;150cfa1422e87bf4193c61258e2e70b1;150cfa1422e87bf4193c61258e2e70b1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;16;-1990.312,614.4767;Inherit;False;10;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-1658.211,-401.8392;Inherit;False;10;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1404.536,-218.4705;Inherit;False;10;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;27;-1777.162,834.751;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1744.324,680.222;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;25;-848.5939,268.9687;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-832.496,161.0739;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;24;-1441.322,-389.4147;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-646.9045,166.974;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;31;-1199.836,-314.8797;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1545.915,764.8848;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-1411.676,759.759;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-522.4753,162.3998;Inherit;False;VertexOffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-1021.259,-322.3982;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;40;-40.34232,-86.01267;Inherit;True;Property;_Normal1;Normal;8;0;Create;True;0;0;0;False;0;False;-1;3ae702793a9819243a5d5c3b5256ac22;3ae702793a9819243a5d5c3b5256ac22;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-25.84052,171.4842;Inherit;False;Property;_Metallic1;Metallic;9;0;Create;True;0;0;0;False;0;False;0.1926593;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;37.80357,101.2484;Inherit;False;33;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;82.9138,-161.4608;Inherit;False;32;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;94.65843,248.9452;Inherit;False;10;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;78.90089,325.0132;Inherit;False;34;VertexOffset;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;331.0326,-96.76358;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;ice shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;3;0
WireConnection;6;1;2;2
WireConnection;4;0;2;1
WireConnection;4;1;2;3
WireConnection;7;0;5;0
WireConnection;7;1;4;0
WireConnection;8;0;6;0
WireConnection;10;0;8;0
WireConnection;13;0;7;0
WireConnection;22;1;13;0
WireConnection;20;0;14;0
WireConnection;20;1;11;0
WireConnection;27;3;17;0
WireConnection;26;0;16;0
WireConnection;26;1;15;0
WireConnection;26;2;18;0
WireConnection;28;0;22;0
WireConnection;28;1;10;0
WireConnection;24;0;20;0
WireConnection;24;1;19;0
WireConnection;24;2;21;0
WireConnection;29;0;28;0
WireConnection;29;1;25;0
WireConnection;31;0;24;0
WireConnection;31;1;19;0
WireConnection;31;2;23;0
WireConnection;30;0;26;0
WireConnection;30;1;27;0
WireConnection;33;0;30;0
WireConnection;34;0;29;0
WireConnection;32;0;31;0
WireConnection;0;0;39;0
WireConnection;0;1;40;0
WireConnection;0;2;36;0
WireConnection;0;3;37;0
WireConnection;0;4;35;0
WireConnection;0;11;38;0
ASEEND*/
//CHKSM=1C09D3893D15609386EAD93E59FFBBCCFD9C4180