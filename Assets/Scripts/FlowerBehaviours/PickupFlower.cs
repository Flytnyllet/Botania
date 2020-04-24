using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlower : InteractableSaving, IInteractable
{
    bool _enabled = true;
    public bool SetEnabled //kommer byta namn
    {
        get { return _enabled; }
        set { _enabled = value; }
    }
    [SerializeField] string _flowerName;
    [SerializeField] Texture2D _pickupAlpha;

    [Tooltip("Används i fall ett annat objekt än det lokala skall tas bort vid upplockning")]
    [SerializeField] GameObject _gameobjectOverload;

    public bool Interact()
    {
        if (_enabled)
        {
            //Pickup save system
            PickUp();

            string debugFlowerNames = "Trying to pick up a " + _flowerName
                + ". Accepted flower types are: [";
            string[] flowerTypes = FlowerLibrary.GetAllFlowerNames();
            foreach (string flower in flowerTypes)
            {
                debugFlowerNames += flower + ", ";
            }
            debugFlowerNames += "]";

            //Debug.Log(debugFlowerNames);

            FlowerLibrary.IncrementFlower(_flowerName, 1);

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
            return true; //Doesn't really have a purpose for this
        }
        return false;
    }
}
