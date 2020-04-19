using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetMapMarker : MonoBehaviour
{
    [SerializeField] Sprite _sprite;

    private void Start()
    {
        MapGenerator.AddWorldMarker(_sprite, transform.position);
    }
}
