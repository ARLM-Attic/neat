sampler  TextureSampler  : register(s0);

float phase;
float amplitude;
float4 tint = float4(1,1,1,1);
float2 uvShift = float2(0,0);
void PixelShaderFunction(inout float4 c : COLOR0, float2 uv : TEXCOORD)
{
	float2 p = uv * 2.0f - 1.0f;
	float wl = length(p);
	c *= tint * tex2D(TextureSampler, frac(uvShift + uv + (p/wl)*0.03*(cos(wl*12.0-phase*4.0))));
	//float2 newPos = amplitude * float2(cos(phase),sin(phase)) + uv;
	//return tex2D(TextureSampler,newPos) + tex2D(TextureSampler,uv);
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
