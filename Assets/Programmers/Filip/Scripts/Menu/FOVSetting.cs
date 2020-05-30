using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FOVSetting : MonoBehaviour
{
    [SerializeField] TMP_Text _value;
    Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _slider.value = Player.GetPlayerCamera().fieldOfView;
        ChangeFOV();
    }

    public void ChangeFOV()
    {
        Player.GetPlayerCamera().fieldOfView = _slider.value;
        _value.text = ((int)_slider.value).ToString();
    }
}
