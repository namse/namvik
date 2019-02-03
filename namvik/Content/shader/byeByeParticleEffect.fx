#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_5_0
#else
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

static const int SAMPLE_COUNT = 5;
static const float GaussianBlurWeight[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

int TextureWidth;
int TextureHeight;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	int2 textureSize = int2(TextureWidth, TextureHeight);

	float4 result = tex2D(TextureSampler, input.TextureCoordinates) * GaussianBlurWeight[0];

	for (int y = 1; y < SAMPLE_COUNT; y++)
	{
		for (int x = 1; x < SAMPLE_COUNT; x++)
		{
			for (int ySign = -1; ySign <= 1; ySign += 2)
			{
				for (int xSign = -1; xSign <= 1; xSign += 2)
				{
					float2 textureCoordinates = input.TextureCoordinates + float2(xSign * x, ySign * y) / textureSize;
					float4 value = tex2D(TextureSampler, textureCoordinates);
					result += value * GaussianBlurWeight[x] * GaussianBlurWeight[y];
				}
			}
		}
	}

	return result;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};