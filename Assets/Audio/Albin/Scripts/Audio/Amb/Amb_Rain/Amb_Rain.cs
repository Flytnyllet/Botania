﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Rain : MonoBehaviour
{
    public static Amb_Rain Instance;




    //[SerializeField] [Range(0f, 1f)] private float rainOverrideTempSlider;

    private float _rainOverrideValue = default;

    private bool _setScriptOverride = false;



    [EventRef] public string rain_Event;

    [ParamRef] public string rainIntensity_Parameter;
    private PARAMETER_DESCRIPTION rainIntensityDescription;
    [ParamRef] public string rainOverride_Parameter;
    private PARAMETER_DESCRIPTION rainOverrideDescription;

    [SerializeField] private Amb_Rain_Emitter _rainEmitterL = default;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterR = default;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterB = default;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterF = default;

    public bool IsStopping { get { return _isStopping; } }
    private bool _isStopping = false;
    private bool _raining = false;

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

        RuntimeManager.StudioSystem.getParameterDescriptionByName(rainIntensity_Parameter, out rainIntensityDescription);
        RuntimeManager.StudioSystem.getParameterDescriptionByName(rainOverride_Parameter, out rainOverrideDescription);

        Init_Rain_Event();
        _isStopping = false;
    }

    private void Init_Rain_Event()
    {
        _rainEmitterL.Init_Event(rain_Event);
        _rainEmitterR.Init_Event(rain_Event);
        _rainEmitterB.Init_Event(rain_Event);
        _rainEmitterF.Init_Event(rain_Event);
    }

    public IEnumerator Start_Rain(float rain_intensity)
    {
        if (!_isStopping)
        {
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, 0.3f);
            _rainEmitterL.Start_Rain_Emitter();
            _rainEmitterR.Start_Rain_Emitter();
            _rainEmitterB.Start_Rain_Emitter();
            _rainEmitterF.Start_Rain_Emitter();
            yield return new WaitForSeconds(2f);
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, rain_intensity);
        }
        else { yield break; }
    }

    public IEnumerator Stop_Rain()
    {
        _isStopping = true;
        if (_rainOverrideValue != 0.9f)
        {
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, 0.3f);
            yield return new WaitForSeconds(5f);
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, 0f);
            yield return new WaitForSeconds(4f);
            _setScriptOverride = true;
            RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0.55f);
            yield return new WaitForSeconds(7f);
            _rainEmitterL.Stop_Rain_Emitter();
            _rainEmitterR.Stop_Rain_Emitter();
            _rainEmitterB.Stop_Rain_Emitter();
            _rainEmitterF.Stop_Rain_Emitter();
            yield return new WaitForSeconds(11f);
            _setScriptOverride = false;
            _isStopping = false;
        }
        else
        {
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, 0.3f);
            yield return new WaitForSeconds(5f);
            RuntimeManager.StudioSystem.setParameterByID(rainIntensityDescription.id, 0f);
            yield return new WaitForSeconds(4f);
            _setScriptOverride = true;
            RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0.75f);
            yield return new WaitForSeconds(10f);
            _rainEmitterL.Stop_Rain_Emitter();
            _rainEmitterR.Stop_Rain_Emitter();
            _rainEmitterB.Stop_Rain_Emitter();
            _rainEmitterF.Stop_Rain_Emitter();
            yield return new WaitForSeconds(11f);
            _setScriptOverride = false;
            _isStopping = false;
        }
    }

    public void Set_Rain_Override(float overrideValue)
    {
        _setScriptOverride = true;
        RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, overrideValue);
        _rainOverrideValue = overrideValue;
    }

    public void Stop_Rain_Override()
    {
        _setScriptOverride = false;
    }

    private void Update()
    {
        if (WorldState.Instance.IsRaining && !_raining)
        {
            StartCoroutine(Start_Rain(WorldState.Instance.RainStrenght));
            _raining = true;
        }

        if (!WorldState.Instance.IsRaining && _raining)
        {
            StartCoroutine(Stop_Rain());
            _raining = false;
        }

        if (!_setScriptOverride)
        {
            switch (Player.GetCurrentBiome())
            {
                case BiomeTypes.FOREST:
                    RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0);
                    break;
                case BiomeTypes.BIRCH:
                    RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0.7f);
                    break;
                case BiomeTypes.WEIRD:
                    RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0);
                    break;
                case BiomeTypes.PLANES:
                    RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, 0);
                    break;
            }
        }
    }
}
