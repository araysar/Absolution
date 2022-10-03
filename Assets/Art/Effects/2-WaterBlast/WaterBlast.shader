// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterBlast"
{
	Properties
	{
		_Scale("Scale", Float) = 1
		_Strength("Strength", Float) = 2
		_Speed("Speed", Float) = 1
		_TwirlCenter("Twirl Center", Vector) = (1,1,0,0)
		_SustractQuantity("Sustract Quantity", Float) = 1
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_DistorsionDir("Distorsion Dir", Vector) = (2,0.5,0,0)
		_EmissionPower("Emission Power", Float) = 4
		_EmissionColor1("Emission Color 1", Color) = (0.3434941,0.7409775,0.7830189,0)
		_EmissionColor2("Emission Color 2", Color) = (0.2414589,0.9622642,0.1860982,0)
		_VertexNoise("Vertex Noise", Float) = 4
		_Albedo("Albedo", Color) = (0.7364402,0.3162602,0.9716981,0)
		_VertexTime("Vertex Time", Float) = 3
		_MaxClamp("Max Clamp", Range( 0 , 1)) = 1
		_Opacity("Opacity", Float) = 2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float3 worldPos;
		};

		uniform float _VertexTime;
		uniform float _VertexNoise;
		uniform float4 _Albedo;
		uniform float4 _EmissionColor1;
		uniform float4 _EmissionColor2;
		uniform float _Scale;
		uniform float2 _DistorsionDir;
		uniform float2 _TwirlCenter;
		uniform float _Strength;
		uniform float _Speed;
		uniform float _SustractQuantity;
		uniform float _MaxClamp;
		uniform float _Opacity;
		uniform float _EmissionPower;
		uniform float _EdgeLength;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float2 voronoihash47( float2 p )
		{
			p = p - 1 * floor( p / 1 );
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi47( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -3; j <= 3; j++ )
			{
				for ( int i = -3; i <= 3; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash47( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F2;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			float simplePerlin2D196 = snoise( ( ase_vertexNormal + ( _Time.y * _VertexTime ) ).xy*_VertexNoise );
			simplePerlin2D196 = simplePerlin2D196*0.5 + 0.5;
			float Vertex208 = (( 0.05 * 0.0 ) + (simplePerlin2D196 - 0.0) * (0.05 - ( 0.05 * 0.0 )) / (1.0 - 0.0));
			float3 temp_cast_1 = (Vertex208).xxx;
			v.vertex.xyz += temp_cast_1;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Albedo.rgb;
			float time47 = 0.0;
			float3 ase_worldPos = i.worldPos;
			float2 center45_g8 = _TwirlCenter;
			float2 delta6_g8 = ( ( ase_worldPos.x * _DistorsionDir ) - center45_g8 );
			float angle10_g8 = ( length( delta6_g8 ) * _Strength );
			float x23_g8 = ( ( cos( angle10_g8 ) * delta6_g8.x ) - ( sin( angle10_g8 ) * delta6_g8.y ) );
			float2 break40_g8 = center45_g8;
			float mulTime30 = _Time.y * _Speed;
			float2 temp_cast_1 = ((0.0*1.0 + mulTime30)).xx;
			float2 break41_g8 = temp_cast_1;
			float y35_g8 = ( ( sin( angle10_g8 ) * delta6_g8.x ) + ( cos( angle10_g8 ) * delta6_g8.y ) );
			float2 appendResult44_g8 = (float2(( x23_g8 + break40_g8.x + break41_g8.x ) , ( break40_g8.y + break41_g8.y + y35_g8 )));
			float2 coords47 = appendResult44_g8 * _Scale;
			float2 id47 = 0;
			float2 uv47 = 0;
			float fade47 = 0.5;
			float voroi47 = 0;
			float rest47 = 0;
			for( int it47 = 0; it47 <3; it47++ ){
			voroi47 += fade47 * voronoi47( coords47, time47, id47, uv47, 0 );
			rest47 += fade47;
			coords47 *= 2;
			fade47 *= 0.5;
			}//Voronoi47
			voroi47 /= rest47;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult114 = clamp( ( ( 1.0 - ase_vertex3Pos.x ) - saturate( ( _SustractQuantity * cos( unity_DeltaTime.x ) ) ) ) , 0.0 , _MaxClamp );
			float Opacity120 = ( voroi47 * clampResult114 * _Opacity );
			float4 lerpResult205 = lerp( _EmissionColor1 , _EmissionColor2 , Opacity120);
			float4 Emission124 = ( lerpResult205 * _EmissionPower );
			o.Emission = Emission124.rgb;
			o.Alpha = Opacity120;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

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
				float3 worldPos : TEXCOORD1;
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
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
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
0;540;1587;459;3117.128;280.4037;1.068494;True;False
Node;AmplifyShaderEditor.CommentaryNode;122;-2668.959,-639.7823;Inherit;False;1522.861;857.215;Opacity;24;119;102;51;114;120;116;117;47;36;35;31;32;29;33;57;30;28;26;106;108;107;77;50;234;OPACITY;1,1,1,1;0;0
Node;AmplifyShaderEditor.DeltaTime;106;-2627.929,67.07282;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-2610.648,-264.5857;Inherit;False;Property;_Speed;Speed;2;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;234;-2458.439,73.26276;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2551.466,-22.93227;Inherit;False;Property;_SustractQuantity;Sustract Quantity;4;0;Create;True;0;0;0;False;0;False;1;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-2340.624,-15.91007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;57;-2588.518,-499.8421;Inherit;False;Property;_DistorsionDir;Distorsion Dir;10;0;Create;True;0;0;0;False;0;False;2,0.5;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-2428.945,-595.511;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;30;-2481.02,-260.6214;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;77;-2457.838,-172.2733;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2252.708,-517.5403;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;102;-2231.174,-149.2705;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;33;-2304.152,-307.8018;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2277.746,-382.0434;Inherit;False;Property;_Strength;Strength;1;0;Create;True;0;0;0;False;0;False;2;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;108;-2204.498,-16.79685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;31;-2430.973,-439.79;Inherit;False;Property;_TwirlCenter;Twirl Center;3;0;Create;True;0;0;0;False;0;False;1,1;1,1.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;36;-2110.996,-459.9345;Inherit;False;Twirl;-1;;8;90936742ac32db8449cd21ab6dd337c8;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;51;-2053.132,-125.1971;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-2212.332,55.62337;Inherit;False;Property;_MaxClamp;Max Clamp;17;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2083.066,-311.2961;Inherit;False;Property;_Scale;Scale;0;0;Create;True;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;47;-1892.207,-438.3394;Inherit;True;2;0;2.16;1;3;True;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.CommentaryNode;207;-2614.5,270.9896;Inherit;False;1221.783;446.4508;Vertex Offset;11;198;199;196;200;197;195;193;190;194;192;208;Vertex Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1894.098,-5.412798;Inherit;False;Property;_Opacity;Opacity;18;0;Create;True;0;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;114;-1895.257,-130.3423;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;192;-2598.684,418.9755;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-1682.018,-259.6056;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-2568.684,560.9758;Inherit;False;Property;_VertexTime;Vertex Time;16;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;123;-1069.898,-607.774;Inherit;False;853.0037;505.7227;Emission;7;124;70;205;68;206;69;204;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;-1441.95,-255.6309;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;193;-2400.684,492.9755;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;190;-2417.684,327.9755;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;200;-2199.85,506.7352;Inherit;False;Property;_VertexNoise;Vertex Noise;14;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;-1037.762,-550.8635;Inherit;False;Property;_EmissionColor1;Emission Color 1;12;0;Create;True;0;0;0;False;0;False;0.3434941,0.7409775,0.7830189,0;0.3434941,0.7409775,0.7830189,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;197;-2200.251,583.6036;Inherit;False;Constant;_VertexScale;Vertex Scale;13;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;195;-2228.685,408.9755;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;204;-1035.194,-376.5291;Inherit;False;Property;_EmissionColor2;Emission Color 2;13;0;Create;True;0;0;0;False;0;False;0.2414589,0.9622642,0.1860982,0;0.3434941,0.7409775,0.7830189,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;206;-1013.281,-197.3583;Inherit;False;120;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;205;-757.9965,-425.533;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;196;-2029.685,406.9755;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;21;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-1977.636,523.7469;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-778.5666,-299.0198;Inherit;False;Property;_EmissionPower;Emission Power;11;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-569.9609,-418.2616;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;198;-1781.52,476.5322;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;208;-1588.768,477.271;Inherit;False;Vertex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;-432.4801,-424.7972;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;211;104.0569,-266.274;Inherit;False;Property;_Albedo;Albedo;15;0;Create;True;0;0;0;False;0;False;0.7364402,0.3162602,0.9716981,0;0.4593717,0.8417088,0.8773585,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;209;128.9641,202.4638;Inherit;False;208;Vertex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;127.8844,-40.75744;Inherit;False;124;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;128.059,130.6089;Inherit;False;120;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;377.6273,-80.54774;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;WaterBlast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;5;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;234;0;106;1
WireConnection;107;0;50;0
WireConnection;107;1;234;0
WireConnection;30;0;26;0
WireConnection;29;0;28;1
WireConnection;29;1;57;0
WireConnection;102;0;77;1
WireConnection;33;2;30;0
WireConnection;108;0;107;0
WireConnection;36;1;29;0
WireConnection;36;2;31;0
WireConnection;36;3;32;0
WireConnection;36;4;33;0
WireConnection;51;0;102;0
WireConnection;51;1;108;0
WireConnection;47;0;36;0
WireConnection;47;2;35;0
WireConnection;114;0;51;0
WireConnection;114;2;117;0
WireConnection;116;0;47;0
WireConnection;116;1;114;0
WireConnection;116;2;119;0
WireConnection;120;0;116;0
WireConnection;193;0;192;2
WireConnection;193;1;194;0
WireConnection;195;0;190;0
WireConnection;195;1;193;0
WireConnection;205;0;69;0
WireConnection;205;1;204;0
WireConnection;205;2;206;0
WireConnection;196;0;195;0
WireConnection;196;1;200;0
WireConnection;199;0;197;0
WireConnection;70;0;205;0
WireConnection;70;1;68;0
WireConnection;198;0;196;0
WireConnection;198;3;199;0
WireConnection;198;4;197;0
WireConnection;208;0;198;0
WireConnection;124;0;70;0
WireConnection;0;0;211;0
WireConnection;0;2;125;0
WireConnection;0;9;121;0
WireConnection;0;11;209;0
ASEEND*/
//CHKSM=F0D5A0946CE8519B315E824AC57627F591264976