using System;
using UnityEngine;
using FMODUnity;

public class Amb_SetGlobalOverride : MonoBehaviour
{
    [SerializeField]
    private GameObject globalOverrideObject = default;
    private SphereCollider overrideCollider;
    private Vector3 _closestPointOnOverride;

    private SphereCollider amb_LocalCollider;
    private StudioListener listener;

    private float _distance;
    private float _lastDistance;
    private float _maxDistance;
    private float _overrideValue;
    private double _d_overrideValue;
    private float _r_overrideValue;
    private float _lastOverrideValue;



    private void Awake()
    {
        amb_LocalCollider = GetComponent<SphereCollider>();
        listener = FindObjectOfType<StudioListener>();
    }

    private void Start()
    {
        overrideCollider = globalOverrideObject.GetComponent<SphereCollider>();

        // FÖRSÖK HITTA MAXDISTANCE MELLAN LOCALCOLLIDERS MAX OCH OVERRIDECOLLIDERS MIN

        _maxDistance = Vector3.Distance(amb_LocalCollider.bounds.max, _closestPointOnOverride);
        Debug.Log(_maxDistance);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        _closestPointOnOverride = overrideCollider.ClosestPoint(listener.transform.position);
        _distance = Vector3.Distance(_closestPointOnOverride, listener.transform.position);
        if (_distance != _lastDistance)
        {
            Debug.Log(_distance);

            //SetGlobalOverride();
            _lastDistance = _distance;
        }
    }

    private void SetGlobalOverride()
    {
        _overrideValue = Mathf.InverseLerp(_maxDistance, 0, _distance);
        _d_overrideValue = Math.Round(_overrideValue, 2);
        _r_overrideValue = Convert.ToSingle(_d_overrideValue);

        if (_r_overrideValue != _lastOverrideValue)
        {
            //Debug.Log(1 - _r_overrideValue);

            //amb_Wind.Set_Parameter(amb_Wind.ambGlobalOverrideParameterId, _overrideValue);
            _lastOverrideValue = _r_overrideValue;
        }

    }

}
