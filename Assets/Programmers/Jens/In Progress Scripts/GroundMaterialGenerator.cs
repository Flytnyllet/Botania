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

    public Material MakeMaterial(int size, Vector2 pos)
    {
        //int ChunkCoordX = Mathf.RoundToInt(pos.x / (meshObject.mesh.bounds.size.x));
        //int ChunkCoordZ = Mathf.RoundToInt(pos.z / (meshObject.mesh.bounds.size.z));
        //Vector2 coord = new Vector2(ChunkCoordX, ChunkCoordZ);
        Material material = new Material(_shader);
        ////Task.Run(() =>
        ////{
        //int size = (int)(meshObject.mesh.bounds.size.x * meshObject.transform.localScale.x);
        //size /= 10;
        size *= 1;
        Debug.Log(size);
        Debug.Log(pos);
        
        
        

        var mainThread = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Run(() =>
        {
            try
            {
                float[,] noise = Noise.GenerateNoiseMap(size, size, _noiseDetailLevel, _layers[0].GetNoise.NoiseSettingsDataMerge, pos*1  );
                Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(previous =>
                {
                    Texture2D noiseTex = TextureGenerator.TextureFromNoiseJens(noise);
                    material.SetTexture("_NoiseTextures", noiseTex);
                }, mainThread);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        });
        //noise = Noise.GenerateNoiseMap(size, size, _noiseDetailLevel, _layers[0].GetNoise.NoiseSettingsDataMerge, pos * 500);
        //Texture2D noiseTex = TextureGenerator.TextureFromNoise(noise);
        //material.SetTexture("_NoiseTextures", noiseTex);

        material.SetTexture("_AltTex", _layers[0].GetTexture());
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

        public NoiseSettingsData GetNoise { get => _noise; }
        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTextureScale() { return _textureScale; }
        public float GetTextureStrenght() { return _textureStrenght; }
    }

}
