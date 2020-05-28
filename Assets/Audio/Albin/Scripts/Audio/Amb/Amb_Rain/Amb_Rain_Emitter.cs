using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Rain_Emitter : MonoBehaviour
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

    public void Init_Event(string event_Ref)
    {
        if (event_Ref == null) { return; }
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.getMaximumDistance(out _maxDistance);
        Attach_Rain_Emitter();
    }

    public void Attach_Rain_Emitter()
    {
        event_Description.createInstance(out event_Instance);
        
    }

    public void Start_Rain_Emitter()
    {
        _isPlaying = true;
        event_Instance.start();
    }

    private void Update()
    {
        if (_isPlaying)
        {
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());
            event_Instance.isVirtual(out _isVirtual);
            event_Instance.getPlaybackState(out _playbackState);

            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else { _isPlaying = false; }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
    }

    public void Stop_Rain_Emitter()
    {
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}