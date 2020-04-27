using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_ShyWhenClose : MonoBehaviour
{
    private EventRef_ShyWhenClose event_ShyWhenClose;

    private void Awake()
    {
        event_ShyWhenClose = GetComponentInChildren<EventRef_ShyWhenClose>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        event_ShyWhenClose.Set_Parameter(event_ShyWhenClose.isShyParameterId, 1);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        StartCoroutine(WaitBeforeNotShy(Random.Range(0, 5f)));
    }

    IEnumerator WaitBeforeNotShy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        event_ShyWhenClose.Set_Parameter(event_ShyWhenClose.isShyParameterId, 0);
    }
}
