using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Trigger : MonoBehaviour
{
    private Transform _camera;
    private bool _shouldTry = false;
    private int _track;

    private void Start()
    {
        _camera = Camera.main.transform;

        switch (transform.parent.name)
        {
            case "Lusthus":
                _shouldTry = true;
                break;
            case "Flower_Magic_Group":
                if (Random.Range(0, 1f) > 0.05f)
                    _shouldTry = true;
                else
                    _shouldTry = false;
                break;
            case "Object_Gate(Clone)":
                if (Random.Range(0, 1f) > 0.5f)
                    _shouldTry = true;
                else
                    _shouldTry = false;
                break;
            case "Object_Well(Clone)":
                if (Random.Range(0, 1f) > 0.5f)
                    _shouldTry = true;
                else
                    _shouldTry = false;
                break;
            case "Object_Grave_04(Clone)":
                if (Random.Range(0, 1f) > 0.5f)
                    _shouldTry = true;
                else
                    _shouldTry = false;
                break;
            case "Object_Pole(Clone)":
                if (Music_Manager.Instance.PlayOnlyFirstTime)
                    _shouldTry = true;
                else
                    _shouldTry = false;
                break;
        }
    }

    private void Update()
    {
        if (_shouldTry &&
            !Music_Manager.Instance.IsPlaying &&
            !Music_Manager.Instance.IsCooldown)
        {
            float distance = Vector3.Distance(transform.position, _camera.position);
            if (distance > 15) { return; }
            else
            {

                Vector3 dir = transform.position - _camera.position;
                float dot = Vector3.Dot(dir.normalized, _camera.transform.forward);

                if (dot > 0.7f)
                {
                    switch (transform.parent.name)
                    {
                        case "Object_Grave_04(Clone)":
                            _track = 1;
                            break;
                        case "Lusthus":
                            _track = 2;
                            break;
                        case "Object_Gate(Clone)":
                            _track = 2;
                            break;
                        case "Object_Well(Clone)":
                            _track = 2;
                            break;
                        case "Flower_Magic":
                            _track = 3;
                            break;
                        case "Object_Pole(Clone)":
                            _track = 5;
                            break;
                    }
                    Music_Manager.Instance.Init_Music(_track);
                    Debug.Log("Initialised music for " + transform.parent.name);
                    _shouldTry = false;
                }
            }
        }
    }
}