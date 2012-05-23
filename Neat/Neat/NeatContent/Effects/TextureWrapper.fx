sampler TextureSampler  : register(s0);

float2 texRelativeSize = float2(1,1);
float2 sliceRelativeSize = float2(1,1);
float2 texPos = float2(0,0);

bool wrapX = true;
bool wrapY = true;

void WrapFunction(inout float4 color : COLOR0, float2 uv : TEXCOORD)
{
	float2 wrappedUv = texPos +
	fmod(
		(uv-texPos)/texRelativeSize, 
		sliceRelativeSize
		);
	
	color *= tex2D(TextureSampler, float2(
		(wrapX ? wrappedUv.x : uv.x),
		(wrapY ? wrappedUv.y : uv.y)
		));
}

technique TextureWrap
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_2_0 WrapFunction();
    }
}