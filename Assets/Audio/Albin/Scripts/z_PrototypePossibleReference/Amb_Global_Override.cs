using System;
using UnityEngine;
using FMODUnity;

public class Amb_Global_Override : MonoBehaviour
{
    [SerializeField]
    private readonly Amb_Global_Wind amb_Wind;
    [SerializeField]
    [Range(0f, 1f)]
    private float overrideValue;

    private SphereCollider amb_LocalCollider;

    [SerializeField]
    private float _overrideColliderRadius = default;
    private SphereCollider overrideCollider;

    private void Awake()
    {
        amb_LocalCollider = GetComponentInParent<SphereCollider>();
        CreateMaxValueCollider(_overrideColliderRadius);
    }

    private void CreateMaxValueCollider(float _radius)
    {
        overrideCollider = gameObject.AddComponent<SphereCollider>();
        overrideCollider.isTrigger = true;
        overrideCollider.center = Vector3.zero;
        overrideCollider.radius = _radius;
    }
}
