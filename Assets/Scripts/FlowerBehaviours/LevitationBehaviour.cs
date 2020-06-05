﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationBehaviour : MonoBehaviour
{
    bool _playerInside = false;
    bool _running = false;
    [SerializeField] CharacterController _charCon;
    SphereCollider _collider;
    [SerializeField] Vector3 _direction;
    [SerializeField] float _fleeSpeed;
    [SerializeField] float _playerDistanceTarget = 10;
    float _colliderStartRadius;


    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _colliderStartRadius = _collider.radius;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _collider.radius = _playerDistanceTarget;
            _playerInside = true;
            if (!_running)
            {
                StartCoroutine(MoveFromTarget(other.transform, 2));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _collider.radius = _colliderStartRadius;
            _playerInside = false;
        }
    }
    IEnumerator MoveFromTarget(Transform target, float deaccelerationTime)
    {
        _running = true;
        while (_playerInside)
        {
            while (_playerInside)
            {
                Vector3 direction = (transform.position - target.position).normalized;
                direction.y = -1;
                _charCon.Move(direction * _fleeSpeed * Time.deltaTime);
                yield return null;
            }
            float time = 0;
            while (time < deaccelerationTime)
            {
                if (_playerInside) { break; }

                Vector3 direction = (transform.position - target.position).normalized;
                direction.y = -1;
                _charCon.Move(direction * _fleeSpeed * (1 - time / deaccelerationTime) * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
        _running = false;
    }
}
