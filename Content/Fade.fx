#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Texture2D SpriteTexture;
Texture2D fadeTexture;
Texture2D paletteTexture;
float fadeAmount;
float paletteY;
bool flip;

//sampler2D palette_sampler= sampler_state
//{
//    Texture = <palette>;
//};


//sampler2D SpriteTextureSampler = sampler_state
//{
//    Texture = <SpriteTexture>;
//};
//
sampler fade_sampler : register(s1)
{ 
    Texture = (fadeTexture);
    Filter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler palette_sampler : register(s2)
{ 
    Texture = (paletteTexture);
    Filter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 Indexed(VertexShaderOutput input) : COLOR
{
    //float4 textureColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);
    float4 fadeColor = tex2D(fade_sampler,input.TextureCoordinates);
    //float2 uv = fadeColor.rg;
    //uv.x = 1.0f;
    //uv.y = paletteY;
    float4 outColor = tex2D(palette_sampler, float2(1.0f,paletteY));
    float4 color = fadeAmount < fadeColor.r ? (0, 1, 0, 0) : outColor;

    if (flip)
        color = fadeAmount < fadeColor.r ? outColor : (0,0,0,0);
        //color = fadeAmount > (1.0f - fadeColor.r) ? (0,0,0,1) : (0,0,0,0);

    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL
        Indexed();
    }
};
