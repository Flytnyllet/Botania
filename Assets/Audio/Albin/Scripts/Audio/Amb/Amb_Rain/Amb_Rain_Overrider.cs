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
            Amb_Rain.Instance.Set_Rain_Override(0.85f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.GetCurrentBiome() == BiomeTypes.BIRCH)
            {
                Amb_Rain.Instance.Set_Rain_Override(0.7f);
            }
            else
            {
                Amb_Rain.Instance.Set_Rain_Override(0);
            }
        }
    }
}
