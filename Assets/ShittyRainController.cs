using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShittyRainController : MonoBehaviour
{
    ParticleSystem _particleSys;

    private void Awake()
    {
        _particleSys = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.START_RAIN, StartRain);
        EventManager.Subscribe(EventNameLibrary.STOP_RAIN, StartRain);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.START_RAIN, StartRain);
        EventManager.UnSubscribe(EventNameLibrary.STOP_RAIN, StartRain);
    }
    void StartRain(EventParameter param)
    {
        var module = _particleSys.emission;
        module.rateOverTime = param.intParam;
        _particleSys.Play();
    }
    void StopRain(EventParameter param)
    {
        _particleSys.Stop();
    }
}
