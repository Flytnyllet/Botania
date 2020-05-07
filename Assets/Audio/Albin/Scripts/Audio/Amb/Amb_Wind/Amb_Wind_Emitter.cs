using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Wind_Emitter : MonoBehaviour
{
    private EventInstance event_Instance;
    private EventDescription event_Description;
    public float MaxDistance { get { return _maxDistance; } }
    private float _maxDistance;
    private PLAYBACK_STATE _playbackState;
    private int _timelinePosition;
    private bool _isPlaying;
    private bool _isVirtual;
    private PARAMETER_DESCRIPTION windIntensityDescription;


    private void Awake()
    {
        RuntimeManager.StudioSystem.getParameterDescriptionByName("wind_intensity", out windIntensityDescription);
    }

    public void Init_Event(string event_Ref)
    {
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.getMaximumDistance(out _maxDistance);
    }

    public void Attach_Wind_Emitter(Transform transform, Rigidbody rigidbody)
    {
        event_Description.createInstance(out event_Instance);
        RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, rigidbody);
        event_Instance.start();
        event_Instance.release();
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            event_Instance.isVirtual(out _isVirtual);
            event_Instance.getPlaybackState(out _playbackState);
        }
    }

    public void Set_Parameter(float wind_IntensityValue)
    {
        RuntimeManager.StudioSystem.setParameterByID(windIntensityDescription.id, wind_IntensityValue);
    }

    public void Stop_Wind_Emitter()
    {
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
        else { _isPlaying = false; Debug.Log("Wind stopped."); }
    }
}
