using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Rain : MonoBehaviour
{
    public static Amb_Rain Instance;




    [SerializeField] [Range(0f, 1f)] private float rainOverrideTempSlider;





    [EventRef] public string rain_Event;

    [ParamRef] public string rainIntensity_Parameter;
    private PARAMETER_DESCRIPTION rainIntensityDescription;
    [ParamRef] public string rainOverride_Parameter;
    private PARAMETER_DESCRIPTION rainOverrideDescription;

    [SerializeField] private Amb_Rain_Emitter _rainEmitterL;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterR;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterB;
    [SerializeField] private Amb_Rain_Emitter _rainEmitterF;

    private bool _setScriptOverride = false;
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
        if (rainOverrideTempSlider != 1)
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
            _rainEmitterF.Start_Rain_Emitter();
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
            _rainEmitterF.Start_Rain_Emitter();
            yield return new WaitForSeconds(11f);
            _setScriptOverride = false;
            _isStopping = false;
        }
    }

    private void Update()
    {
        if (!_setScriptOverride)
        {
            RuntimeManager.StudioSystem.setParameterByID(rainOverrideDescription.id, rainOverrideTempSlider);
        }

        if (WorldState.Instance.IsRaining && !_raining)
        {
            StartCoroutine(Start_Rain(1f));
            _raining = true;
        }

        if (!WorldState.Instance.IsRaining && _raining)
        {
            StartCoroutine(Stop_Rain());
            _raining = false;
        }
    }
}
