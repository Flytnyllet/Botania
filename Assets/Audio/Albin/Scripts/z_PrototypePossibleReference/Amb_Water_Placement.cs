using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Water_Placement : MonoBehaviour
{
    [SerializeField] private GameObject _nearWater;
    private SphereCollider _bounds;
    private Transform position;

    private void Awake()
    {
        _bounds = GetComponent<SphereCollider>();
    }

    private void Update()
    {

        //_nearWater.transform.position = _bounds.ClosestPointOnBounds()
    }
}
