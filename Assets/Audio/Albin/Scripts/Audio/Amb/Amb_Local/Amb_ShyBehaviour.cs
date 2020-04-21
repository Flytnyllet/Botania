using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_ShyBehaviour : MonoBehaviour
{
    private Amb_SetCollider amb_SetCollider;

    private void OnEnable()
    {
        amb_SetCollider = GetComponentInParent<Amb_SetCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        amb_SetCollider.Set_Parameter(amb_SetCollider.IsShyParameterId, 1);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        StartCoroutine(WaitBeforeNotShy(Random.Range(1, 5)));
    }

    IEnumerator WaitBeforeNotShy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        amb_SetCollider.Set_Parameter(amb_SetCollider.IsShyParameterId, 0);
    }
}
