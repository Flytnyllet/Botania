﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static bool ReadyToSpawnWorld { get; private set; } = false;
    public static bool PlayerSpawned { get; private set; } = false;

    [SerializeField] float _seaLevelSpawn = 10;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] Transform _playerParent;
    [SerializeField] float _playerSpawnHeightOffset = 3f;
    [SerializeField, Range(0, 1000)] float _distanceRayCast = 200;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] BiomeInfo _biomeInfoInstance;
    [SerializeField, Range(0.01f, 30)] float _updateBiomeTime = 1.0f;

    [SerializeField] FPSMovement _fpsScript;

    MouseLook _mouseLook;
    Camera _playerCamera;

    //Singleton
    static Player _thisSingleton;

    static Transform _playerTransform;
    static BiomeInfo _biomeInfo;
    static Vector3 _spawnPosition;

    Timer _updateBiomeTableTimer;


    private void Awake()
    {
        if (_thisSingleton == null)
        {
            _thisSingleton = this;
            ReadyToSpawnWorld = false;
            PlayerSpawned = false;
            _playerTransform = GetComponent<Transform>();
            _mouseLook = GetComponent<MouseLook>();
            _playerCamera = GetComponent<Camera>();
            _updateBiomeTableTimer = new Timer(_updateBiomeTime);
            _biomeInfo = _biomeInfoInstance;
            _spawnPosition = _playerParent.position;

            UpdateBiomeInfo();
        }
        else
            Destroy(this);
    }

    public static void Load()
    {
        object spawnPosition = Serialization.Load(Saving.FileNames.PLAYER_POSITION);

        if (spawnPosition != null && (Vector3)spawnPosition != new Vector3(-1000, -1000, -1000))
            _spawnPosition = (Vector3)spawnPosition;

        _thisSingleton.StartCoroutine(_thisSingleton.PlacePlayer());
    }

    public static void Save()
    {
        Vector3 savePosition = _playerTransform.position;
        savePosition.y = 0;
        Serialization.Save(Saving.FileNames.PLAYER_POSITION, savePosition + Vector3.up * 75); //+75 is just to guarantee it doesn't fall throught the ground
    }

    public static void Wipe()
    {
        Serialization.Save(Saving.FileNames.PLAYER_POSITION, new Vector3(-1000, -1000, -1000)); //This is to mark it as null
    }

    IEnumerator PlacePlayer()
    {
        //This makes sure Start runs on FPSScript
        yield return null;
        _fpsScript.Teleport(_spawnPosition);
        ReadyToSpawnWorld = true;

        bool hit = false;
        RaycastHit collision;
        do
        {
            yield return null;
            hit = Physics.Raycast(_spawnPosition, Vector3.down, out collision, _distanceRayCast, _layerMask.value);
            Debug.DrawRay(_spawnPosition, Vector3.down * _distanceRayCast, Color.cyan, 1f);
        } while (!hit);

        PlayerSpawned = true;

        Vector3 spawnPosition = collision.point;

        if (collision.point.y < _seaLevelSpawn)
            spawnPosition.y = _seaLevelSpawn;

        _fpsScript.Teleport(spawnPosition);

        OnStartFadeIn.FadeIn();
    }

    void Update()
    {
        _updateBiomeTableTimer.Time += Time.deltaTime;

        if (_updateBiomeTableTimer.Expired())
        {
            UpdateBiomeInfo();
            _updateBiomeTableTimer.Reset();
        }
    }

    void UpdateBiomeInfo()
    {
        float currentX = _playerTransform.position.x / _meshSettings.MeshScale;
        float currentY = _playerTransform.position.z / _meshSettings.MeshScale;

        _biomeInfo.Update(new Vector2(currentX, currentY));
    }

    public static Transform GetPlayerTransform()
    {
        return _playerTransform;
    }

    public static void SetSensitivity(float value)
    {
        _thisSingleton._mouseLook.Sensitivity = value;
    }

    public static float GetSensitivity()
    {
        return _thisSingleton._mouseLook.Sensitivity;
    }

    public static Camera GetPlayerCamera()
    {
        return _thisSingleton._playerCamera;
    }

    public static BiomeTypes GetCurrentBiome()
    {
        return _biomeInfo.GetCurrentBiome();
    }
}

public enum BiomeTypes
{
    FOREST,
    BIRCH,
    WEIRD,
    PLANES
}

[System.Serializable]
public struct BiomeInfo
{
    [SerializeField] List<BiomeInfoInstance> _biomeTypes;

    public BiomeTypes GetCurrentBiome()
    {
        for (int i = 0; i < _biomeTypes.Count; i++)
        {
            if (_biomeTypes[i].GetValue() > 0)
                return _biomeTypes[i].GetBiomeType();
        }

        throw new System.Exception("There is no biome here!? Should not be possible!?");
    }

    public void Update(Vector2 samplePoint)
    {
        float totalSum = 0.0f;

        for (int i = 0; i < _biomeTypes.Count; i++)
        {
            totalSum += _biomeTypes[i].GetAndSetPointInNoise(samplePoint);
        }

        for (int i = 0; i < _biomeTypes.Count; i++)
        {
            _biomeTypes[i].NormalizeValue(totalSum);
        }
    }
}

[System.Serializable]
public class BiomeInfoInstance
{
    [SerializeField] NoiseSettingsData _noiseSettings;
    [SerializeField, Range(0, 1)] float _noiseStartPoint = 0.0f;

    [SerializeField] BiomeTypes _biomeType;
    float _value;

    public float GetValue() { return _value; }
    public BiomeTypes GetBiomeType() { return _biomeType; }

    public float GetAndSetPointInNoise(Vector2 samplePoint)
    {
        float[,] noise = Noise.GenerateNoiseMap(1, 1, 1, _noiseSettings.NoiseSettingsDataMerge, samplePoint);
        noise = Noise.Clamp(noise, _noiseSettings);
        _value = noise[0, 0] > _noiseStartPoint ? noise[0, 0] : 0;

        return _value;
    }

    public void NormalizeValue(float max)
    {
        if (max != 0)
            _value /= max;
    }
}

