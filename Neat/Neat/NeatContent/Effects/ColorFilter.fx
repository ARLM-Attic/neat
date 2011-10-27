sampler  TextureSampler  : register(s0);

float4 mulColor;

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);
	return col*mulColor;
}

technique Technique1
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
