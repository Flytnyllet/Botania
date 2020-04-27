using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class EventRef_RandomOneShot : MonoBehaviour
{
    [EventRef]
    public string event_Ref;
    private EventDescription event_Description;
    [HideInInspector]
    public float maxDistance;
    private float minDistance;
    private bool iconIsActive = false;
    private Vector3 iconPosition;

    public bool randomizeY;
    public float waitTimeRange_Min;
    public float waitTimeRange_Max;

    [SerializeField]
    private bool debug = default;

    private void Start()
    {
        Init_Ref();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (iconIsActive)
            Gizmos.DrawIcon(iconPosition, "FMOD/FMODEmitter.tiff", true);

        if (debug)
            Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    public void Init_Ref()
    {
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.getMaximumDistance(out maxDistance);
        event_Description.getMinimumDistance(out minDistance);
    }

    public void Init_OneShot(Vector3 position, float waitTime)
    {
        RuntimeManager.PlayOneShot(event_Ref, position);
        iconPosition = position;
        StartCoroutine(DrawIconForSeconds(waitTime));
    }

    private IEnumerator DrawIconForSeconds(float waitTime)
    {
        iconIsActive = true;
        yield return new WaitForSeconds(waitTime);
        iconIsActive = false;
    }
}

