sampler image0 : register(s0);
sampler noise : register(s1);

float time;

float4 insideColor(float4 sampledColor, float2 coords) {
    return sampledColor;
}

float4 outlineColor(float4 sampledColor, float2 coords) {
    float2 coords1 = coords + float2(time * 0.1, 0);
    float2 coords2 = coords + float2(0, time * 0.07);
    float noise1 = tex2D(noise, coords1).r;
    float noise2 = tex2D(noise, coords2).r;

    float noise = lerp(noise1, noise2, 0.5);
    float mult = lerp(0.8, 1.2, noise);
    return float4(0.3, 0.01, 0.01, 1) * mult;
}

float4 AlphaCentauriMetaball(float4 samplerColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(image0, coords) * samplerColor;

    if (all(abs(color.rgb) < 1e-3) && color.a > (1 - 1e-3)) {
        return outlineColor(color, coords);
    }

    if (any(color)) {
        return insideColor(color, coords);
    }

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 AlphaCentauriMetaball();
    }
}
