using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupFlower : InteractableSaving, IInteractable
{
    public delegate void onPickUp(); 
    public static event onPickUp onPickUpEvent;

    bool _enabled = true;
    [SerializeField] bool _dissableTriggerafterPickup = false;
    public bool SetEnabled //kommer byta namn
    {
        get { return _enabled; }
        set { _enabled = value; }
    }
    [SerializeField] ItemDataContainer _flowerData;
    [SerializeField] Texture2D _pickupAlpha;

    [Tooltip("Används i fall ett annat objekt än det lokala skall tas bort vid upplockning")]
    [SerializeField] GameObject _gameobjectOverload;
    [SerializeField] UnityEvent _pickupAction; 

    public bool Interact()
    {
        if (_enabled)
        {
            //Pickup save system
            PickUp();
            
            NotificationObject.name = _flowerData.itemName;
            NotificationObject.sprite = _flowerData.itemIcon;
            if(onPickUpEvent != null)
                onPickUpEvent.Invoke();

            string debugFlowerNames = "Trying to pick up a " + _flowerData.name
                + ". Accepted flower types are: [";
            string[] flowerTypes = FlowerLibrary.GetAllFlowerNames();
            foreach (string flower in flowerTypes)
            {
                debugFlowerNames += flower + ", ";
            }
            debugFlowerNames += "]";

            //Debug.Log(debugFlowerNames);

            FlowerLibrary.IncrementFlower(_flowerData.name, 1);

            if (_gameobjectOverload == null)
            {
                if (_pickupAlpha != null)
                {
                    GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
                    GetComponent<Collider>().enabled = false; //This may not work since there are multiple colliders
                }
                else { Destroy(this.gameObject); }
            }
            else
            {
                if (_pickupAlpha != null)
                {
                    _gameobjectOverload.GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
                    GetComponent<Collider>().enabled = false;//This may not work since there are multiple colliders
                }
                else { Destroy(this.gameObject); }
            }
            if (_dissableTriggerafterPickup) GetComponent<Collider>().enabled = false;
            _pickupAction.Invoke();
            return true; //Doesn't really have a purpose for this
        }
        return false;
    }
}
