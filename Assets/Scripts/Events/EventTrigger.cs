using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [Header("Trigger Options")]
    [SerializeField] Interactions interaction = Interactions.onButtonPress;
    enum Interactions { onEnter = 0, onButtonPress, onLeave };
    //[SerializeField] InteractionType interactionType = InteractionType.Direct;
    //public enum InteractionType { Direct = 0, PreAndPostTimer };
    [SerializeField] Actions postTriggerAction = Actions.Nothing;
    enum Actions { Nothing = 0, DeleteObject, DeactivateTrigger };

    [Header("Event Data")]
    public string[] eventCalls = new string[1];
    public EventParameter eventParameter;
    public UnityEvent action;

    void Engage()
    {
        foreach (string eventCall in eventCalls)
        {
            if (eventCall != "")
            {
                    EventManager.TriggerEvent(eventCall, eventParameter);
                    action.Invoke();
            }
            switch (postTriggerAction)
            {
                case Actions.DeleteObject:
                    Destroy(this.gameObject);
                    break;

                case Actions.DeactivateTrigger:
                    Destroy(this);
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interaction == Interactions.onEnter && collision.tag == "Player")
        {
            Engage();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (interaction == Interactions.onEnter && collision.gameObject.tag == "Player")
        {
            Engage();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (interaction == Interactions.onButtonPress && collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Engage();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (interaction == Interactions.onButtonPress && collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Engage();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interaction == Interactions.onLeave && collision.gameObject.tag == "Player")
        {
            Engage();
        }
    }
}
