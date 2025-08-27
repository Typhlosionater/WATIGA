sampler image0 : register(s0);

float2 textureSize;
float2 pixelSize;

float2 snapToGrid(float2 coords) {
    float2 pixelCoords = floor(coords * textureSize);
    float2 snappedCoords = floor(pixelCoords / 2) * 2;
    float2 snappedUV = (snappedCoords + float2(0.5, 0.5)) / textureSize;
    return snappedUV;
}

float2 offsetAndSnapToGrid(float2 coords, float2 pixelOffset) {
    float2 pixelCoords = floor(coords * textureSize);
    float2 snappedCoords = floor((pixelCoords) / 2) * 2;
    float2 offsetCoords = snappedCoords + pixelOffset;
    float2 snappedUV = (offsetCoords + float2(0.5, 0.5)) / textureSize;
    return snappedUV;
}

float4 Metaball(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = snapToGrid(coords);

    float4 color = tex2D(image0, coords) * sampleColor;
    if (any(color)) {
        return color;
    }

    float4 left = tex2D(image0, coords - float2(2 * pixelSize.x, 0));
    float4 right = tex2D(image0, coords + float2(2 * pixelSize.x, 0));
    float4 top = tex2D(image0, coords - float2(0, 2 * pixelSize.y));
    float4 bottom = tex2D(image0, coords + float2(0, 2 * pixelSize.y));
    float all = left + right + top + bottom;
    if (any(all)) {
        return float4(0, 0, 0, 1);
    }

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 Metaball();
    }
}
