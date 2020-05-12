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

    private int _parameterCount;
    public bool IsShy { get { return _isShy; } }
    private bool _isShy = false;
    public PARAMETER_ID IsShyParameterId { get { return _isShyParameterId; } }
    private PARAMETER_ID _isShyParameterId;

    public void Init_Event(string event_Ref)
    {
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.loadSampleData();
        event_Description.createInstance(out event_Instance);
        event_Description.getMaximumDistance(out _maxDistance);
        event_Description.is3D(out _is3D);
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

    public void Attach_Local_Emitter(Transform transform, Rigidbody rigidbody)
    {
        if (!_is3D) { return; }
        RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, rigidbody);
        event_Instance.start();
    }

    public void Set_Parameter(PARAMETER_ID parameterID, float value)
    {
        event_Instance.setParameterByID(parameterID, value);
    }

    public void Set_Parameter_Name(string name, float value)
    {
        event_Instance.setParameterByName(name, value);
    }

    public void Start_Local_Emitter()
    {
        
        _isPlaying = true;
    }

    public void Stop_Local_Emitter()
    {
        event_Description.releaseAllInstances();
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.DetachInstanceFromGameObject(event_Instance);
        event_Instance.clearHandle();
        _isPlaying = false;
    }
}
