#ifndef PixelPerfectOutline
#define PixelPerfectOutline

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_TexelSize;

float4 _OutlineColor;
float _Radius;


float CalculateOutline(float4 texColor, float2 uv)
{
	float na = 0;
	float r = _Radius;

	for (int nx = -r; nx <= r; nx++)
	{
		for(int ny = -r; ny <= r; ny++)
		{
			if(nx * nx + ny * ny <= r) // r * r is ugly
				{
				float4 nc = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv +
					float2(_MainTex_TexelSize.x * nx, _MainTex_TexelSize.y * ny));
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

