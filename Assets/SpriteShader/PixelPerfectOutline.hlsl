#ifndef PixelPerfectOutline
#define PixelPerfectOutline


float4 _OutlineColor;
float _Radius;

float CalculateLUTOutline(float4 texColor, float2 uv, Texture2D tex, SamplerState st, float4 texelSize)
{
	float na = 0;
	float r = _Radius;

	for (int nx = -r; nx <= r; nx++)
	{
		for(int ny = -r; ny <= r; ny++)
		{
			if(nx * nx + ny * ny <= r) // r * r is ugly
				{
				float4 nc = SAMPLE_TEXTURE2D(tex, st, uv +
					float2(texelSize.x * nx, texelSize.y * ny));
				na += ceil(nc.z); // alpha is 1 always in 16bit texture
				// if that texture is filled with color
				// na increases
				}
		}
	}

	na = clamp(na, 0, 1);
	na -= ceil(texColor.a);

	return na;
}

float CalculateOutline(float4 texColor, float2 uv, Texture2D tex, SamplerState st, float4 texelSize)
{
	float na = 0;
	float r = _Radius;

	for (int nx = -r; nx <= r; nx++)
	{
		for(int ny = -r; ny <= r; ny++)
		{
			if(nx * nx + ny * ny <= r) // r * r is ugly
				{
				float4 nc = SAMPLE_TEXTURE2D(tex, st, uv +
					float2(texelSize.x * nx, texelSize.y * ny));
				na += ceil(nc.a); 
				// if that texture is filled with color
				// na increases
				}
		}
	}

	na = clamp(na, 0, 1);
	na -= ceil(texColor.a);

	return na;
}

#endif

