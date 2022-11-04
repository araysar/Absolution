// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader-Shine"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_ShineWidth("Shine Width", Range( 1 , 1.2)) = 1.1
		_Speed("Speed", Vector) = (2,-2,0,0)
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
			#define ASE_NEEDS_FRAG_COLOR


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
				float4 ase_texcoord1 : TEXCOORD1;
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _MainTex_ST;
			uniform float2 _Speed;
			uniform float _ShineWidth;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				float4 ase_clipPos = UnityObjectToClipPos(IN.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				OUT.ase_texcoord1 = screenPos;
				
				
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

				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 temp_output_95_0 = ( tex2D( _MainTex, uv_MainTex ) * IN.color );
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 appendResult173 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
				float2 texCoord175 = IN.texcoord.xy * float2( 1,1 ) + appendResult173;
				float2 panner176 = ( 1.0 * _Time.y * _Speed + texCoord175);
				float2 CenteredUV15_g18 = ( ( panner176 * float2( 1,1 ) ) - float2( 1,1 ) );
				float2 break17_g18 = CenteredUV15_g18;
				float2 appendResult23_g18 = (float2(( length( CenteredUV15_g18 ) * 1.0 * 2.0 ) , ( atan2( break17_g18.x , break17_g18.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
				float2 break12_g17 = appendResult23_g18;
				float temp_output_15_0_g17 = ( ( break12_g17.y - ( ( 1.0 / 6.28318548202515 ) * break12_g17.x ) ) * 1.0 );
				float temp_output_20_0_g17 = ( abs( ( temp_output_15_0_g17 - round( temp_output_15_0_g17 ) ) ) * _ShineWidth );
				float smoothstepResult22_g17 = smoothstep( 0.45 , 0.55 , temp_output_20_0_g17);
				
				fixed4 c = ( temp_output_95_0 + ( temp_output_95_0 * smoothstepResult22_g17 ) );
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
0;534;1240;457;641.335;-240.3848;1;True;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;200;-608.335,343.3848;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;173;-431.7802,375.7955;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;38;-521.2952,-37.48194;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;175;-278.6612,329.3148;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;1,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;179;-250.4108,454.5412;Inherit;False;Property;_Speed;Speed;1;0;Create;True;0;0;0;False;0;False;2,-2;2,-2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;15;-349.2976,-59.052;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;8ec03c6df54fa1a4a93c57bef99d0504;8ec03c6df54fa1a4a93c57bef99d0504;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;39;-239.1913,127.0489;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;176;-37.25928,373.1201;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-82.94131,496.7426;Inherit;False;Property;_ShineWidth;Shine Width;0;0;Create;True;0;0;0;False;0;False;1.1;1.09;1;1.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;45.90419,78.83565;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;177;206.3929,399.2826;Inherit;True;Whirl;-1;;17;7d75aee9e4d352a4299928ac98404afc;2,26,1,25,1;6;27;FLOAT2;0,0;False;1;FLOAT2;1,1;False;7;FLOAT2;1,1;False;16;FLOAT;1;False;21;FLOAT;1;False;10;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;640.2079,243.7525;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;152;982.8758,153.6673;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;199;1309.2,185.5992;Float;False;True;-1;2;ASEMaterialInspector;0;8;Shader-Shine;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;173;0;200;1
WireConnection;173;1;200;2
WireConnection;175;1;173;0
WireConnection;15;0;38;0
WireConnection;176;0;175;0
WireConnection;176;2;179;0
WireConnection;95;0;15;0
WireConnection;95;1;39;0
WireConnection;177;27;176;0
WireConnection;177;21;178;0
WireConnection;159;0;95;0
WireConnection;159;1;177;0
WireConnection;152;0;95;0
WireConnection;152;1;159;0
WireConnection;199;0;152;0
ASEEND*/
//CHKSM=2784B0FAF1D07E2A5FB9AB372EB54962D5214A82