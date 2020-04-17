using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_RandomOneShot : MonoBehaviour
{
    private SphereCollider amb_RandomOneShotCollider;
    private EventRef_RandomOneShot event_RandomOneShots;
    private bool waiting;
    private Vector3 randomOneShotPosition;
    private float randomX;
    private float randomY;
    private float randomZ;

    private void Awake()
    {
        amb_RandomOneShotCollider = GetComponentInParent<SphereCollider>();
        event_RandomOneShots = GetComponentInChildren<EventRef_RandomOneShot>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        if (!waiting)
            StartCoroutine(WaitBeforeRandomOneShot(Random.Range(event_RandomOneShots.waitTimeRange_Min, event_RandomOneShots.waitTimeRange_Max)));
    }

    private IEnumerator WaitBeforeRandomOneShot(float waitTime)
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        GetRandomPosition();
        event_RandomOneShots.Init_OneShot(randomOneShotPosition, waitTime);
        waiting = false;
    }

    private void GetRandomPosition()
    {
        randomX = Random.Range(amb_RandomOneShotCollider.bounds.min.x, amb_RandomOneShotCollider.bounds.max.x);

        if (event_RandomOneShots.randomizeY)
            randomY = Random.Range(amb_RandomOneShotCollider.bounds.min.y, amb_RandomOneShotCollider.bounds.max.y);
        else
            randomY = amb_RandomOneShotCollider.bounds.center.y;

        randomZ = Random.Range(amb_RandomOneShotCollider.bounds.min.z, amb_RandomOneShotCollider.bounds.max.z);

        randomOneShotPosition = new Vector3(randomX, randomY, randomZ);
    }
}
