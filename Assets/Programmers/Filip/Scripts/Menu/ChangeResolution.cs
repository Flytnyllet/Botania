using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text;

public class ChangeResolution : MonoBehaviour
{
    TMP_Dropdown _dropdown;

    Vector2[] _aspectRatios = new Vector2[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(3, 2),
        new Vector2Int(4, 3),
        new Vector2Int(5, 4),
        new Vector2Int(16, 10),
        new Vector2Int(16, 9),
        new Vector2(16, 9.6f),
        new Vector2Int(21, 9)
    };

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        AddResolutionAsOptions();
    }

    private void AddResolutionAsOptions()
    {
        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " " + GetAspectRatio(resolutions[i]));

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
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

    string GetAspectRatio(Resolution resolution)
    {
        float epsilon = 0.05f;

        float ratio = (float)resolution.width / resolution.height;

        for (int i = 0; i < _aspectRatios.Length; i++)
        {
            float thisRatio = (float)_aspectRatios[i].x / _aspectRatios[i].y;

            if (thisRatio > ratio - epsilon && thisRatio < ratio + epsilon)
                return "(" + _aspectRatios[i].x + ":" + _aspectRatios[i].y + ")";
        }

        Debug.LogError("This aspect ratio is currently not supported!");
        return string.Empty;
    }
}
