using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_SetCollider : MonoBehaviour
{
    private EventInstance event_Instance;
    private EventDescription event_Description;
    private float maxDistance;
    private bool _is3D;

    private SphereCollider event_Collider;
    private Amb_GetRandomEvent amb_RandomEvent;
    [SerializeField]
    private Amb_Local_Wind amb_Local_Wind = default;

    private void OnEnable()
    {
        amb_RandomEvent = GetComponent<Amb_GetRandomEvent>();
        //amb_Local_Wind = GetComponentInChildren<Amb_Local_Wind>();
    }

    private void Start()
    {
        Init_EventCollider();
    }

    public void Init_EventCollider()
    {
        event_Instance = RuntimeManager.CreateInstance(amb_RandomEvent.Amb_RandomEvent);
        event_Description = RuntimeManager.GetEventDescription(amb_RandomEvent.Amb_RandomEvent);
        event_Description.getMaximumDistance(out maxDistance);
        event_Description.is3D(out _is3D);
        if (_is3D)
            RuntimeManager.AttachInstanceToGameObject(event_Instance, transform, GetComponent<Rigidbody>());
        Set_Collider();
    }

    private void Set_Collider()
    {
        event_Collider = gameObject.AddComponent<SphereCollider>();
        event_Collider.isTrigger = true;
        event_Collider.center = Vector3.zero;
        event_Collider.radius = maxDistance;
    }

    private void Stop_Collider()
    {
        event_Collider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.start();
        event_Instance.release();
        amb_Local_Wind.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        event_Instance.clearHandle();
        amb_Local_Wind.Stop_Local_Wind();
    }
}