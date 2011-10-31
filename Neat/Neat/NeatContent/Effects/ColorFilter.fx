sampler  TextureSampler  : register(s0);

float4 mulColor;

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);
	return col*mulColor;
}

float4 ReplacePixelShaderFunction() : COLOR
{
	return normalize(mulColor);
}

float4 BalanceFunction() : COLOR
{
	float4 positive = abs(mulColor);
	float4 c = max(positive.r, max(positive.g, positive.b));
	return float4(saturate(positive.r / c.r), saturate(positive.g / c.g), saturate(positive.b / c.b), mulColor.a);
}

technique ColorFilter
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique ColorReplace
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 ReplacePixelShaderFunction();
    }
}

technique ColorBalance
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 BalanceFunction();
    }
}