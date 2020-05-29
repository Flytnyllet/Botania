using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ChangeResolution : MonoBehaviour
{
    TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        AddResolutionAsOptions();
    }

    private void AddResolutionAsOptions()
    {
        Resolution currentResolution = Screen.currentResolution;

        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
                currentResolutionIndex = i;
        }

        _dropdown.AddOptions(options);
        _dropdown.SetValueWithoutNotify(currentResolutionIndex);
    }

    public void DoChangeResolution()
    {
        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        Screen.SetResolution(resolutions[_dropdown.value].width, resolutions[_dropdown.value].height, Screen.fullScreen);
    }
}
