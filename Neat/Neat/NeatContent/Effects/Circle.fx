sampler  TextureSampler  : register(s0);
float radius = 0.5;// 1.4142135623730951f;
float4 color = float4(1,1,1,1);
float thickness = 0.01;
void StrokeFunction(inout float4 c : COLOR0, float2 uv : TEXCOORD)
{
	float d = distance(uv,float2(0.5f,0.5f));
	if (d<=radius && d>=radius-thickness) 
		//c *= color;
		c *= color * float4(1,1,1, 1-abs((d-radius)/thickness));
	else c = float4(0,0,0,0);
}

technique DrawCircle
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 StrokeFunction();
    }
}


void FillFunction(inout float4 c : COLOR0, float2 uv : TEXCOORD)
{
	if (distance(uv,float2(0.5f,0.5f))<radius) c *= color;
	else c = float4(0,0,0,0);
}

technique FillCircle
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 FillFunction();
    }
}
