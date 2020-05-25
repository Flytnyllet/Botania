using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Local_Trigger : MonoBehaviour
{
    private Amb_Local_Emitter _emitter;
    private SphereCollider _collider;

    private void OnEnable()
    {
        _emitter = GetComponentInChildren<Amb_Local_Emitter>();
        _emitter.Init_Event();
        Init_Trigger();
    }

    private void Init_Trigger()
    {
        _collider = gameObject.AddComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.center = Vector3.zero;
        _collider.radius = _emitter.MaxDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _emitter.Attach_Local_Emitter(transform, GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_emitter.IsPlaying)
            {
                _emitter.Stop_Local_Emitter();
            }
        }
    }

    private void OnDisable()
    {
        if (_emitter == null) { return; }

        if (_emitter.IsPlaying)
        {
            _emitter.Stop_Local_Emitter();
        }
    }
}