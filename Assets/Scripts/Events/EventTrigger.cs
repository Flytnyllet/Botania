using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [Header("Trigger Options")]
    [SerializeField] INTERACTIONS interaction = INTERACTIONS.onButtonPress; 
    enum INTERACTIONS { onEnter = 0, onButtonPress, onLeave };

    [SerializeField] ACTIONS postTriggerAction = ACTIONS.Nothing;
    enum ACTIONS { Nothing = 0, DeleteObject, DeactivateTrigger };

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
                case ACTIONS.DeleteObject:
                    Destroy(this.gameObject);
                    break;

                case ACTIONS.DeactivateTrigger:
                    Destroy(this);
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interaction == INTERACTIONS.onEnter && collision.tag == "Player")
        {
            Engage();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (interaction == INTERACTIONS.onEnter && collision.gameObject.tag == "Player")
        {
            Engage();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (interaction == INTERACTIONS.onButtonPress && collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Engage();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (interaction == INTERACTIONS.onButtonPress && collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Engage();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interaction == INTERACTIONS.onLeave && collision.gameObject.tag == "Player")
        {
            Engage();
        }
    }
}
