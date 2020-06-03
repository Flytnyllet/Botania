using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Underwater : MonoBehaviour
{
    [EventRef]
    public string underwater_Event;
    private EventDescription underwater_Description;
    private EventInstance underwater_Instance;

    private void Awake()
    {
        underwater_Description = RuntimeManager.GetEventDescription(underwater_Event);
    }

    private void OnEnable()
    {
        underwater_Description.createInstance(out underwater_Instance);
        underwater_Instance.start();
    }

    private void OnDisable()
    {
        underwater_Instance.release();
        underwater_Instance.triggerCue();
    }
}
