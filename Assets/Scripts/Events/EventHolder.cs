using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class EventBox
{
    public string subscriptionName;
    public UnityEvent action;
    public void TriggerEvent(EventParameter eventParam)
    {
        action.Invoke();
    }
}

public class EventHolder : MonoBehaviour
{
    public EventBox[] events;
    private void OnEnable()
    {
        for (int i = 0; i < events.Length; i++)
        {
            EventManager.Subscribe(events[i].subscriptionName, events[i].TriggerEvent);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < events.Length; i++)
        {
            EventManager.UnSubscribe(events[i].subscriptionName, events[i].TriggerEvent);
        }
    }
}
