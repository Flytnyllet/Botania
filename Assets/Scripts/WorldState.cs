using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    [SerializeField] float _eventMinTime = 10;
    [SerializeField] float _eventMaxTime = 20;

    [SerializeField] float _fogChangeTime = 4;
    [SerializeField] float _targetFogDensity = 0.5f;
    float _baseFogDensity;

    WorldState _instance;
    public WorldState Instance { get => _instance; }
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
        setRaining(false);
        StartEvent(WORLD_EVENTS.Normal);
        StartCoroutine(changeWindSpeed(1, 1));
    }

    enum WORLD_EVENTS { Normal = 0, Rain, Fog, StrongWind };
    void StartEvent(WORLD_EVENTS worldEvent)
    {
        switch (worldEvent)
        {
            case WORLD_EVENTS.Normal:
                ActionDelayer.RunAfterDelay(() =>
                {
                    //pick random enum
                    StartEvent((WORLD_EVENTS)Random.Range(1, System.Enum.GetValues(typeof(WORLD_EVENTS)).Length));
                }, Random.Range(_eventMinTime, _eventMaxTime));
                break;

            case WORLD_EVENTS.Rain:
                setRaining(true);
                ActionDelayer.RunAfterDelay(() =>
                {
                    setRaining(false);
                    StartEvent(WORLD_EVENTS.Normal);
                }, Random.Range(_eventMinTime, _eventMaxTime));
                break;

            case WORLD_EVENTS.Fog:

                StartCoroutine(ChangeFogDensity(_fogChangeTime, _targetFogDensity));
                _thickFog = true;
                ActionDelayer.RunAfterDelay(() =>
                {
                    StartCoroutine(ChangeFogDensity(_fogChangeTime, _baseFogDensity));
                    _thickFog = false;
                    StartEvent(WORLD_EVENTS.Normal);
                }, Random.Range(_eventMinTime, _eventMaxTime));
                break;

            case WORLD_EVENTS.StrongWind:
                //Debug.Log("A wind is rising");
                StartCoroutine(changeWindSpeed(2, 3));
                ActionDelayer.RunAfterDelay(() =>
                {
                   // Debug.Log("the wind is settling");
                    StartCoroutine(changeWindSpeed(2, 1));
                    StartEvent(WORLD_EVENTS.Normal);
                }, Random.Range(_eventMinTime, _eventMaxTime));
                break;
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
    bool raining = false;
    public bool IsRaining { get => raining; }
    public void setRaining(bool b)
    {
        if (b)
        {
            Shader.SetGlobalFloat("gRainWave", 0.9f);
            raining = true;
            EventManager.TriggerEvent(EventNameLibrary.START_RAIN, new EventParameter());
        }
        else
        {
            Shader.SetGlobalFloat("gRainWave", 1f);
            raining = false;
            EventManager.TriggerEvent(EventNameLibrary.STOP_RAIN, new EventParameter());
        }
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
            RenderSettings.fogDensity = Mathf.Lerp(startVal, targetStrenght, time / lerpTime);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
