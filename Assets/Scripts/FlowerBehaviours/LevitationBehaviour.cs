using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationBehaviour : MonoBehaviour
{
    bool _playerInside = false;
    [SerializeField]CharacterController _charCon;
    SphereCollider _collider;
    [SerializeField] Transform _targetTransform;
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
            _targetTransform = other.transform;
            _playerInside = true;
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
    //private void Update()
    //{
    //    if (_playerInside)
    //    {
    //        Vector3 direction = (transform.position - _targetTransform.position).normalized;
    //        direction.y = -1;
    //        _charCon.Move(direction * _fleeSpeed * Time.deltaTime);
    //    }
    //    else
    //    {
    //        _charCon.Move(_direction * Time.deltaTime);
    //    }
    //}
    

}
