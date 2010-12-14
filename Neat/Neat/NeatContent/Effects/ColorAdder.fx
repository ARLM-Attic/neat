sampler  TextureSampler  : register(s0);

float4 ovlColor;

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);
	return col+ovlColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}