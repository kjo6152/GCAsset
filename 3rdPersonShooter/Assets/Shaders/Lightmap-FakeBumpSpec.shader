// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced '_PPLAmbient' with 'UNITY_LIGHTMODEL_AMBIENT'
// Upgrade NOTE: replaced 'glstate.matrix.texture' with 'UNITY_MATRIX_TEXTURE'

Shader "Lightmapped/ Fake Dir/Bumped Specular" {
Properties {
	_Color ("Main Color", Vector) = (1.5, 1.5, 1.5, 1)
	_SpecColor ("Spec Color", Color) = (.5, .5, .5, .5)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB), Gloss (A)", 2D) = "white" {}
	_BumpMap ("Bumpmap (RGB)", 2D) = "bump" {}
	_LightMap ("Lightmap (rgb)", 2D) = "bump" {}
}

Category {
	/* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
	Fog { Color [_AddFog] }

	// ------------------------------------------------------------------
	// ARB fragment program	
	SubShader {
		UsePass " Glossy/BASE"
		// Get in the lightmap
		Pass {	
			Name "BASE"
			Tags {"LightMode" = "Always"}				
CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata_tanlightmap members vertex,tangent,normal,texcoord,texcoord1)
#pragma exclude_renderers d3d11 xbox360
// profiles arbfp1
// fragment frag
// fragmentoption ARB_fog_exp2
// fragmentoption ARB_precision_hint_fastest

// vertex vert
#include "UnityCG.cginc"

struct appdata_tanlightmap {
    float4 vertex;
    float4 tangent;
    float3 normal;
    float4 texcoord;
    float4 texcoord1;
};


struct v2f { 
	float4 pos : SV_POSITION;
	float4	uv			: TEXCOORD0;
	float2	uv3			: TEXCOORD1;
	float3	I		: TEXCOORD2;
	float3	TtoW0 	: TEXCOORD3;
	float3	TtoW1	: TEXCOORD4;
	float3	TtoW2	: TEXCOORD5;
	float3	lightDirT	: TEXCOORD6;

}; 

#define TRANSFORM_UV2(idx) mul( UNITY_MATRIX_TEXTURE[idx], v.texcoord1 ).xy
uniform float3 _LightmapDir = normalize(float3(-1,1,1));
uniform float4 _SpecColor, _Color, _LightmapColor;

v2f vert (appdata_tanlightmap v)
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv.xy = TRANSFORM_UV(1);
	o.uv.zw = TRANSFORM_UV(0);
	o.uv3 = TRANSFORM_UV2(2);
	
	TANGENT_SPACE_ROTATION;

	o.I = mul( (float3x3)_Object2World, -ObjSpaceViewDir( v.vertex ) );	
	o.TtoW0 = mul(rotation, _Object2World[0].xyz);
	o.TtoW1 = mul(rotation, _Object2World[1].xyz);
	o.TtoW2 = mul(rotation, _Object2World[2].xyz);
	float3 dir = mul ( (float3x3)_World2Object, _LightmapDir);
	o.lightDirT = mul( rotation, dir );	

	return o;
}

uniform sampler2D _BumpMap : register(s0);
uniform sampler2D _MainTex : register(s1);
uniform sampler2D _LightMap : register(s2);
uniform samplerCUBE _SpecCube : register(s3);

float4 frag (v2f i) : COLOR
{
	float4 texcol = tex2D(_MainTex,i.uv.xy);
	float4 lightmap = tex2D (_LightMap, i.uv3) * _LightmapColor;

	// get normal from the normal map
	float3 normal = tex2D(_BumpMap, i.uv.xy).xyz * 2 - 1;

	// transform normal to world space
	half3 wn;
	wn.x = dot(i.TtoW0, normal.xyz);
	wn.y = dot(i.TtoW1, normal.xyz);
	wn.z = dot(i.TtoW2, normal.xyz);
	
	// calculate reflection vector in world space
	half3 r = reflect(i.I, wn);
	
	half4 reflcolor = texCUBE(_SpecCube, r).rgbr * _SpecColor * texcol.a;
	return ((dot (i.lightDirT, normal) + UNITY_LIGHTMODEL_AMBIENT) * texcol * _Color + reflcolor) * lightmap;
}
ENDCG  
			SetTexture [_BumpMap] {combine texture}
			SetTexture [_MainTex] {combine texture}
			SetTexture [_LightMap] {combine texture}
			SetTexture [_SpecCube] {combine texture}
		}
		UsePass " BumpedSpecular/PPL"
	}
}

Fallback "Fallback Lightmapped VertexLit"
}