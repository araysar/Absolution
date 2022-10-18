// Upgrade NOTE: upgraded instancing buffer 'ShaderWater' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader-Water"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_MoveTexture("Move Texture", Vector) = (0,1,0,0)
		_Waterdepth("Water depth", Float) = 2
		_MinDepth("Min Depth", Range( 0 , 1)) = 0
		_Speed("Speed", Float) = 2
		_DistorsionDir("Distorsion Dir", Vector) = (1,0,0,0)
		_Distorsionamount("Distorsion amount", Float) = 0.1
		_Texture("Texture", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardUtils.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float _Waterdepth;
			uniform float _MinDepth;
			uniform sampler2D _Texture;
			uniform sampler2D _Normal;
			uniform float _Speed;
			uniform float _Distorsionamount;
			uniform float2 _MoveTexture;
			UNITY_INSTANCING_BUFFER_START(ShaderWater)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Texture_ST)
#define _Texture_ST_arr ShaderWater
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define _MainTex_ST_arr ShaderWater
				UNITY_DEFINE_INSTANCED_PROP(float2, _DistorsionDir)
#define _DistorsionDir_arr ShaderWater
			UNITY_INSTANCING_BUFFER_END(ShaderWater)

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 texCoord106 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult113 = clamp( pow( texCoord106.y , _Waterdepth ) , _MinDepth , 2.0 );
				float2 _DistorsionDir_Instance = UNITY_ACCESS_INSTANCED_PROP(_DistorsionDir_arr, _DistorsionDir);
				float mulTime46 = _Time.y * _Speed;
				float TimeVar197_g1 = mulTime46;
				float4 _Texture_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Texture_ST_arr, _Texture_ST);
				float2 uv_Texture = IN.texcoord.xy * _Texture_ST_Instance.xy + _Texture_ST_Instance.zw;
				float2 MainUvs222_g1 = uv_Texture;
				float4 tex2DNode65_g1 = tex2D( _Normal, ( ( _DistorsionDir_Instance * TimeVar197_g1 ) + MainUvs222_g1 ) );
				float4 appendResult82_g1 = (float4(0.0 , tex2DNode65_g1.g , 0.0 , tex2DNode65_g1.r));
				float2 temp_output_84_0_g1 = (UnpackScaleNormal( appendResult82_g1, _Distorsionamount )).xy;
				float2 panner179_g1 = ( 1.0 * _Time.y * _MoveTexture + MainUvs222_g1);
				float2 temp_output_71_0_g1 = ( temp_output_84_0_g1 + panner179_g1 );
				float4 tex2DNode96_g1 = tex2D( _Texture, temp_output_71_0_g1 );
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				
				fixed4 c = ( clampResult113 * ( tex2DNode96_g1 * ( tex2D( _MainTex, uv_MainTex ) * IN.color ) ) * 1.5 );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
0;650;1257;341;-223.9258;-143.2785;1;True;False
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;38;-521.2952,-37.48194;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;108;395.3773,161.7601;Inherit;False;Property;_Waterdepth;Water depth;8;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-349.2976,-59.052;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;8ec03c6df54fa1a4a93c57bef99d0504;8ec03c6df54fa1a4a93c57bef99d0504;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;364.5655,42.26366;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;39;-239.1913,127.0489;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;152.4263,703.4376;Inherit;False;Property;_Speed;Speed;10;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;45.90419,78.83565;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;93;-66.46756,181.246;Inherit;True;Property;_Texture;Texture;13;0;Create;True;0;0;0;False;0;False;8ec03c6df54fa1a4a93c57bef99d0504;8ec03c6df54fa1a4a93c57bef99d0504;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;45;154.6634,465.7566;Inherit;False;Property;_Distorsionamount;Distorsion amount;12;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;99;141.4088,307.7333;Inherit;False;Property;_MoveTexture;Move Texture;7;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;47;198.8629,546.9042;Inherit;False;InstancedProperty;_DistorsionDir;Distorsion Dir;11;0;Create;True;0;0;0;False;0;False;1,0;0,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;46;299.8276,682.0732;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;450.5889,235.522;Inherit;False;Property;_MinDepth;Min Depth;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;107;609.6256,129.9771;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;94;-75.16754,382.7461;Inherit;True;Property;_Normal;Normal;14;0;Create;True;0;0;0;False;0;False;c044fafa8f3da954f91a8032f8ede047;c044fafa8f3da954f91a8032f8ede047;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;92;503.5606,317.9801;Inherit;True;UI-Sprite Effect Layer;0;;1;789bf62641c5cfe4ab7126850acc22b8;18,74,0,204,0,191,1,225,0,242,0,237,0,249,0,186,0,177,1,182,1,229,0,92,0,98,0,234,0,126,0,129,1,130,0,31,0;18;192;COLOR;0,0,0,1;False;39;COLOR;1,1,1,1;False;37;SAMPLER2D;;False;218;FLOAT2;0,0;False;239;FLOAT2;0,0;False;181;FLOAT2;0,0;False;75;SAMPLER2D;;False;80;FLOAT;1;False;183;FLOAT2;0,0;False;188;SAMPLER2D;;False;33;SAMPLER2D;;False;248;FLOAT2;0,0;False;233;SAMPLER2D;;False;101;SAMPLER2D;;False;57;FLOAT4;0,0,0,0;False;40;FLOAT;0;False;231;FLOAT;1;False;30;FLOAT;1;False;2;COLOR;0;FLOAT2;172
Node;AmplifyShaderEditor.ClampOpNode;113;795.5892,193.5221;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;922.9258,399.2785;Inherit;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;128;-77.245,598.3379;Inherit;True;Property;_Texture0;Texture 0;15;0;Create;True;0;0;0;False;0;False;c044fafa8f3da954f91a8032f8ede047;c044fafa8f3da954f91a8032f8ede047;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SaturateNode;143;1078.68,426.4781;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;951.733,246.5676;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;100;1282.2,295.5992;Float;False;True;-1;2;ASEMaterialInspector;0;8;Shader-Water;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;15;0;38;0
WireConnection;95;0;15;0
WireConnection;95;1;39;0
WireConnection;46;0;49;0
WireConnection;107;0;106;2
WireConnection;107;1;108;0
WireConnection;92;39;95;0
WireConnection;92;37;93;0
WireConnection;92;181;99;0
WireConnection;92;75;94;0
WireConnection;92;80;45;0
WireConnection;92;183;47;0
WireConnection;92;40;46;0
WireConnection;113;0;107;0
WireConnection;113;1;114;0
WireConnection;140;0;113;0
WireConnection;140;1;92;0
WireConnection;140;2;144;0
WireConnection;100;0;140;0
ASEEND*/
//CHKSM=FED65A4E9D462A307474940254AEE739E3627F25