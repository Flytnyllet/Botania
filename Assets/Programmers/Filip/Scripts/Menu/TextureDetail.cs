using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TextureDetail : MonoBehaviour
{
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
        _dropdown.SetValueWithoutNotify(_options.Count - QualitySettings.masterTextureLimit);
    }

    public void ChangeTextureDetail()
    {
        QualitySettings.masterTextureLimit = (_options.Count - _dropdown.value - 1);
    }
}
