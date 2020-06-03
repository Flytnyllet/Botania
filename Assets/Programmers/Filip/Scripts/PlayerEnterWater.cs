using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterWater : MonoBehaviour
{
    [Header("Settings")]
    Camera _camera;
    [SerializeField] Color _colour;
    [SerializeField] Material _underWaterEffect;
    [SerializeField] CameraEffect _cameraEffects;
    [SerializeField] LayerMask _underWaterCullingMask;
    LayerMask _surfaceWaterCullingMask;
    [SerializeField] float _seaLevel;

    [Header("Drop")]

    [SerializeField] GameObject[] _toggleOnGameObjects;
    [SerializeField] GameObject[] _toggleOffGameObjects;

    bool _isInWater = false;
    private void Start()
    {
        _camera = Camera.main;
        _surfaceWaterCullingMask = _camera.cullingMask;
        _underWaterEffect.SetColor("_WaterCol", _colour);
    }
    private void Update()
    {
        if (_camera.transform.position.y <= _seaLevel && !_isInWater)
            ToggleWater(true);
        else if (_camera.transform.position.y > _seaLevel && _isInWater)
            ToggleWater(false);
    }

    void ToggleWater(bool status)
    {
        _isInWater = status;

        if (status)
        {
            _camera.cullingMask = _underWaterCullingMask;
            _underWaterEffect.SetFloat("_Distance", 200);
            _cameraEffects.ActivateImageEffect(_underWaterEffect);
        }
        else
        {
            _camera.cullingMask = _surfaceWaterCullingMask;
            _cameraEffects.RemoveImageEffect(_underWaterEffect);
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
