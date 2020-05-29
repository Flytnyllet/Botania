using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FOVSetting : MonoBehaviour
{
    Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void ChangeFOV()
    {
        Player.GetPlayerCamera().fieldOfView = _slider.value;
    }
}
