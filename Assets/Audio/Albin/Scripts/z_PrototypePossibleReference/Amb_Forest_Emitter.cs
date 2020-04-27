using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Forest_Emitter : MonoBehaviour
{
    private Amb_GetRandomEvent amb_GetRandomEvent;
    private EventInstance event_Instance;
    private PLAYBACK_STATE event_State;
    private int state = default;
    private int lastState = default;
    private EventDescription event_Description;
    private float maxDistance;
    private bool is3D;
    private bool hasPlayed = default;

    private SphereCollider event_Collider;

    [SerializeField]
    private bool debug = default;


    private void Awake()
    {
        amb_GetRandomEvent = GetComponent<Amb_GetRandomEvent>();
    }

    private void Start()
    {
        Init_Event();
    }

    private void Init_Event()
    {
        event_Instance = RuntimeManager.CreateInstance(amb_GetRandomEvent.Amb_RandomEvent);
        event_Description = RuntimeManager.GetEventDescription(amb_GetRandomEvent.Amb_RandomEvent);
        event_Description.getMaximumDistance(out maxDistance);
        event_Description.is3D(out is3D);
        if (is3D)
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());
        event_Instance.getPlaybackState(out event_State);
        hasPlayed = false;
        SetCollider();
    }

    private void SetCollider()
    {
        event_Collider = gameObject.AddComponent<SphereCollider>();
        event_Collider.isTrigger = true;
        event_Collider.center = Vector3.zero;
        event_Collider.radius = maxDistance * 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        if (hasPlayed)
            Init_Event();
        event_Instance.start();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.getPlaybackState(out event_State);
        CheckPlaybackState();
    }

    private void CheckPlaybackState()
    {
        switch (event_State)
        {
            case PLAYBACK_STATE.PLAYING:
                state = 0;
                break;
            case PLAYBACK_STATE.SUSTAINING:
                state = 1;
                break;
            case PLAYBACK_STATE.STOPPED:
                state = 2;
                break;
            case PLAYBACK_STATE.STARTING:
                state = 3;
                break;
            case PLAYBACK_STATE.STOPPING:
                state = 4;
                break;
        }
        if (debug)
        {
            if (state != lastState)
            {
                Debug.Log("event_Instance on object: " + transform.parent.gameObject.name + " is in state: " + event_State);
                lastState = state;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }

        if (event_State == PLAYBACK_STATE.PLAYING)
        {
            Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.release();
        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        event_Instance.getPlaybackState(out event_State);
        event_Instance.clearHandle();
        hasPlayed = true;
    }
}
