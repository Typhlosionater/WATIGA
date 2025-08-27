sampler image0 : register(s0);

float2 textureSize;

float2 snapToGrid(float2 coords) {
    float2 pixelCoords = floor(coords * textureSize);
    float2 snappedCoords = floor(pixelCoords / 2) * 2;
    float2 snappedUV = (snappedCoords + float2(0.5, 0.5)) / textureSize;
    return snappedUV;
}

float4 Metaball(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = snapToGrid(coords);

    float4 color = tex2D(image0, coords) * sampleColor;
    if (any(color)) {
        return color;
    }

    float sum = 0;
    float4 sumOfNeighbors = float4(0, 0, 0, 1);
    float2 pixelCoords = coords * textureSize;
    float radius = 12;
    for (int y = -radius; y <= radius; y++) {
        for (int x = -radius; x <= radius; x++) {
            float2 pixelCoords = coords * textureSize;
            float2 offsetPixelCoords = pixelCoords + float2(x, y);
            float2 offsetCoords = offsetPixelCoords / textureSize;

            float4 nearbyColor = tex2D(image0, offsetCoords);
            if (any(nearbyColor)) {
                sumOfNeighbors += nearbyColor;
                sum++;
            }
        }
    }

    if (sum > 300) {
        float4 average = sumOfNeighbors / sum;
        average.a = 1;
        return average;
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
