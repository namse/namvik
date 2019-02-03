#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

Texture2D ZigZagTexture;
sampler2D ZigZagTextureSampler = sampler_state
{
	Texture = <ZigZagTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 temp = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 zigZag = tex2D(ZigZagTextureSampler, input.TextureCoordinates);

	float x = input.TextureCoordinates.x + (zigZag.x - 0.5) * 0.2;
	if (x > 1 || x < 0) {
		return float4(0, 0, 0, 0);
	}

	float4 next = tex2D(SpriteTextureSampler, float2(x, input.TextureCoordinates.y));
	return next;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};