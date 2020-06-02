using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterWater : MonoBehaviour
{
    [Header("Settings")]
    Transform _camera;
    [SerializeField] float _seaLevel;

    [Header("Drop")]

    [SerializeField] GameObject[] _toggleOnGameObjects;
    [SerializeField] GameObject[] _toggleOffGameObjects;

    bool _isInWater = false;
    private void Start()
    {
        _camera = Camera.main.transform;
    }
    private void Update()
    {
        if (_camera.position.y <= _seaLevel && !_isInWater)
            ToggleWater(true);
        else if (_camera.position.y > _seaLevel && _isInWater)
            ToggleWater(false);
    }

    void ToggleWater(bool status)
    {
        _isInWater = status;

        for (int i = 0; i < _toggleOnGameObjects.Length; i++)
        {
            _toggleOnGameObjects[i].SetActive(status);
        }

        for (int i = 0; i < _toggleOffGameObjects.Length; i++)
        {
            _toggleOffGameObjects[i].SetActive(!status);
        }
    }
}
