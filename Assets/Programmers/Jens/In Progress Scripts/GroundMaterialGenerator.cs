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

    [SerializeField] Layer[] _layers;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] Shader _shader;
    [SerializeField] int _noiseDetailLevel = 1;
    [SerializeField] Texture2D _mainTex;
    // Update is called once per frame

    public Material MakeMaterial(Vector2 chunkCoord)
    {
        //Vector2 pos = chunkCoord * _meshSettings.MeshWorldSize / _meshSettings.MeshScale;
        Material material = new Material(_shader);
        TaskScheduler mainThread = TaskScheduler.FromCurrentSynchronizationContext();
        //Task.Run(() =>
        //{
        float[,] noise = Noise.GenerateNoiseMap(TEXTURE_SIZE, TEXTURE_SIZE, _noiseDetailLevel, _layers[0].GetNoise.NoiseSettingsDataMerge, chunkCoord);
        Texture2D noiseTex = TextureGenerator.TextureFromNoise(noise);
        material.SetTexture("_NoiseTextures", noiseTex);
        //});
        material.SetInt("layerCount", _layers.Length);
        Texture2DArray textureArray = GenerateTextureArray(_layers.Select(x => x.GetTexture()).ToArray());
        material.SetTexture("baseTextures", textureArray);
        material.SetTexture("_MainTex", _mainTex);
        material.SetColorArray("baseColors", _layers.Select(x => x.GetTint()).ToArray());
        material.SetFloatArray("baseTextureScales", _layers.Select(x => x.GetTextureScale()).ToArray());
        material.SetFloatArray("baseTextureStrenght", _layers.Select(x => x.GetTextureStrenght()).ToArray());

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

        public NoiseSettingsData GetNoise => _noise;
        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTextureScale() { return _textureScale; }
        public float GetTextureStrenght() { return _textureStrenght; }
    }

}
