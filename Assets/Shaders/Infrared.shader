// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Infrared"
{
	Properties
	{
		_Curve("Curve", Range( 0 , 10)) = 1
		_Curve2("Curve2", Range( 0 , 10)) = 1
		_Position("Position", Vector) = (0,0,0,0)
		_Position2("Position2", Vector) = (0,0,0,0)
		_Size("Size", Float) = 0.5
		_Size2("Size2", Float) = 1
		_Strength("Strength", Float) = 1
		_Strength2("Strength 2", Float) = 0.5
		_IRColor("IR Color", 2D) = "white" {}
		_IRMax("IR Max", Float) = 0
		_IRMin("IR Min", Float) = 0
		_Attenuation("Attenuation", Float) = 1
		_BaseColor("Base Color", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _BaseColor;
		uniform float _Attenuation;
		uniform sampler2D _IRColor;
		uniform float3 _Position2;
		uniform float _Curve2;
		uniform float _Size2;
		uniform float _Strength2;
		uniform float3 _Position;
		uniform float _Curve;
		uniform float _Size;
		uniform float _Strength;
		uniform float _IRMin;
		uniform float _IRMax;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult190 = dot( ase_worldNormal , ase_worldlightDir );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_486_0 = ( ( 1 * ase_lightColor.a ) * _Attenuation );
			float temp_output_484_0 = ( dotResult190 * temp_output_486_0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 temp_output_105_0 = float3( 0,0,0 );
			float3 temp_output_85_0 = float3( 0,0,0 );
			float temp_output_110_0 = max( ( saturate( ( ( 1.0 - pow( distance( ase_vertex3Pos , ( temp_output_105_0 + _Position2 ) ) , _Curve2 ) ) + _Size2 ) ) * _Strength2 ) , ( saturate( ( ( 1.0 - pow( distance( ase_vertex3Pos , ( temp_output_85_0 + _Position ) ) , _Curve ) ) + _Size ) ) * _Strength ) );
			float2 temp_cast_0 = (temp_output_110_0).xx;
			float4 tex2DNode271 = tex2D( _IRColor, temp_cast_0 );
			float temp_output_272_0 = (_IRMin + (temp_output_110_0 - 0.0) * (_IRMax - _IRMin) / (1.0 - 0.0));
			float4 lerpResult530 = lerp( ( _BaseColor * temp_output_484_0 ) , tex2DNode271 , temp_output_272_0);
			o.Albedo = lerpResult530.rgb;
			float4 lerpResult140 = lerp( float4( 0,0,0,0 ) , tex2DNode271 , temp_output_272_0);
			o.Emission = lerpResult140.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				surfIN.worldNormal = IN.worldNormal;
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
Version=15600
216;92;1217;632;-1501.261;1649.093;2.310455;True;False
Node;AmplifyShaderEditor.TransformPositionNode;85;-1445.531,-25.30064;Float;False;Object;Object;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;90;-1388.301,159.1202;Float;False;Property;_Position;Position;2;0;Create;True;0;0;False;0;0,0,0;-0.2,-1.05,-4.25;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;106;-1576.966,-713.9664;Float;False;Property;_Position2;Position2;3;0;Create;True;0;0;False;0;0,0,0;0,0.25,-3.25;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;105;-1632.07,-898.387;Float;False;Object;Object;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-1046.908,-0.2014337;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-1297.071,-859.3699;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;103;-1382.533,-1089.088;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;48;-1469.744,-278.5058;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-1131.504,-639.5953;Float;False;Property;_Curve2;Curve2;1;0;Create;True;0;0;False;0;1;0.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-944.966,233.4914;Float;False;Property;_Curve;Curve;0;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;81;-873.3373,-149.5789;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;102;-1059.875,-1022.665;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;96;-780.0258,-1023.045;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;51;-593.488,-149.9584;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;108;-469.4305,-949.136;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;94;-282.8925,-76.04963;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-314.0868,-754.2708;Float;False;Property;_Size2;Size2;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-338.1692,226.8972;Float;False;Property;_Size;Size;4;0;Create;True;0;0;False;0;0.5;0.53;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;187;66.35741,-1921.769;Float;False;540.401;320.6003;Comment;3;190;189;188;N . L;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightAttenuation;214;-58.62871,-1485.558;Float;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;215;84.727,-1270.527;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-84.43573,-818.5096;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-80.80508,-28.56283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;47.67627,-628.318;Float;False;Property;_Strength2;Strength 2;7;0;Create;True;0;0;False;0;0.5;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;77.54291,239.7947;Float;False;Property;_Strength;Strength;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;216;315.6844,-1520.602;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;99;99.85912,-807.6498;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;188;178.3577,-1873.769;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;487;400.071,-1248.271;Float;False;Property;_Attenuation;Attenuation;24;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;189;130.3575,-1713.769;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;95;109.0327,-28.78843;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;486;572.9652,-1473.785;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;284.3943,20.54907;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;303.9822,-759.4103;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;190;466.3578,-1809.769;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;478;772.8336,38.70585;Float;False;Property;_IRMin;IR Min;23;0;Create;True;0;0;False;0;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;519;2005.112,-1675.62;Float;False;Property;_BaseColor;Base Color;27;0;Create;True;0;0;False;0;0,0,0,0;0.5188678,0.5188678,0.5188678,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;477;799.0112,139.9253;Float;False;Property;_IRMax;IR Max;22;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;484;798.7889,-1641.42;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;110;477.105,-330.393;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;459;-4169.39,-3808.433;Float;False;4121.131;1878.308;Triplanar Noise;58;372;371;424;401;417;402;373;429;374;430;433;375;428;376;434;382;425;377;360;379;408;387;422;381;421;423;383;389;388;440;420;364;419;378;386;385;439;392;418;394;391;393;390;395;396;397;399;398;405;361;367;406;438;380;384;400;456;498;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;271;953.4457,-721.5711;Float;True;Property;_IRColor;IR Color;9;0;Create;True;0;0;False;0;e543949d3b0f299408e25a65b78d966c;eba2061e114976c4c8abc6ad0bddef47;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;272;1052.033,-257.4275;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;520;2369.799,-1464.694;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;396;-1031.77,-2821.78;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;422;-2014.958,-3243.844;Float;False;421;Tiling;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;507;-1709.353,894.4537;Float;False;Property;_MinNew;Min New;17;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;503;-2035.746,608.2738;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-1818.493,-3349.138;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;400;-613.0969,-2985.11;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;456;-291.2595,-2934.696;Float;False;Noise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;390;-1189.097,-3177.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;360;-3342.932,-3547.38;Float;False;Property;_Tiling;Tiling;12;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;379;-2258.903,-3134.463;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;380;-2437.402,-3353.865;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;394;-1478.879,-3078.549;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;392;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;405;-1026.823,-3620.919;Float;True;Spherical;Object;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;9;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;393;-1189.097,-2905.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;381;-2628.39,-2917.437;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;421;-2988.42,-3601.895;Float;False;Tiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;391;-1476.094,-2800.612;Float;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;392;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;392;-1473.796,-3334.011;Float;True;Property;_TriplanarAlbedo;Triplanar Albedo;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;388;-2185.365,-3322.834;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;445;-963.5115,664.2074;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;505;-1932.295,769.1168;Float;False;Constant;_Int1;Int 1;20;0;Create;True;0;0;False;0;10;0;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;423;-1901.133,-2414.177;Float;False;421;Tiling;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;386;-1221.097,-3145.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;530;2933.486,-1213.439;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;430;-3154.077,-2363.359;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;418;-2275.018,-2615.911;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;377;-2628.39,-3061.437;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;495;1022.807,-1131.474;Float;False;Property;_MinLight;Min Light;25;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;316;1722.388,-2231.702;Float;False;Property;_NoiseAmount;Noise Amount;10;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;440;-1926.967,-2941.896;Float;False;421;Tiling;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;500;-1680.309,1041.332;Float;False;Property;_MaxNew;Max New;16;0;Create;True;0;0;False;0;0;1.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;408;-2221.387,-2475.53;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;499;-867.2811,394.0119;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;513;-1684.052,441.0689;Float;False;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;269;1608.408,-1838.737;Float;True;Property;_NightVisionRamp;Night Vision Ramp;8;0;Create;True;0;0;False;0;52e66a9243cdfed44b5e906f5910d35b;4203020942116ac4aaf1befabdbb6d44;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;364;-2230.027,-3741.471;Float;True;Property;_Texture0;Texture 0;11;0;Create;True;0;0;False;0;None;bdbe94d7623ec3940947b62544306f1c;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleRemainderNode;428;-3015.933,-2282.745;Float;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;315;1995.925,-2440.166;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;424;-3569.591,-2426.318;Float;False;Property;_TimeScale;Time Scale;14;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;367;-1919.898,-3739.476;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;387;-1924.45,-2761.855;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;389;-2053.066,-3022.965;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;385;-1221.097,-2873.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;417;-3352.888,-2429.772;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;383;-1957.097,-3145.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;195;761.4802,-1937.59;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;384;-2325.895,-3054.98;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;402;-3428.39,-2901.437;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;425;-2495.242,-2479.776;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2932.39,-2901.437;Float;True;BlendComponents;1;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;375;-3092.39,-2901.437;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;374;-3254.49,-2835.04;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;429;-3271.442,-2217.584;Float;False;Constant;_Int0;Int 0;20;0;Create;True;0;0;False;0;10;0;0;1;INT;0
Node;AmplifyShaderEditor.Vector3Node;373;-3461.508,-2720.987;Float;False;Constant;_Vector0;Vector 0;-1;0;Create;True;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;401;-3595.17,-2889.23;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;399;-869.0969,-3241.11;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;371;-3860.39,-2965.437;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.WorldNormalVector;372;-4119.39,-2846.437;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;382;-2723.474,-2575.239;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;434;-2752.066,-2331.686;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleRemainderNode;506;-1676.786,703.9556;Float;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;469;1103.049,-1680.776;Float;False;Property;_NVMax;NV Max;21;0;Create;True;0;0;False;0;0;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;498;-2230.14,-2153.626;Float;False;Random;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;453;-488.794,595.6336;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;395;-1109.097,-3081.11;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;-1706.577,-2591.517;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;497;1472.695,-1434.995;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;496;1741.062,-1353.384;Float;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;52e66a9243cdfed44b5e906f5910d35b;4203020942116ac4aaf1befabdbb6d44;True;0;False;white;Auto;False;Instance;269;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;485;1114.789,-1547.922;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;502;-2258.317,619.0626;Float;False;Property;_PulseSpeed;Pulse Speed;15;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;439;-1702.175,-3044.224;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;361;-1428.569,-3457.39;Float;False;Property;_Falloff;Falloff;13;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;494;1291.208,-1288.395;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;512;-1012.097,572.556;Float;False;Property;_IRNoiseAmount;IR Noise Amount;26;0;Create;True;0;0;False;0;0;4.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;446;-1252.514,892.7593;Float;False;Property;_NonsenseScale;Nonsense Scale;19;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;504;-1814.93,623.3419;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;406;-1349.9,-3758.433;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;140;2478.066,-813.3549;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;438;-1121.499,-2495.888;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;433;-3053.197,-2045.125;Float;False;Property;_ScaleChange;Scale Change;18;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;397;-1109.097,-3353.11;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;467;1301.398,-1912.605;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;398;-853.0969,-2889.11;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;492;886.7408,-1309.62;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;468;1069.425,-1783.622;Float;False;Property;_NVMin;NV Min;20;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;378;-2628.39,-2773.437;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;317;1628.019,-2499.737;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;501;-1412.919,655.0147;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;0.9;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;463;3377.338,-1036.455;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/Infrared;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;89;0;85;0
WireConnection;89;1;90;0
WireConnection;104;0;105;0
WireConnection;104;1;106;0
WireConnection;81;0;48;0
WireConnection;81;1;89;0
WireConnection;102;0;103;0
WireConnection;102;1;104;0
WireConnection;96;0;102;0
WireConnection;96;1;107;0
WireConnection;51;0;81;0
WireConnection;51;1;28;0
WireConnection;108;0;96;0
WireConnection;94;0;51;0
WireConnection;97;0;108;0
WireConnection;97;1;109;0
WireConnection;92;0;94;0
WireConnection;92;1;93;0
WireConnection;216;0;214;0
WireConnection;216;1;215;2
WireConnection;99;0;97;0
WireConnection;95;0;92;0
WireConnection;486;0;216;0
WireConnection;486;1;487;0
WireConnection;111;0;95;0
WireConnection;111;1;112;0
WireConnection;113;0;99;0
WireConnection;113;1;114;0
WireConnection;190;0;188;0
WireConnection;190;1;189;0
WireConnection;484;0;190;0
WireConnection;484;1;486;0
WireConnection;110;0;113;0
WireConnection;110;1;111;0
WireConnection;271;1;110;0
WireConnection;272;0;110;0
WireConnection;272;3;478;0
WireConnection;272;4;477;0
WireConnection;520;0;519;0
WireConnection;520;1;484;0
WireConnection;396;0;391;0
WireConnection;396;1;418;0
WireConnection;503;0;502;0
WireConnection;419;0;388;0
WireConnection;419;1;422;0
WireConnection;400;0;399;0
WireConnection;400;1;398;0
WireConnection;456;0;400;0
WireConnection;390;0;386;0
WireConnection;379;0;377;0
WireConnection;394;1;439;0
WireConnection;405;0;364;0
WireConnection;405;3;360;0
WireConnection;405;4;361;0
WireConnection;393;0;385;0
WireConnection;381;0;376;0
WireConnection;421;0;360;0
WireConnection;391;1;420;0
WireConnection;392;0;364;0
WireConnection;392;1;419;0
WireConnection;388;0;408;1
WireConnection;388;1;408;2
WireConnection;445;0;501;0
WireConnection;445;1;446;0
WireConnection;386;0;383;0
WireConnection;530;0;520;0
WireConnection;530;1;271;0
WireConnection;530;2;272;0
WireConnection;430;0;417;0
WireConnection;418;0;378;2
WireConnection;377;0;376;0
WireConnection;408;0;425;0
WireConnection;499;1;513;0
WireConnection;499;2;512;0
WireConnection;513;0;503;0
WireConnection;269;1;467;0
WireConnection;428;0;430;0
WireConnection;428;1;429;0
WireConnection;315;1;317;0
WireConnection;315;2;316;0
WireConnection;367;2;364;0
WireConnection;387;0;408;0
WireConnection;387;1;408;1
WireConnection;389;0;408;0
WireConnection;389;1;408;2
WireConnection;385;0;381;1
WireConnection;417;0;424;0
WireConnection;383;0;379;0
WireConnection;195;0;190;0
WireConnection;402;0;401;0
WireConnection;425;0;382;0
WireConnection;425;1;434;0
WireConnection;376;0;375;0
WireConnection;375;0;402;0
WireConnection;375;1;374;0
WireConnection;374;0;402;0
WireConnection;374;1;373;0
WireConnection;401;0;371;0
WireConnection;401;1;372;0
WireConnection;399;0;397;0
WireConnection;399;1;395;0
WireConnection;434;0;428;0
WireConnection;434;4;433;0
WireConnection;506;0;504;0
WireConnection;506;1;505;0
WireConnection;498;0;425;0
WireConnection;453;1;445;0
WireConnection;395;0;394;0
WireConnection;395;1;393;0
WireConnection;420;0;387;0
WireConnection;420;1;423;0
WireConnection;496;1;494;0
WireConnection;485;0;484;0
WireConnection;439;0;389;0
WireConnection;439;1;440;0
WireConnection;494;0;190;0
WireConnection;494;1;495;0
WireConnection;504;0;503;0
WireConnection;140;1;271;0
WireConnection;140;2;272;0
WireConnection;397;0;392;0
WireConnection;397;1;390;0
WireConnection;467;0;190;0
WireConnection;467;3;468;0
WireConnection;467;4;469;0
WireConnection;398;0;396;0
WireConnection;492;0;190;0
WireConnection;492;1;486;0
WireConnection;378;0;376;0
WireConnection;317;0;269;0
WireConnection;317;1;456;0
WireConnection;501;0;506;0
WireConnection;501;3;507;0
WireConnection;501;4;500;0
WireConnection;463;0;530;0
WireConnection;463;2;140;0
ASEEND*/
//CHKSM=C14B639E67FEE4A83041B4E4F032BC6DAAF2A6E9