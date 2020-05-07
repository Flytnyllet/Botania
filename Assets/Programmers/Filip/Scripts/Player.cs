using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] BiomeInfo _biomeInfoInstance;
    [SerializeField, Range(0.01f, 30)] float _updateBiomeTime = 1.0f; 

    //Singleton
    static Player _thisSingleton;

    static Transform _playerTransform;
    static BiomeInfo _biomeInfo;

    Timer _updateBiomeTableTimer;

    private void Awake()
    {
        if (_thisSingleton == null)
        {
            _thisSingleton = this;
            _playerTransform = GetComponent<Transform>();
            _updateBiomeTableTimer = new Timer(_updateBiomeTime);
            _biomeInfo = _biomeInfoInstance;
        }
        else
            Destroy(this);
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

    public static List<BiomeInfoInstance> GetBiomeInfo()
    {
        return _biomeInfo.GetValues();
    }
}

public enum BiomeTypes
{
    FOREST
}

[System.Serializable]
public struct BiomeInfo
{
    [SerializeField] List<BiomeInfoInstance> _biomeTypes;

    public List<BiomeInfoInstance> GetValues()
    {
        return new List<BiomeInfoInstance>(_biomeTypes);
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

    public BiomeTypes GetBiomeType() { return _biomeType; }
    public float GetValue() { return _value; }

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

