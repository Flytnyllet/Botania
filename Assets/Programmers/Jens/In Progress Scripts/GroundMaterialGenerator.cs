using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System;

[CreateAssetMenu(menuName = "MaterialMaker")]
public class GroundMaterialGenerator : ScriptableObject
{
    readonly static int TEXTURE_SIZE = 512;
    readonly static TextureFormat TEXTURE_FORMAT = TextureFormat.RGB565;

    [SerializeField] Layer _layer;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] Shader _shader;
    [SerializeField] int _noiseDetailLevel = 1;
    [SerializeField] Texture2D _mainTex;
    // Update is called once per frame
    object obj;

    public Material MakeMaterial(int size, Vector2 pos)
    {
        Material material = new Material(_shader);
        Texture2D noiseTex = new Texture2D(size, size);
        Color[] colorMap = new Color[1];

        Task.Run(() =>
        {
            try
            {
                colorMap = new Color[size * size];
                float[,] noise = Noise.GenerateNoiseMap(size, size, _noiseDetailLevel, _layer.GetNoise.NoiseSettingsDataMerge, pos);


                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        colorMap[y * size + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, 1, noise[x, y]));

                ThreadedDataRequester.AddToCallbackQueue(() =>
                {
                    noiseTex.SetPixels(colorMap);
                    noiseTex.Apply();
                    material.SetTexture("_NoiseTextures", noiseTex);

                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        });
        material.SetTexture("_AltTex", _layer.GetTexture());
        material.SetTexture("_MainTex", _mainTex);
        material.SetColor("baseColor", _layer.GetTint());
        material.SetFloat("baseTextureScale", _layer.GetTextureScale());
        return material;

    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        //DU FÅR GÄRNA TA BORT DETTA SEN, ÄR BARA HÄR FÖR HAR INGA TEXTURES O FÅR NULL_EXCEPTION
        if (textures[0] == null)
            return null;

        Texture2DArray textureArray = new Texture2DArray(TEXTURE_SIZE, TEXTURE_SIZE, textures.Length, TEXTURE_FORMAT, true);

        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    [System.Serializable]
    public class Layer
    {
        [SerializeField] NoiseSettingsData _noise;
        [SerializeField] Texture2D _texture;
        [SerializeField, Range(0, 1)] float _textureStrenght;
        [SerializeField] Color _tint;
        [SerializeField, Range(0, 1)] float _textureScale;

        public NoiseSettingsData GetNoise { get => _noise; }
        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTextureScale() { return _textureScale; }
        public float GetTextureStrenght() { return _textureStrenght; }
    }

}
