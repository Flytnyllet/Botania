using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Singleton
    static Player _thisSingleton;

    static Transform _playerTransform;

    private void Awake()
    {
        if (_thisSingleton == null)
            _thisSingleton = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _playerTransform = GetComponent<Transform>();
    }

    public static Transform GetPlayerTransform()
    {
        return _playerTransform;
    }
}
