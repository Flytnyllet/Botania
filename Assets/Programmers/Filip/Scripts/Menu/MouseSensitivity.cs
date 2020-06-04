using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    [SerializeField] MenuNumberString[] _numberStrings;

    [SerializeField] TMP_Text _value;
    Slider _slider;

    bool _first = true;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (!_first)
        {
            _slider.value = Player.GetSensitivity();
            ChangeSensitivity();
        }
    }

    //This is just to make sure Player is initialized before setting
    private void Start()
    {
        if (_first)
        {
            _slider.value = Player.GetSensitivity();
            ChangeSensitivity();

            _first = false;
        }
    }

    public void ChangeSensitivity()
    {
        Player.SetSensitivity(_slider.value);
        _value.text = GetSensitivityName();
    }

    string GetSensitivityName()
    {
        for (int i = 0; i < _numberStrings.Length; i++)
        {
            if (_slider.value < _numberStrings[i].number)
                return _numberStrings[i].name;
        }

        return string.Empty;
    }
}
