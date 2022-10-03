// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water cell"
{
	Properties
	{
		_VertexNoise("Vertex Noise", Float) = 2
		_VertexTime("Vertex Time", Float) = 0.5
		_Color0("Color 0", Color) = (0.371262,0.764151,0.6966732,0)
		_Color1("Color 1", Color) = (0.1541919,0.3897443,0.7264151,0)
		_VertexScale("Vertex Scale", Float) = 0.05
		_icenormal("ice normal", 2D) = "white" {}
		_TextureSpeed("Texture Speed", Vector) = (0.1,0.1,0,0)

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
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
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
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//This is a late directive
			
			uniform float _VertexTime;
			uniform float _VertexNoise;
			uniform float _VertexScale;
			uniform sampler2D _icenormal;
			uniform float2 _TextureSpeed;
			uniform float4 _Color0;
			uniform float4 _Color1;
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
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float mulTime48 = _Time.y * _VertexTime;
				float simplePerlin2D64 = snoise( sin( ( v.ase_normal + mulTime48 ) ).xy*_VertexNoise );
				simplePerlin2D64 = simplePerlin2D64*0.5 + 0.5;
				float temp_output_46_0 = (0.0 + (simplePerlin2D64 - 0.0) * (_VertexScale - 0.0) / (1.0 - 0.0));
				float3 temp_cast_1 = (temp_output_46_0).xxx;
				
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = temp_cast_1;
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
				float2 texCoord72 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner75 = ( 1.0 * _Time.y * _TextureSpeed + texCoord72);
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(WorldPosition);
				float3 ase_worldNormal = i.ase_texcoord2.xyz;
				float dotResult23 = dot( worldSpaceLightDir , ase_worldNormal );
				float mulTime48 = _Time.y * _VertexTime;
				float simplePerlin2D64 = snoise( sin( ( i.ase_normal + mulTime48 ) ).xy*_VertexNoise );
				simplePerlin2D64 = simplePerlin2D64*0.5 + 0.5;
				float temp_output_46_0 = (0.0 + (simplePerlin2D64 - 0.0) * (_VertexScale - 0.0) / (1.0 - 0.0));
				float4 lerpResult50 = lerp( ( tex2D( _icenormal, panner75 ) * ( (dotResult23*0.5 + 0.5) * _Color0 ) ) , _Color1 , temp_output_46_0);
				
				
				finalColor = lerpResult50;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
0;540;1587;459;1490.582;476.5083;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;38;-1111.152,197.8387;Inherit;False;Property;_VertexTime;Vertex Time;1;0;Create;True;0;0;0;False;0;False;0.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;48;-951.8864,204.4085;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;17;-1174.221,-103.8339;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;39;-948.1498,53.83794;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;22;-1197.595,-252.8499;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;23;-967.1946,-171.65;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-719.0936,129.1993;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-992.5402,-519.7655;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;79;-781.7992,-327.7735;Inherit;False;Property;_TextureSpeed;Texture Speed;6;0;Create;True;0;0;0;False;0;False;0.1,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;65;-537.6465,132.9107;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;2;-490.2148,-68.84147;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;0.371262,0.764151,0.6966732,0;0.371262,0.764151,0.6966732,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;24;-815.3973,-186.3032;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;75;-569.6315,-411.3666;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,22;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-556.7331,249.8965;Inherit;False;Property;_VertexNoise;Vertex Noise;0;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;64;-365.9493,135.2563;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;70;-366.2118,-403.5452;Inherit;True;Property;_icenormal;ice normal;5;0;Create;True;0;0;0;False;0;False;-1;28228e6875c0c8741922cfd91346a7ac;3ae702793a9819243a5d5c3b5256ac22;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-265.2314,-159.9891;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-120.0239,304.2479;Inherit;False;Property;_VertexScale;Vertex Scale;4;0;Create;True;0;0;0;False;0;False;0.05;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;46;64.08617,195.7562;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;30.57117,-211.5076;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;57;-48.81361,4.86325;Inherit;False;Property;_Color1;Color 1;3;0;Create;True;0;0;0;False;0;False;0.1541919,0.3897443,0.7264151,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;50;380.4317,-26.21056;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;35;670.2559,144.04;Float;False;True;-1;2;ASEMaterialInspector;100;1;Water cell;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;48;0;38;0
WireConnection;23;0;22;0
WireConnection;23;1;17;0
WireConnection;43;0;39;0
WireConnection;43;1;48;0
WireConnection;65;0;43;0
WireConnection;24;0;23;0
WireConnection;75;0;72;0
WireConnection;75;2;79;0
WireConnection;64;0;65;0
WireConnection;64;1;41;0
WireConnection;70;1;75;0
WireConnection;3;0;24;0
WireConnection;3;1;2;0
WireConnection;46;0;64;0
WireConnection;46;4;42;0
WireConnection;71;0;70;0
WireConnection;71;1;3;0
WireConnection;50;0;71;0
WireConnection;50;1;57;0
WireConnection;50;2;46;0
WireConnection;35;0;50;0
WireConnection;35;1;46;0
ASEEND*/
//CHKSM=672F1C66FC80594D93BBE14F6E11A9C8C1F8A59F