using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Local_Trigger : MonoBehaviour
{
    private float _rotation;
    private Transform _transform;
    private Transform _cameraTransform;

    private void Awake()
    {
        _transform = gameObject.transform;
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        _rotation = gameObject.transform.rotation.x;
    }

    private void Update()
    {
        _transform = _cameraTransform;
        //_rotation = 0;
    }
}
