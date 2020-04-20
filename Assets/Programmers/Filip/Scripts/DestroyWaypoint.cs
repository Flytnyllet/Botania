using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyWaypoint : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MapGenerator.RemoveWaypoint(transform.parent.transform);
            Destroy(transform.parent.gameObject);
        }
    }
}
