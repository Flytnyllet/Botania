using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Local_Wind : MonoBehaviour
{
    private string event_Ref;
    private EventInstance event_Instance;
    private EventDescription event_Description;
    private PLAYBACK_STATE _event_State;
    private bool _is3D;

    [SerializeField]
    private Amb_Data amb_Data = default;
    private Amb_GetRandomEvent amb_RandomEvent;

    private void OnEnable()
    {
        amb_RandomEvent = GetComponentInParent<Amb_GetRandomEvent>();
        Init_Local_Wind();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
    }

    private void Init_Local_Wind()
    {
        switch (amb_RandomEvent.amb_List)
        {
            case "Amb_Forest":
                event_Ref = amb_Data.amb_forest_wind;
                break;
            case "Amb_Grassland":
                event_Ref = amb_Data.amb_forest_wind;
                break;
        }
        event_Instance = RuntimeManager.CreateInstance(event_Ref);
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.is3D(out _is3D);
        if (_is3D)
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());

        Start_Local_Wind();
    }

    public void Start_Local_Wind()
    {
        event_Instance.start();
    }

    public void Stop_Local_Wind()
    {
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void FixedUpdate()
    {
        event_Instance.getPlaybackState(out _event_State);

        if (_event_State != PLAYBACK_STATE.STOPPED)
            return;
        else
        {
            event_Instance.release();
            event_Instance.clearHandle();
            gameObject.SetActive(false);
        }
    }
}