using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Local_Emitter : MonoBehaviour
{
    [EventRef]
    public string event_Ref;
    private EventInstance event_Instance;
    private EventDescription event_Description;
    public float MaxDistance { get { return _maxDistance; } }
    private float _maxDistance;
    private bool _is3D;
    private bool _isVirtual;
    public bool IsPlaying { get { return _isPlaying; } }
    private bool _isPlaying = false;
    private PLAYBACK_STATE _playbackState;

    private int _parameterCount;
    public bool IsShy { get { return _isShy; } }
    private bool _isShy = false;
    private PARAMETER_ID _isShyParameterId;

    public void Init_Event()
    {
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.loadSampleData();
        event_Description.getMaximumDistance(out _maxDistance);
        event_Description.is3D(out _is3D);
    }

    public void Attach_Local_Emitter(Transform transform, Rigidbody rigidbody)
    {
        if (!_is3D) { return; }

        event_Description.createInstance(out event_Instance);
        RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, rigidbody);
        event_Instance.start();
        _isPlaying = true;

        event_Description.getParameterDescriptionCount(out int _parameterCount);
        if (_parameterCount > 0)
        {
            EventDescription isShyEventDescription;
            event_Instance.getDescription(out isShyEventDescription);
            PARAMETER_DESCRIPTION isShyParameterDescription;
            isShyEventDescription.getParameterDescriptionByName("is_shy", out isShyParameterDescription);
            _isShyParameterId = isShyParameterDescription.id;
            _isShy = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isShy == true && other.CompareTag("Player"))
        {
            Set_Parameter(_isShyParameterId, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isShy == true && other.CompareTag("Player"))
        {
            StartCoroutine(WaitBeforeNotShy(Random.Range(1, 5)));
        }
    }

    IEnumerator WaitBeforeNotShy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Set_Parameter(_isShyParameterId, 0);
    }

    public void Set_Parameter(PARAMETER_ID parameterID, float value)
    {
        event_Instance.setParameterByID(parameterID, value);
    }

    private void Update()
    {
        if (_isPlaying)
        {
            event_Instance.isVirtual(out _isVirtual);
            event_Instance.getPlaybackState(out _playbackState);

            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else { _isPlaying = false; }
        }
    }

    public void Stop_Local_Emitter()
    {
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}