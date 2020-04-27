using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Mus_Blom_07_Emitter : MonoBehaviour
{
    [EventRef]
    public string event_Ref;
    private EventInstance event_Instance;
    private EventDescription event_Description;
    private bool _is3D;
    [HideInInspector]
    public float _maxDistance;

    [HideInInspector]
    public PARAMETER_ID _isFollowParameterId;
    [HideInInspector]
    public PARAMETER_ID _blom07DistanceParameterId;
    [HideInInspector]
    public PARAMETER_ID _blom07MovParameterId;
    [HideInInspector]
    public PARAMETER_ID _blom07PotionParameterId;
    [HideInInspector]
    public PARAMETER_ID _blom07CalmParameterId;

    private PLAYBACK_STATE _event_State;
    private int _state;
    private int _lastState;

    [SerializeField]
    private bool debug;

    private void OnEnable()
    {
        event_Instance = RuntimeManager.CreateInstance(event_Ref);
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.getMaximumDistance(out _maxDistance);
        event_Description.is3D(out _is3D);
        if (_is3D)
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());

        Init_Parameter();
        Init_Blom_07();
    }

    private void Init_Parameter()
    {
        EventDescription isFollowEventDescription;
        event_Instance.getDescription(out isFollowEventDescription);
        PARAMETER_DESCRIPTION isFollowParameterDescription;
        isFollowEventDescription.getParameterDescriptionByName("is_follow", out isFollowParameterDescription);
        _isFollowParameterId = isFollowParameterDescription.id;

        EventDescription blom07DistanceEventDescription;
        event_Instance.getDescription(out blom07DistanceEventDescription);
        PARAMETER_DESCRIPTION blom07DistanceParameterDescription;
        blom07DistanceEventDescription.getParameterDescriptionByName("blom_07_distance", out blom07DistanceParameterDescription);
        _blom07DistanceParameterId = blom07DistanceParameterDescription.id;

        EventDescription blom07MovEventDescription;
        event_Instance.getDescription(out blom07MovEventDescription);
        PARAMETER_DESCRIPTION blom07MovParameterDescription;
        blom07MovEventDescription.getParameterDescriptionByName("blom_07_mov", out blom07MovParameterDescription);
        _blom07MovParameterId = blom07MovParameterDescription.id;

        EventDescription blom07PotionEventDescription;
        event_Instance.getDescription(out blom07PotionEventDescription);
        PARAMETER_DESCRIPTION blom07PotionParameterDescription;
        blom07PotionEventDescription.getParameterDescriptionByName("blom_07_potion", out blom07PotionParameterDescription);
        _blom07PotionParameterId = blom07PotionParameterDescription.id;

        EventDescription blom07CalmEventDescription;
        event_Instance.getDescription(out blom07CalmEventDescription);
        PARAMETER_DESCRIPTION blom07CalmParameterDescription;
        blom07CalmEventDescription.getParameterDescriptionByName("blom_07_calm", out blom07CalmParameterDescription);
        _blom07CalmParameterId = blom07CalmParameterDescription.id;

    }

    private void Init_Blom_07()
    {
        event_Instance.start();
        event_Instance.release();
    }

    private void FixedUpdate()
    {
        event_Instance.getPlaybackState(out _event_State);
        CheckPlaybackState();
    }

    public void Set_Parameter(PARAMETER_ID id, float value)
    {
        event_Instance.setParameterByID(id, value);
    }

    public void Override_Max_Distance(float overrideMaxDistance)
    {
        event_Instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, overrideMaxDistance);
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _maxDistance);
        }

        if (_event_State == PLAYBACK_STATE.PLAYING)
        {
            Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
        }
    }

    private void CheckPlaybackState()
    {
        switch (_event_State)
        {
            case PLAYBACK_STATE.PLAYING:
                _state = 0;
                break;
            case PLAYBACK_STATE.SUSTAINING:
                _state = 1;
                break;
            case PLAYBACK_STATE.STOPPED:
                _state = 2;
                break;
            case PLAYBACK_STATE.STARTING:
                _state = 3;
                break;
            case PLAYBACK_STATE.STOPPING:
                _state = 4;
                break;
        }
    }

    public void Stop_Blom_07()
    {
        event_Instance.release();
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
