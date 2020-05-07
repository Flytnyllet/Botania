using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_AttachLocalWind : MonoBehaviour
{
    private Amb_Wind_Emitter _windEmitter;
    private SphereCollider _windCollider;
    private bool _isAttached = default;

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
        {
            _windEmitter = GetComponentInChildren<Amb_Wind_Emitter>();
            _windCollider = GetComponent<SphereCollider>();
            _windCollider.isTrigger = true;
            _windCollider.radius = Amb_Local_Wind.Instance._amb_Wind_List[0].MaxDistance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }

        _windEmitter.Init_Event(Amb_Local_Wind.Instance.Amb_Wind_Data[0]);
        Attach_Local_Wind();
    }

    private void Attach_Local_Wind()
    {
        _windEmitter.Attach_Wind_Emitter(transform, GetComponent<Rigidbody>());
        _isAttached = true;
    }

    private void Update()
    {
        if (_isAttached)
        {
            if (_windEmitter == null) { return; }
            _windEmitter.Set_Parameter(Amb_Global_Wind.Instance.wind_IntensityValue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }
        _windEmitter.Stop_Wind_Emitter();
        _isAttached = false;
    }
}
