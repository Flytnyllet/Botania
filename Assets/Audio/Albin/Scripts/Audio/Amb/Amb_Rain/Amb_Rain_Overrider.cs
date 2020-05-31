using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Rain_Overrider : MonoBehaviour
{
    private SphereCollider _trigger;

    private void Awake()
    {
        _trigger = GetComponent<SphereCollider>();
        if (_trigger == null) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Amb_Rain.Instance.Set_Rain_Override(0.9f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Amb_Rain.Instance.Stop_Rain_Override();
        }
    }
}
