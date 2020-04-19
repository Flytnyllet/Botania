using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TempGroundTextureGenerator : MonoBehaviour
{
    readonly static int TEXTURE_SIZE = 512;
    readonly static TextureFormat TEXTURE_FORMAT = TextureFormat.RGB565;


    [SerializeField] Layer[] _layers;
    [SerializeField] Shader _shader;
    [SerializeField] int _noiceDetailLevel = 1;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //MakeMaterial();
        }
    }

    void MakeMaterial(Vector2Int resolution, int detailLevel, Vector2 chunkCoord)
    {
        Material material = new Material(_shader);
        Texture2D[] noises = new Texture2D[_layers.Length];

        for (int i = 0; i < _layers.Length; i++)
        {
            float[,] noise = Noise.GenerateNoiseMap(resolution.x, resolution.y, _noiceDetailLevel, _layers[i].GetNoise.NoiseSettingsDataMerge, chunkCoord);
            noises[i] = TextureGenerator.TextureFromNoise(noise);
        }
        material.SetInt("layerCount", _layers.Length);
        Texture2DArray textureArray = GenerateTextureArray(_layers.Select(x => x.GetTexture()).ToArray());
        material.SetTexture("baseTextures", textureArray);
        material.SetColorArray("baseColors", _layers.Select(x => x.GetTint()).ToArray());
        material.SetFloatArray("baseTextureScales", _layers.Select(x => x.GetTextureScale()).ToArray());
        //material.SetFloatArray("noiseX", noise[  ])

        GetComponent<MeshRenderer>().material = material;
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
        [SerializeField] Color _tint;
        [SerializeField, Range(0, 1)] float _textureScale;

        public NoiseSettingsData GetNoise => _noise;
        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTextureScale() { return _textureScale; }
    }

}
