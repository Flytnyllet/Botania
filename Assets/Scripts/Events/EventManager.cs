using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class EventParameter //Add more Event Parameter Variables here if needed
{
    public string stringParam;
    public int intParam;
    public float floatParam;
    public Color colourParam = Color.white;
    public Vector2 posParam;
    public Transform transformParam;
}
// Right now when creating and subscribing Events, the function delegated needs to pass an EventParameter! 
// I will add a Function Overloads to remove this requirement in the future

public static class EventManager
{
    //Actions with parameters
    private static Dictionary<string, Action<EventParameter>> eventDicionaryA = new Dictionary<string, Action<EventParameter>>();

    //UnityActions with parameters
    private static Dictionary<string, UnityEvent<EventParameter>> eventDictionaryB = new Dictionary<string, UnityEvent<EventParameter>>();

    //Events subscribed to needs to be unsubsribed from as well, do this by adding a call for UnSubscribe() on the object's OnDissable call
    public static void Subscribe(string eventName, Action<EventParameter> subscription)
    {
        Action<EventParameter> thisEvent;
        if (eventDicionaryA.TryGetValue(eventName, out thisEvent))
        {
            //Add Event to an existing Action
            thisEvent += subscription;
            //Update Dictionary
            eventDicionaryA[eventName] = thisEvent;
        }
        else
        {
            thisEvent += subscription;
            //Create new action for the Dictionary
            eventDicionaryA.Add(eventName, thisEvent);
        }
    }

    public static void UnSubscribe(string eventName, Action<EventParameter> subscription)
    {
        Action<EventParameter> thisEvent;
        if (eventDicionaryA.TryGetValue(eventName, out thisEvent))
        {
            //Removes Event from existing Action
            thisEvent -= subscription;
            //Updates Dictionary
            eventDicionaryA[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, EventParameter param)
    {
        Action<EventParameter> thisEvent;
        if (eventDicionaryA.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(param);
        }
        else
        {
            Debug.LogError($"event name: {eventName} not found in eventDictionary");
        }
    }

    public static void PrintAllCalls()
    {
        foreach (KeyValuePair<string, Action<EventParameter>> call in eventDicionaryA)
        {
            Debug.Log(call.Key);
        }
    }
}
