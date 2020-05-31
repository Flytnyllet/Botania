using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Amb_Local_Manager : MonoBehaviour
{
    public static Amb_Local_Manager Instance;

    public float Biome_1 { get { return _biome_1; } }
    private float _biome_1 = default;
    public float Biome_2 { get { return _biome_2; } }
    private float _biome_2 = default;
    public float Biome_3 { get { return _biome_3; } }
    private float _biome_3 = default;
    public float Biome_4 { get { return _biome_4; } }
    private float _biome_4 = default;

    [ParamRef]
    public string biome_1_parameter;
    private PARAMETER_DESCRIPTION biome_1_Description;
    [ParamRef]
    public string biome_2_parameter;
    private PARAMETER_DESCRIPTION biome_2_Description;
    [ParamRef]
    public string biome_3_parameter;
    private PARAMETER_DESCRIPTION biome_3_Description;
    [ParamRef]
    public string biome_4_parameter;
    private PARAMETER_DESCRIPTION biome_4_Description;

    private int _currentBiome;
    private int _lastBiome;

    [SerializeField]
    private GameObject amb_01;
    [SerializeField]
    private GameObject amb_02;
    [SerializeField]
    private GameObject amb_03;
    [SerializeField]
    private GameObject amb_04;
    [SerializeField]
    private GameObject amb_Water;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        _lastBiome = 42;

        RuntimeManager.StudioSystem.getParameterDescriptionByName(biome_1_parameter, out biome_1_Description);
        RuntimeManager.StudioSystem.getParameterDescriptionByName(biome_2_parameter, out biome_2_Description);
        RuntimeManager.StudioSystem.getParameterDescriptionByName(biome_3_parameter, out biome_3_Description);
        RuntimeManager.StudioSystem.getParameterDescriptionByName(biome_4_parameter, out biome_4_Description);

    }

    private void Update()
    {
        if (transform.position.y < 10.7)
        {
            amb_Water.SetActive(true);
            _currentBiome = 5;
            _biome_1 = 0;
            _biome_2 = 0;
            _biome_3 = 0;
            _biome_4 = 1;
            amb_01.SetActive(false);
            amb_02.SetActive(false);
            amb_03.SetActive(false);
            amb_04.SetActive(true);

            RuntimeManager.StudioSystem.setParameterByID(biome_1_Description.id, _biome_1);
            RuntimeManager.StudioSystem.setParameterByID(biome_2_Description.id, _biome_2);
            RuntimeManager.StudioSystem.setParameterByID(biome_3_Description.id, _biome_3);
            RuntimeManager.StudioSystem.setParameterByID(biome_4_Description.id, _biome_4);
        }
        else
        {
            amb_Water.SetActive(false);

            if (_currentBiome != _lastBiome)
            {
                switch (Player.GetCurrentBiome())
                {
                    case BiomeTypes.FOREST:
                        _currentBiome = 1;
                        _biome_1 = 1;
                        _biome_2 = 0;
                        _biome_3 = 0;
                        _biome_4 = 0;
                        amb_01.SetActive(true);
                        amb_02.SetActive(false);
                        amb_03.SetActive(false);
                        amb_04.SetActive(false);

                        Amb_Rain.Instance.Set_Rain_Override(0f);
                        break;
                    case BiomeTypes.BIRCH:
                        _currentBiome = 2;
                        _biome_1 = 0;
                        _biome_2 = 1;
                        _biome_3 = 0;
                        _biome_4 = 0;
                        amb_01.SetActive(false);
                        amb_02.SetActive(true);
                        amb_03.SetActive(false);
                        amb_04.SetActive(false);

                        Amb_Rain.Instance.Set_Rain_Override(0.7f);
                        break;
                    case BiomeTypes.WEIRD:
                        _currentBiome = 3;
                        _biome_1 = 0;
                        _biome_2 = 0;
                        _biome_3 = 1;
                        _biome_4 = 0;
                        amb_01.SetActive(false);
                        amb_02.SetActive(false);
                        amb_03.SetActive(true);
                        amb_04.SetActive(false);

                        Amb_Rain.Instance.Set_Rain_Override(0f);
                        break;
                    case BiomeTypes.PLANES:
                        _currentBiome = 4;
                        _biome_1 = 0;
                        _biome_2 = 0;
                        _biome_3 = 0;
                        _biome_4 = 1;
                        amb_01.SetActive(false);
                        amb_02.SetActive(false);
                        amb_03.SetActive(false);
                        amb_04.SetActive(true);

                        if (Random.Range(0, 1f) > 0.5f)
                        {
                            Music_Manager.Instance.Init_Music(4);
                        }

                        Amb_Rain.Instance.Set_Rain_Override(0f);
                        break;
                }
                RuntimeManager.StudioSystem.setParameterByID(biome_1_Description.id, _biome_1);
                RuntimeManager.StudioSystem.setParameterByID(biome_2_Description.id, _biome_2);
                RuntimeManager.StudioSystem.setParameterByID(biome_3_Description.id, _biome_3);
                RuntimeManager.StudioSystem.setParameterByID(biome_4_Description.id, _biome_4);
                _lastBiome = _currentBiome;
            }
        }
    }
}