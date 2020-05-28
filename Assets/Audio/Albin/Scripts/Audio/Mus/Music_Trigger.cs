using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Trigger : MonoBehaviour
{
    private Transform _camera;
    private bool _shouldTry = false;

    private void Start()
    {
        _camera = Camera.main.transform;
        _shouldTry = true;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _camera.position); 
        if (distance > 18) { return; }
        else
        {

            Vector3 dir = transform.position - _camera.position;
            float dot = Vector3.Dot(dir.normalized, _camera.transform.forward);

            if (dot > 0.7f && _shouldTry)
            {
                Debug.Log(transform.parent.name);

                switch (transform.parent.name)
                {
                    case "Lusthus":
                        Debug.Log("LUSTHUS!!!!!!!!!!");
                        break;
                    case "Flower_Magic":
                        Debug.Log("MAGIIIC!!!");
                        break;
                    case "Object_Gate(Clone)":
                        Debug.Log("GAAAATE!!!!!");
                        break;
                    case "Object_Grave_04(Clone)":
                        Debug.Log("GRAVE....RAAAAVE!!!!");
                        break;
                }
                _shouldTry = false;
            }

        }
    }
}