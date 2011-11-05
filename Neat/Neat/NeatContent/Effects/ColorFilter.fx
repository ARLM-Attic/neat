sampler  TextureSampler  : register(s0);

float4 mulColor = float4(1,1,1,1);

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 col = tex2D(TextureSampler,uv);
	return col*mulColor;
}

float4 ReplacePixelShaderFunction() : COLOR
{
	return mulColor;
}

float4 BalanceFunction(float2 uv : TEXCOORD) : COLOR
{
	float4 p = mulColor;// float4(abs(mulColor.r), abs(mulColor.g), abs(mulColor.b), abs(mulColor.a));// float4(1,1,1,1);// abs(mulColor);
	float4 c = max(p.r, max(p.g, p.b));
	p.rgb /= c.rgb;
	p.a = mulColor.a;
	return p;
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