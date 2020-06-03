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
    [SerializeField] Color _particleWaterColour;
    [SerializeField] ParticleSystem _surfaceParticles;
    [SerializeField] ParticleSystem _waterParticles;
    ParticleSystem.MinMaxGradient _surfaceCol;
    ParticleSystem.MinMaxGradient _waterCol;

    [Header("Drop")]

    [SerializeField] GameObject[] _toggleOnGameObjects;

    bool _isInWater = false;
    private void Start()
    {
        _camera = Camera.main;
        _surfaceWaterCullingMask = _camera.cullingMask;
        _underWaterEffect.SetColor("_WaterCol", _colour);
        _surfaceCol = new ParticleSystem.MinMaxGradient(_surfaceParticles.main.startColor.colorMin, _surfaceParticles.main.startColor.colorMin);
        _waterCol = new ParticleSystem.MinMaxGradient(_waterParticles.main.startColor.colorMin, _waterParticles.main.startColor.colorMin);

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
            var module = _surfaceParticles.main;
            module.startColor = _waterCol;
            _camera.cullingMask = _underWaterCullingMask;
            _underWaterEffect.SetFloat("_Distance", 200);
            _cameraEffects.ActivateImageEffect(_underWaterEffect);
        }
        else
        {
            var module = _surfaceParticles.main;
            module.startColor = _surfaceCol;
            _camera.cullingMask = _surfaceWaterCullingMask;
            _cameraEffects.RemoveImageEffect(_underWaterEffect);
        }

        for (int i = 0; i < _toggleOnGameObjects.Length; i++)
        {
            _toggleOnGameObjects[i].SetActive(status);
        }
    }
}
