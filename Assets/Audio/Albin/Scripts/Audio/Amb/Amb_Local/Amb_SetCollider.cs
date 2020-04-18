﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_SetCollider : MonoBehaviour
{
    private EventInstance event_Instance;
    private EventDescription event_Description;
    private float _maxDistance;
    private bool _is3D;
    private bool _hasPlayed = default;

    [SerializeField]
    private bool debug;

    private PLAYBACK_STATE _event_State;
    private int _state = default;
    private int _lastState = default;


    private SphereCollider event_Collider;
    private Amb_GetRandomEvent amb_RandomEvent;
    [SerializeField]
    private Amb_Local_Wind amb_Local_Wind = default;

    private void OnEnable()
    {
        amb_RandomEvent = GetComponent<Amb_GetRandomEvent>();
        _hasPlayed = false;
    }

    private void Start()
    {
        Init_EventCollider();
    }

    public void Init_EventCollider()
    {
        event_Instance = RuntimeManager.CreateInstance(amb_RandomEvent.Amb_RandomEvent);
        event_Description = RuntimeManager.GetEventDescription(amb_RandomEvent.Amb_RandomEvent);
        event_Description.getMaximumDistance(out _maxDistance);
        event_Description.is3D(out _is3D);
        if (_is3D)
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());
        if (!_hasPlayed)
            Set_Collider();
    }

    private void Set_Collider()
    {
        event_Collider = gameObject.AddComponent<SphereCollider>();
        event_Collider.isTrigger = true;
        event_Collider.center = Vector3.zero;
        event_Collider.radius = _maxDistance;
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
        if (debug)
        {
            if (_state != _lastState)
            {
                Debug.Log("event_Instance on object: " + transform.parent.gameObject.name + " is in state: " + _event_State);
                _lastState = _state;
            }
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        if (_hasPlayed)
            Init_EventCollider();

        event_Instance.start();
        event_Instance.release();

        amb_Local_Wind.gameObject.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.getPlaybackState(out _event_State);
        CheckPlaybackState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        event_Instance.getPlaybackState(out _event_State);
        event_Instance.clearHandle();
        _hasPlayed = true;
    }

    public void Stop_Collider()
    {
        event_Collider.gameObject.SetActive(false);
    }
}