using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public int ButtonTarget;
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

    public void TriggerEventLocally()
    {
        events[ButtonTarget].action.Invoke();
    }
    public void CallEventTrigger()
    {
        EventManager.TriggerEvent(events[ButtonTarget].subscriptionName, new EventParameter());
    }

}
#if (UNITY_EDITOR)
[CustomEditor(typeof(EventHolder))]
public class EventHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EventHolder script = (EventHolder)target;
        if (GUILayout.Button("Call Event Action Locally"))
        {
            script.TriggerEventLocally();
        }
        if (GUILayout.Button("Call Event"))
        {
            script.CallEventTrigger();
        }
    }
}

#endif