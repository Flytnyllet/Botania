using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_ShyBehaviour : MonoBehaviour
{
    private Amb_GetRandomEvent amb_RandomEvent;
    private SphereCollider _isShyCollider;

    private void OnEnable()
    {
        amb_RandomEvent = GetComponentInParent<Amb_GetRandomEvent>();
        _isShyCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }
        amb_RandomEvent.RandomEmitter.Set_Parameter_Name("is_shy", 1);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }
        StartCoroutine(WaitBeforeNotShy(Random.Range(1, 5)));
    }

    IEnumerator WaitBeforeNotShy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        amb_RandomEvent.RandomEmitter.Set_Parameter_Name("is_shy", 0);
    }
}
