using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WorldState : MonoBehaviour
{
    [SerializeField] float _eventMinTime = 10;
    [SerializeField] float _eventMaxTime = 20;

    [SerializeField] float _fogChangeTime = 4;
    [SerializeField] float _targetFogDensity = 0.5f;
    float _currentFogDensity;
    float _baseFogDensity;


    [SerializeField] int _lightningStrikesPerRain = 2;

    static WorldState _instance;
    public static WorldState Instance { get => _instance; }

    [SerializeField] private FMODUnity.StudioEventEmitter _amb_Thunder;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else { Destroy(this.gameObject); }

        _baseFogDensity = RenderSettings.fogDensity;
    }
    private void Start()
    {
        _currentFogDensity = RenderSettings.fogDensity;
        _cloudthicknessLowStepSTARTVALUE = _cloudthicknessLowStep;
        setRaining(false);
        StartEvent(WORLD_EVENTS.Normal);
        StartCoroutine(changeWindSpeed(1, 1));
        StartCoroutine(ChangeCloudThickness(1, _cloudthicknessLowStep));
    }

    enum WORLD_EVENTS { Normal = 0, Rain/*, Fog, StrongWind*/ };
    void StartEvent(WORLD_EVENTS worldEvent)
    {
        switch (worldEvent)
        {
            case WORLD_EVENTS.Normal:
                var test = ActionDelayer.RunAfterDelay(() =>
                {
                    //pick random enum
                    StartEvent((WORLD_EVENTS)Random.Range(1, System.Enum.GetValues(typeof(WORLD_EVENTS)).Length));
                }, Random.Range(_eventMinTime, _eventMaxTime));
                break;

            case WORLD_EVENTS.Rain:
                //This  IF / ELSE part is awfull, look away
                float eventTime = Random.Range(_eventMinTime, _eventMaxTime);
                if (Random.Range(0, 1f) < 0.5f)
                {
                    setRaining(true, 2000, 0.3f);
                    EventParameter param = new EventParameter() { floatParam = 0.75f, floatParam2 = 4f };
                    ActionDelayer.RunAfterDelay(() =>
                    {
                        ActionDelayer.RunAfterDelay(() =>
                        {
                            if (IsRaining)
                            {
                                _amb_Thunder.Play();
                            }
                        }, Random.Range(0.15f, 1f));
                        EventManager.TriggerEvent(EventNameLibrary.LIGHTNING_STRIKE, param);

                    }, eventTime * 0.25f);

                }
                else
                {
                    setRaining(true, 10000, 1);
                    float lightningTimingBaseOffset = (eventTime * 0.5f - 5) / _lightningStrikesPerRain;
                    float lightningTiming = 0;
                    for (int i = 0; i < _lightningStrikesPerRain - 1; i++)
                    {
                        lightningTiming += lightningTimingBaseOffset + Random.Range(-1, 1);
                        EventParameter param = new EventParameter() { floatParam = 0.75f, floatParam2 = 4f };
                        ActionDelayer.RunAfterDelay(() =>
                        {
                            ActionDelayer.RunAfterDelay(() =>
                            {
                                if (IsRaining)
                                {
                                    _amb_Thunder.Play();
                                }
                            }, Random.Range(0.15f, 1f));
                            EventManager.TriggerEvent(EventNameLibrary.LIGHTNING_STRIKE, param);

                        }, lightningTiming);
                    }
                }
                ActionDelayer.RunAfterDelay(() =>
                {
                    _rainStrenght = 0f;
                    setRaining(false);
                    StartEvent(WORLD_EVENTS.Normal);
                }, eventTime * 0.5f);
                break;

                //case WORLD_EVENTS.Fog:
                //    StartCoroutine(ChangeFogDensity(_fogChangeTime, _targetFogDensity));
                //    _thickFog = true;
                //    ActionDelayer.RunAfterDelay(() =>
                //    {
                //        StartCoroutine(ChangeFogDensity(_fogChangeTime, _baseFogDensity));
                //        _thickFog = false;
                //        StartEvent(WORLD_EVENTS.Normal);
                //    }, Random.Range(_eventMinTime, _eventMaxTime));
                //    break;

                //case WORLD_EVENTS.StrongWind:
                //    //Debug.Log("A wind is rising");
                //    StartCoroutine(changeWindSpeed(2, 3));
                //    ActionDelayer.RunAfterDelay(() =>
                //    {
                //        // Debug.Log("the wind is settling");
                //        StartCoroutine(changeWindSpeed(2, 1));
                //        StartEvent(WORLD_EVENTS.Normal);
                //    }, Random.Range(_eventMinTime, _eventMaxTime));
                //    break;
        }
    }



    //
    //Wind
    //
    static float _windSpeed = 1;
    public static float WindSpeed
    {
        get => _windSpeed;
    }
    IEnumerator changeWindSpeed(float lerpTime, float targetSpeed)
    {
        float time = 0;
        float startVal = _windSpeed;
        while (time < lerpTime)
        {
            _windSpeed = Mathf.Lerp(startVal, targetSpeed, time / lerpTime);
            Shader.SetGlobalFloat("gWindSpeed", _windSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        _windSpeed = targetSpeed;
        Shader.SetGlobalFloat("gWindSpeed", targetSpeed);
    }

    //
    //Rain
    //
    bool _raining = false;
    float _rainStrenght = 0.0f;
    float _cloudthicknessLowStep = 0.6f;
    float _cloudthicknessLowStepSTARTVALUE;
    public bool IsRaining { get => _raining; }
    public float RainStrenght { get => _rainStrenght; }
    public void setRaining(bool shouldRain, int rainAmount = 10000, float rainSoundLevel = 0)
    {
        if (shouldRain)
        {
            Shader.SetGlobalFloat("gRainWave", 0.9f);
            StartCoroutine(ChangeCloudThickness(20, -0.8f));
            ActionDelayer.RunAfterDelay(() =>
            {
                _rainStrenght = rainSoundLevel;
                _raining = true;
                if (_raining)
                    EventManager.TriggerEvent(EventNameLibrary.START_RAIN, new EventParameter() { intParam = rainAmount });
            }, 10);

        }
        else
        {
            Shader.SetGlobalFloat("gRainWave", 1f);
            StartCoroutine(ChangeCloudThickness(10, _cloudthicknessLowStepSTARTVALUE));
            _raining = false;
            EventManager.TriggerEvent(EventNameLibrary.STOP_RAIN, new EventParameter());
        }
    }
    //Cloud
    IEnumerator ChangeCloudThickness(float lerpTime, float lowSmoothstepTarget)
    {
        float time = 0;
        float startVal = _cloudthicknessLowStep;
        while (time < lerpTime)
        {
            time += Time.deltaTime;
            _cloudthicknessLowStep = Mathf.Lerp(startVal, lowSmoothstepTarget, time / lerpTime);
            Shader.SetGlobalFloat("gCloudLowStep", _cloudthicknessLowStep);
            yield return null;
        }
        Shader.SetGlobalFloat("gCloudLowStep", _cloudthicknessLowStep);
    }


    //
    //Fog
    //
    bool _thickFog = false;
    public bool ThickFog { get => _thickFog; }
    IEnumerator ChangeFogDensity(float lerpTime, float targetStrenght)
    {
        float time = 0;
        float startVal = RenderSettings.fogDensity;
        while (time < lerpTime)
        {
            _currentFogDensity = Mathf.Lerp(startVal, targetStrenght, time / lerpTime);
            if (worldStateDominant)
            {
                RenderSettings.fogDensity = _currentFogDensity;
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    //Absolute fucking trash, like holy fuck don't even look at it
    //Lerps between 0 and current FogThickness, depending on wether something wants to override the actual value
    //This is all because Unity doesn't use the fucking alpha colour on the fog colour
    //Forget to change the worldState back and I'll stab you, then stab myself for creating this garbage.
    bool worldStateDominant = true;
    public void ChangeFogThickness(float lerpTime)
    {
        worldStateDominant = !worldStateDominant;
        StartCoroutine(ChangeFogDensityChild(lerpTime));
    }
    IEnumerator ChangeFogDensityChild(float lerpTime)
    {
        float time = 0;
        float startVal = RenderSettings.fogDensity;
        while (time < lerpTime)
        {
            RenderSettings.fogDensity = Mathf.Lerp(startVal,
                worldStateDominant ? _currentFogDensity : 0, time / lerpTime);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
