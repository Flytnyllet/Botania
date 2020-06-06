using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text;

public class ChangeResolution : MonoBehaviour
{
    [SerializeField] GameObject _revertWindow;
    [SerializeField, Range(1, 50)] float _revertTime = 10;
    [SerializeField] TMP_Text _revertText;

    TMP_Dropdown _dropdown;
    Vector2Int _lastResolution;
    bool _currentlyCountingDown = false;

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
        _revertWindow.SetActive(false);
    }

    private void Start()
    {
        AddResolutionAsOptions();
    }

    private void AddResolutionAsOptions()
    {
        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " " + GetAspectRatio(resolutions[i]));

        _dropdown.AddOptions(options);
        _dropdown.SetValueWithoutNotify(GetResolutionIndex(Screen.width, Screen.height));
    }

    public void DoChangeResolution(bool final = false)
    {
        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        if (!final)
        {
            _revertWindow.SetActive(true);
            _currentlyCountingDown = true;
            StartCoroutine(CountDownToRevert());

            _lastResolution = new Vector2Int(Screen.width, Screen.height);
        }

        Screen.SetResolution(resolutions[_dropdown.value].width, resolutions[_dropdown.value].height, Screen.fullScreen);
    }

    public void RevertResolution()
    {
        _currentlyCountingDown = false;
        _revertWindow.SetActive(false);
        _dropdown.SetValueWithoutNotify(GetResolutionIndex(_lastResolution.x, _lastResolution.y));
        DoChangeResolution(true);
    }

    public void KeepResolution()
    {
        _revertWindow.SetActive(false);
        _currentlyCountingDown = false;
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

    int GetResolutionIndex(int width, int height)
    {
        Resolution[] resolutions = (Resolution[])Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
                return i;
        }

        Debug.LogError("This resolution doesn't match supported resolutions!?: " + width + " x " + height);
        return resolutions.Length - 1;
    }

    IEnumerator CountDownToRevert()
    {
        Timer countDownTimer = new Timer(_revertTime);

        while (_currentlyCountingDown)
        {
            yield return null;
            countDownTimer.Time += Time.deltaTime;

            _revertText.text = "Reverting in " + ((int)(_revertTime - countDownTimer.Time + 1)).ToString() + " Seconds...";

            if (countDownTimer.Expired())
                RevertResolution();
        }
    }
}
