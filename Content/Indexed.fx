#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
Texture2D palette;
float time;
float swap;

//sampler2D palette_sampler= sampler_state
//{
//    Texture = <palette>;
//};

sampler palette_sampler : register(s1)
{ 
    Texture = (palette);
    Filter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 Indexed(VertexShaderOutput input) : COLOR
{
    float4 textureColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);
    float2 uv = textureColor.rg;
    uv.y = time;

    return tex2D(palette_sampler, float2(uv.x*swap,time));
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL
        Indexed();
    }
};