using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GraphicsQuality : MonoBehaviour
{
    TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        AddOptions();
    }

    private void AddOptions()
    {
        List<string> options = QualitySettings.names.ToList<string>();

        _dropdown.AddOptions(options);
        _dropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(_dropdown.value);
    }
}
