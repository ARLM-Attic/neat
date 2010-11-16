sampler  TextureSampler  : register(s0);

float2 a;
float2 b;
float2 c;
float4 FillColor;
float2 Bounds;

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);

	float2 A = float2(a[0]/Bounds[0], a[1]/Bounds[1]);
	float2 B = float2(b[0]/Bounds[0], b[1]/Bounds[1]);
	float2 C = float2(c[0]/Bounds[0], c[1]/Bounds[1]);

	float2 v0 = C - A;
	float2 v1 = B - A;
	float2 v2 = float2(uv.x, uv.y) - A;

	float2 dot00 = dot(v0,v0);
	float2 dot01 = dot(v0,v1);
	float2 dot02 = dot(v0,v2);
	float2 dot11 = dot(v1,v1);
	float2 dot12 = dot(v1,v2);

	float invDenom, u, v;

	invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
	u = (dot11 * dot02 - dot01 * dot12) * invDenom;
	v = (dot00 * dot12 - dot01 * dot02) * invDenom;

	float4 finalColor = col * FillColor;

	if ((u > 0) && (v > 0) && (u + v < 1)) return finalColor;
	else return col;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
