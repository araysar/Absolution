// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ice"
{
	Properties
	{
		_Ice1("Ice 1", 2D) = "white" {}
		_Vertexfall("Vertex fall", Range( -2 , 0)) = -0.3117499
		_IceColow("Ice Colow", Color) = (0.3160377,0.9413746,1,0)
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 15
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_Normal("Normal", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0.1926593
		_EmissionPower("Emission Power", Float) = 2
		_FresnelPower("Fresnel Power", Float) = 0
		_EmissionColor("Emission Color", Color) = (1,1,1,0)
		_IceShardTexture("Ice Shard Texture", 2D) = "white" {}
		_IceShardmax("Ice Shard max", Range( 0 , 1)) = 0.8705882
		_Ice2("Ice 2", 2D) = "white" {}
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

		uniform sampler2D _IceShardTexture;
		uniform float _IceShardmax;
		uniform float _Vertexfall;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _IceColow;
		uniform sampler2D _Ice1;
		uniform float4 _Ice1_ST;
		uniform sampler2D _Ice2;
		uniform float4 _Ice2_ST;
		uniform float4 _EmissionColor;
		uniform float _EmissionPower;
		uniform float _FresnelPower;
		uniform float _Metallic;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float2 appendResult17 = (float2(ase_worldNormal.x , ase_worldNormal.z));
			float VertexY33 = saturate( ( _Vertexfall * ase_worldNormal.y ) );
			float3 ase_vertexNormal = v.normal.xyz;
			float4 VertexOffset41 = ( ( tex2Dlod( _IceShardTexture, float4( (( _IceShardmax * appendResult17 )*1.0 + 0.5), 0, 0.0) ) * VertexY33 ) * ase_vertexNormal.y );
			v.vertex.xyz += VertexOffset41.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Ice1 = i.uv_texcoord * _Ice1_ST.xy + _Ice1_ST.zw;
			float2 uv_Ice2 = i.uv_texcoord * _Ice2_ST.xy + _Ice2_ST.zw;
			float4 tex2DNode24 = tex2D( _Ice2, uv_Ice2 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float VertexY33 = saturate( ( _Vertexfall * ase_worldNormal.y ) );
			float4 lerpResult6 = lerp( ( _IceColow * tex2D( _Ice1, uv_Ice1 ) ) , tex2DNode24 , VertexY33);
			float4 lerpResult26 = lerp( lerpResult6 , tex2DNode24 , VertexY33);
			float4 Albedo37 = lerpResult26;
			o.Albedo = Albedo37.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV48 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode48 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV48, _FresnelPower ) );
			float4 Emission51 = ( ( VertexY33 * _EmissionColor * _EmissionPower ) * fresnelNode48 );
			o.Emission = Emission51.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = VertexY33;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

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
0;540;1587;459;903.1953;351.6456;1.188739;True;False
Node;AmplifyShaderEditor.CommentaryNode;43;-2003.06,-248.6449;Inherit;False;1696.306;490.0703;;14;15;41;12;11;33;10;16;20;18;17;19;3;8;2;Vertex Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;2;-1997.364,-2.315676;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;8;-1642.028,7.632482;Inherit;False;Property;_Vertexfall;Vertex fall;1;0;Create;True;0;0;0;False;0;False;-0.3117499;0;-2;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1800.544,-11.31878;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1960.372,-121.4101;Inherit;False;Property;_IceShardmax;Ice Shard max;14;0;Create;True;0;0;0;False;0;False;0.8705882;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1346.631,21.57005;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;10;-1151.173,23.88838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;39;-1948.422,-941.8958;Inherit;False;1152.415;593.3251;Albedo;9;37;35;6;36;26;29;28;5;24;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1651.889,-119.3746;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-1024.411,18.21277;Inherit;False;VertexY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-1927.815,-728.0701;Inherit;True;Property;_Ice1;Ice 1;0;0;Create;True;0;0;0;False;0;False;-1;28228e6875c0c8741922cfd91346a7ac;28228e6875c0c8741922cfd91346a7ac;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;52;-1993.407,299.5464;Inherit;False;796.972;493.6;Emission;8;51;49;48;50;47;44;46;45;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;20;-1492.351,-120.2141;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;29;-1846.726,-908.8511;Inherit;False;Property;_IceColow;Ice Colow;2;0;Create;True;0;0;0;False;0;False;0.3160377,0.9413746,1,0;0.3160377,0.9413746,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-1975.831,609.0379;Inherit;False;Property;_EmissionPower;Emission Power;10;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;45;-1978.531,434.538;Inherit;False;Property;_EmissionColor;Emission Color;12;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;-1974.421,694.6269;Inherit;False;Property;_FresnelPower;Fresnel Power;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-1963.831,362.5213;Inherit;False;33;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;24;-1923.622,-541.6281;Inherit;True;Property;_Ice2;Ice 2;15;0;Create;True;0;0;0;False;0;False;-1;150cfa1422e87bf4193c61258e2e70b1;150cfa1422e87bf4193c61258e2e70b1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1631.729,-653.7947;Inherit;False;33;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;16;-1295.711,-171.5323;Inherit;True;Property;_IceShardTexture;Ice Shard Texture;13;0;Create;True;0;0;0;False;0;False;-1;8cdb61b90efa9f34aae895af6fc0b617;8cdb61b90efa9f34aae895af6fc0b617;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1607.788,-823.9523;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1378.054,-470.4258;Inherit;False;33;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;6;-1414.84,-641.3701;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;11;-822.1111,17.01316;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1717.843,428.2666;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;48;-1750.681,582.7956;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-806.0131,-90.88151;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-620.4219,-84.98139;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1519.433,512.9294;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;26;-1173.354,-566.8351;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-994.7758,-574.3536;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1385.194,507.8036;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-495.9933,-89.55562;Inherit;False;VertexOffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;31;-13.86023,-337.968;Inherit;True;Property;_Normal;Normal;8;0;Create;True;0;0;0;False;0;False;-1;3ae702793a9819243a5d5c3b5256ac22;3ae702793a9819243a5d5c3b5256ac22;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;53;64.28556,-150.7069;Inherit;False;51;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;14.94152,-79.17123;Inherit;False;Property;_Metallic;Metallic;9;0;Create;True;0;0;0;False;0;False;0.1926593;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;105.3828,73.05775;Inherit;False;41;VertexOffset;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;109.3957,-413.4161;Inherit;False;37;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;137.3917,-1.632836;Inherit;False;33;VertexY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;25;373.3996,-305.1555;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;ice;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;3;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;2;1
WireConnection;17;1;2;3
WireConnection;3;0;8;0
WireConnection;3;1;2;2
WireConnection;10;0;3;0
WireConnection;18;0;19;0
WireConnection;18;1;17;0
WireConnection;33;0;10;0
WireConnection;20;0;18;0
WireConnection;16;1;20;0
WireConnection;28;0;29;0
WireConnection;28;1;5;0
WireConnection;6;0;28;0
WireConnection;6;1;24;0
WireConnection;6;2;36;0
WireConnection;47;0;44;0
WireConnection;47;1;45;0
WireConnection;47;2;46;0
WireConnection;48;3;50;0
WireConnection;15;0;16;0
WireConnection;15;1;33;0
WireConnection;12;0;15;0
WireConnection;12;1;11;2
WireConnection;49;0;47;0
WireConnection;49;1;48;0
WireConnection;26;0;6;0
WireConnection;26;1;24;0
WireConnection;26;2;35;0
WireConnection;37;0;26;0
WireConnection;51;0;49;0
WireConnection;41;0;12;0
WireConnection;25;0;38;0
WireConnection;25;1;31;0
WireConnection;25;2;53;0
WireConnection;25;3;32;0
WireConnection;25;4;34;0
WireConnection;25;11;42;0
ASEEND*/
//CHKSM=10EFB678DF32E307B2320746AF63DA3234A83CD8