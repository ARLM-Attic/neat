sampler  TextureSampler  : register(s0);
float top;
float down;
float left;
float right;

float4 CropFunction(float2 uv : TEXCOORD) : COLOR
{
	if (uv.x >= left && uv.x <= right && uv.y >= top && uv.y <= down)
		return tex2D(TextureSampler,uv);
	return float4(0,0,0,0);
}

technique ColorReplace
{
   pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 CropFunction();
    }
}