using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RenderDistance : MonoBehaviour
{
    [SerializeField] TerrainGenerator _terrainGeneratorScript;
    [SerializeField] List<string> _options;

    TMP_Dropdown _dropdown;

    int _optionsCount;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        AddOptions();
    }

    private void AddOptions()
    {
        _dropdown.AddOptions(_options);
        _dropdown.SetValueWithoutNotify(TerrainGenerator.RenderDistanceIndex);
    }

    public void ChangeRenderDistance()
    {
        _terrainGeneratorScript.ChangeRenderDistance(_dropdown.value);
    }
}
