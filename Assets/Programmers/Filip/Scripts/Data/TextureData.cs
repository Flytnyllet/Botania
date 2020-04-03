using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New TextureData", menuName = "Generation/TextureData")]
public class TextureData : UpdatableData
{
    readonly static int TEXTURE_SIZE = 512;
    readonly static TextureFormat TEXTURE_FORMAT = TextureFormat.RGB565;

    [SerializeField] Layer[] _layers;


    float _savedMinHeight;
    float _savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", _layers.Length);
        material.SetColorArray("baseColors", _layers.Select(x => x.GetTint()).ToArray());
        material.SetFloatArray("baseStartHeights", _layers.Select(x => x.GetStartHeight()).ToArray());
        material.SetFloatArray("baseBlends", _layers.Select(x => x.GetBlendStrength()).ToArray());
        material.SetFloatArray("baseColorStrength", _layers.Select(x => x.GetTintStrength()).ToArray());
        material.SetFloatArray("baseTextureScales", _layers.Select(x => x.GetTextureScale()).ToArray());
        Texture2DArray textureArray = GenerateTextureArray(_layers.Select(x => x.GetTexture()).ToArray());
        material.SetTexture("baseTextures", textureArray);

        UpdateMeshHeights(material, _savedMaxHeight, _savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        _savedMinHeight = minHeight;
        _savedMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
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
        [SerializeField] Texture2D _texture;
        [SerializeField] Color _tint;

        [SerializeField, Range(0, 1)] float _tintStrength;
        [SerializeField, Range(0, 1)] float _startHeight;
        [SerializeField, Range(0, 1)] float _blendStrength;
        [SerializeField, Range(0, 1)] float _textureScale;

        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTintStrength() { return _tintStrength; }
        public float GetStartHeight() { return _startHeight; }
        public float GetBlendStrength() { return _blendStrength; }
        public float GetTextureScale() { return _textureScale; }
    }
}
