using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterWater : MonoBehaviour
{
    [Header("Settings")]
    Transform _camera;
    Color _colour;
    [SerializeField] Color _skyboxColour;
    [SerializeField] float _seaLevel;

    [Header("Drop")]

    [SerializeField] GameObject[] _toggleOnGameObjects;
    [SerializeField] GameObject[] _toggleOffGameObjects;

    bool _isInWater = false;
    private void Start()
    {
        _camera = Camera.main.transform;
        _colour = RenderSettings.fogColor;
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

        if (status)
        {
            RenderSettings.fogColor = _skyboxColour;// new Color(0.384f, 0.322f, 0.549f); //Magic background colour numbers
            RenderSettings.fogEndDistance = 10;
        }

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
