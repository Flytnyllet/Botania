using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FOVSetting : MonoBehaviour
{
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
            _slider.value = Player.GetPlayerCamera().fieldOfView;
            ChangeFOV();
        }
    }

    //This is just to make sure Player is initialized before setting
    private void Start()
    {
        if (_first)
        {
            _slider.value = Player.GetPlayerCamera().fieldOfView;
            ChangeFOV();

            _first = false;
        }
    }

    public void ChangeFOV()
    {
        Player.GetPlayerCamera().fieldOfView = _slider.value;
        _value.text = ((int)_slider.value).ToString();
    }
}
