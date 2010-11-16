sampler  TextureSampler  : register(s0);

float4 mulColor;

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);

	//col.r = 0;
	//col.g = 0;
	//col.b = 0;

	return col*mulColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
