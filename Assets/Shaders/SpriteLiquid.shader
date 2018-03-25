// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Liquid"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		_WaveSize("Wave Size", Float) = 1
		_WaveTime("Wave Time", Float) = 1
		_WaveHeight("Wave Height", Float) = 1
		_WaveSize2("Wave Size", Float) = 1
		_WaveTime2("Wave Time", Float) = 1
		_WaveHeight2("Wave Height", Float) = 1
		_Fill("Fill Amount", Range(0.0, 1.0)) = .5

		// required for UI.Mask
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		// required for UI.Mask
		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}
		ColorMask[_ColorMask]

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
	};

	fixed4 _Color;
	half _WaveSize;
	half _WaveTime;
	half _WaveHeight;
	half _WaveSize2;
	half _WaveTime2;
	half _WaveHeight2;
	half _Fill;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;

	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		if (_AlphaSplitEnabled)
			color.a = tex2D(_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		float2 modifiedTexCoord = float2 (IN.texcoord.x, IN.texcoord.y + 
			max(_WaveHeight * sin(_WaveSize * IN.texcoord.x + _WaveTime * _Time.y),
				_WaveHeight2 * sin(_WaveSize2 * IN.texcoord.x + _WaveTime2 * -_Time.y)
			)
		);
		if (modifiedTexCoord.y < _Fill) {
			fixed4 c = SampleSpriteTexture(modifiedTexCoord) * IN.color;
			c.rgb *= c.a;
			return c;
		}
		else {
			return half4 (0, 0, 0, 0);
		}
	}
		ENDCG
	}
	}
}

