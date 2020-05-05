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
        SetWindSpeed(1);
        StartEvent(0);
    }

    enum WORLD_EVENTS { Normal, Rain, Fog };
    void StartEvent(int i)
    {
        try
        {
            StartEvent((WORLD_EVENTS)i);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    void StartEvent(WORLD_EVENTS worldEvent)
    {
        switch (worldEvent)
        {
            case WORLD_EVENTS.Normal:
                ActionDelayer.RunAfterDelay(() =>
                {
                    StartEvent(Random.Range(1, System.Enum.GetNames(typeof(WORLD_EVENTS)).Length));
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
                ActionDelayer.RunAfterDelay(() =>
                {
                    StartCoroutine(ChangeFogDensity(_fogChangeTime, _baseFogDensity));
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
    public void SetWindSpeed(float time)
    {
        StartCoroutine(changeWindSpeed(time, 2));
    }
    public void SetWindSpeed(float time, float targetSpeed)
    {
        StartCoroutine(changeWindSpeed(time, targetSpeed));
    }
    IEnumerator changeWindSpeed(float lerpTime, float targetSpeed)
    {
        float time = 0;
        float startVal = _windSpeed;
        while (time < lerpTime)
        {
            Mathf.Lerp(startVal, targetSpeed, time / lerpTime);
            yield return null;
        }
        Shader.SetGlobalFloat("gWindSpeed", 0.95f);
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
            Shader.SetGlobalFloat("gRainWave", 0.95f);
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
