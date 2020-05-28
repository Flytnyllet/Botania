using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashLightColourChanger : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] float _speed = 1;
    [SerializeField] Color _colA;
    [SerializeField] Color _colB;

    // Update is called once per frame
    void Update()
    {
        float value = (Mathf.Sin(Time.time * _speed) + 1) * 0.5f;
        _light.color = Color.Lerp(_colA, _colB, value);
    }
}
