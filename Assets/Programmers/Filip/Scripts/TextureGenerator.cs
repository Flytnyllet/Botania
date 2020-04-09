using System.Collections;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point; //KANSKE VILL ÄNDRAS SENARE!!!

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromNoise(float[,] noise)
    {
        int width = noise.GetLength(0);
        int height = noise.GetLength(1);

        Color[] colorMap = new Color[width * height];

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (noise[x, y] < minValue)
                    minValue = noise[x, y];
                if (noise[x, y] > maxValue)
                    maxValue = noise[x, y];
            }
        }

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(minValue, maxValue, noise[x, y]));

        return TextureFromColorMap(colorMap, width, height);
    }
}
