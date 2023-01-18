// Made with Amplify Shader Editor v1.9.0.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bottle/Standard_PBL"
{
	Properties
	{
		_Albedo_Map("Albedo_Map", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_Normal_Map("Normal_Map", 2D) = "bump" {}
		_Emission_Map("Emission_Map", 2D) = "white" {}
		[HDR]_Emission_Color("Emission_Color", Color) = (1,1,1,0)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Gloss("Gloss", Range( 0 , 1)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Alpha_Cutoff("Alpha_Cutoff", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal_Map;
		uniform float4 _Normal_Map_ST;
		uniform float4 _Color;
		uniform sampler2D _Albedo_Map;
		uniform float4 _Albedo_Map_ST;
		uniform float4 _Emission_Color;
		uniform sampler2D _Emission_Map;
		uniform float4 _Emission_Map_ST;
		uniform float _Metallic;
		uniform float _Gloss;
		uniform float _Alpha_Cutoff;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal_Map = i.uv_texcoord * _Normal_Map_ST.xy + _Normal_Map_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal_Map, uv_Normal_Map ) );
			float2 uv_Albedo_Map = i.uv_texcoord * _Albedo_Map_ST.xy + _Albedo_Map_ST.zw;
			float4 tex2DNode7 = tex2D( _Albedo_Map, uv_Albedo_Map );
			o.Albedo = ( (_Color).rgb * (tex2DNode7).rgb );
			float2 uv_Emission_Map = i.uv_texcoord * _Emission_Map_ST.xy + _Emission_Map_ST.zw;
			o.Emission = ( (_Emission_Color).rgb * (tex2D( _Emission_Map, uv_Emission_Map )).rgb );
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
			o.Alpha = 1;
			clip( ( tex2DNode7.a * _Alpha_Cutoff ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19002
48;75.22388;1132.657;616.6119;499.0068;188.9172;1;True;False
Node;AmplifyShaderEditor.SamplerNode;7;-893.7927,-156.7523;Inherit;True;Property;_Albedo_Map;Albedo_Map;0;0;Create;True;0;0;0;False;0;False;-1;None;199322ea2e483e84dae0834d3b93ee5e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-871.6329,-354.332;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-890.6417,328.9297;Inherit;True;Property;_Emission_Map;Emission_Map;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-825.6694,575.4288;Inherit;False;Property;_Emission_Color;Emission_Color;4;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;9;-496.1101,-346.2108;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;8;-492.9006,-105.847;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;20;-524.0927,579.5549;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;17;-514.3997,328.7826;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-259.0242,-350.8069;Inherit;False;Property;_Alpha_Cutoff;Alpha_Cutoff;8;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-568.7338,244.8017;Inherit;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-572.9005,69.9288;Inherit;False;Property;_Gloss;Gloss;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-248.589,444.8206;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-248.2759,-43.74814;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;12;-760.6232,-603.5513;Inherit;True;Property;_Normal_Map;Normal_Map;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;65.61429,-326.3179;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;36;238.9546,-95.02731;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Bottle/Standard_PBL;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;18;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;7;-1;-1;-1;0;False;0;0;False;;-1;0;False;_Opacity;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;10;0
WireConnection;8;0;7;0
WireConnection;20;0;19;0
WireConnection;17;0;16;0
WireConnection;18;0;20;0
WireConnection;18;1;17;0
WireConnection;11;0;9;0
WireConnection;11;1;8;0
WireConnection;37;0;7;4
WireConnection;37;1;28;0
WireConnection;36;0;11;0
WireConnection;36;1;12;0
WireConnection;36;2;18;0
WireConnection;36;3;15;0
WireConnection;36;4;14;0
WireConnection;36;10;37;0
ASEEND*/
//CHKSM=BD75D3142BDBE9EA87252FB7C57C98AF854C7C99