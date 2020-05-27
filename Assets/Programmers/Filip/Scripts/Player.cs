using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] Transform _playerParent;
    [SerializeField] float _playerSpawnHeightOffset = 3f;
    [SerializeField, Range(0, 1000)] float _distanceRayCast = 200;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] BiomeInfo _biomeInfoInstance;
    [SerializeField, Range(0.01f, 30)] float _updateBiomeTime = 1.0f; 

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
            _playerTransform = GetComponent<Transform>();
            _updateBiomeTableTimer = new Timer(_updateBiomeTime);
            _biomeInfo = _biomeInfoInstance;
            _spawnPosition = _playerParent.position;

            UpdateBiomeInfo();
        }
        else
            Destroy(this);
    }

    private void Start()
    {
        _thisSingleton.StartCoroutine(PlacePlayer());
    }

    public static void Load()
    {
        object spawnPosition = Serialization.Load(Saving.FileNames.PLAYER_POSITION);

        if (spawnPosition != null)
            _spawnPosition = (Vector3)spawnPosition;
    }

    public static void Save()
    {
        Serialization.Save(Saving.FileNames.PLAYER_POSITION, _playerTransform.position + Vector3.up * 50); //+50 is just to guarantee it doesn't fall throught the ground
    }

    IEnumerator PlacePlayer()
    {
        bool hit = false;
        RaycastHit collision;
        do
        {
            yield return null;
            hit = Physics.Raycast(_thisSingleton._playerParent.transform.position, Vector3.down, out collision, _distanceRayCast, _layerMask.value);
            Debug.DrawRay(_thisSingleton._playerParent.transform.position, Vector3.down * _distanceRayCast, Color.cyan, 1f);

        } while (!hit);

        Debug.LogError(_spawnPosition);
        //PLACERA SPELARE HÄR!
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

