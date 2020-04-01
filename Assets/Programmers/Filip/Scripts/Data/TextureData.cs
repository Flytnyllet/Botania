using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TextureData", menuName = "Generation/TextureData")]
public class TextureData : UpdatableData
{
    [SerializeField] Color[] _baseColors;
    [SerializeField, Range(0, 1)] float[] _baseStartHeights;

    float _savedMinHeight;
    float _savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        UpdateMeshHeights(material, _savedMaxHeight, _savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetInt("baseColorCount", _baseColors.Length);
        material.SetColorArray("baseColors", _baseColors);
        material.SetFloatArray("baseStartHeights", _baseStartHeights);

        _savedMinHeight = minHeight;
        _savedMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
