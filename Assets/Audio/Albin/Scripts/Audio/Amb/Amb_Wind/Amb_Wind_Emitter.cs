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
    public bool IsPlaying { get { return _isPlaying; } }
    private bool _isPlaying;
    private bool _isVirtual;


    private void Awake()
    {
    }

    public void Init_Event(string event_Ref)
    {
        if (event_Ref == null) { return; }
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.getMaximumDistance(out _maxDistance);
        Attach_Wind_Emitter();
    }

    public void Attach_Wind_Emitter()
    {
        event_Description.createInstance(out event_Instance);
        RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());
        event_Instance.start();
        event_Instance.release();
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            Set_Parameter(Amb_Local_Manager.Instance.Biome_1);
            Set_Parameter(Amb_Local_Manager.Instance.Biome_2);
            Set_Parameter(Amb_Local_Manager.Instance.Biome_3);
            Set_Parameter(Amb_Local_Manager.Instance.Biome_4);

            event_Instance.isVirtual(out _isVirtual);
            event_Instance.getPlaybackState(out _playbackState);

            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else { _isPlaying = false; }
        }
    }

    public void Stop_Wind_Emitter()
    {
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
