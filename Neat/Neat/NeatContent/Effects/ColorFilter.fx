sampler  TextureSampler  : register(s0);

float4 color = float4(1,1,1,1);

float4 TintFunction(float4 c : COLOR0, float2 uv : TEXCOORD0) : COLOR
{
	float4 col = c * tex2D(TextureSampler,uv);
	return col*color;
}

float4 ReplaceFunction() : COLOR
{
	return color;
}

float4 BalanceFunction(float4 tint : COLOR0, float2 uv : TEXCOORD0) : COLOR
{
	float4 p = tint * tex2D(TextureSampler,uv) * color;
	float4 c = max(p.r, max(p.g, p.b));
	p.rgb /= c.rgb;
	p.a *= color.a;
	return p;
}

float4 AddFunction(float4 c : COLOR0, float2 uv : TEXCOORD0) : COLOR
{
	float4 col = c*tex2D(TextureSampler,uv);
	return col+color;
}

float4 SubFunction(float4 c : COLOR0, float2 uv : TEXCOORD0) : COLOR
{
	float4 col = c*tex2D(TextureSampler,uv);
	return color-col;
}

technique ColorTint
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 TintFunction();
    }
}

technique ColorReplace
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 ReplaceFunction();
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

technique ColorAdd
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 AddFunction();
    }
}

technique ColorSub
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 SubFunction();
    }
}