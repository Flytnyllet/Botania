using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFlowerBehaviour : MonoBehaviour
{
    float _startAudioRadius;
    private void Awake()
    {
        //_startAudioRadius = AudioSource.Radius;
    }
    private void OnEnable()
    {
        EventManager.Subscribe("HearingPotion", IncreaseAudioSourceRadius);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe("HearingPotion", IncreaseAudioSourceRadius);
    }
    
    void IncreaseAudioSourceRadius(EventParameter param)
    {
        //AudioSource.Radius = param.intParam;
        ActionDelayer.RunAfterDelay(() =>
        {
            //AudioSource.Radius = _startAudioRadius;
        }, param.floatParam);
    }

}
