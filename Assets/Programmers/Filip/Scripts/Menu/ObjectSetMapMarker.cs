using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetMapMarker : MonoBehaviour
{
    [SerializeField] MapMarkers _mapMarker;

    private void Start()
    {
        //MapGenerator.AddWorldMarkerGlobal(_mapMarker, transform.position, _mapMarker.ToString());
    }
}
