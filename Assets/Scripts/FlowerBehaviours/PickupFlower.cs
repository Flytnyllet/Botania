using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlower : InteractableSaving, IInteractable
{
    [SerializeField] string _flowerName;
    [SerializeField] Texture2D _pickupAlpha;

    [Tooltip("Används i fall ett annat objekt än det lokala skall tas bort vid upplockning")]
    [SerializeField] GameObject _gameobjectOverload;

    public bool Interact()
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

        Debug.Log(debugFlowerNames);

        FlowerLibrary.IncrementFlower(_flowerName, 1);

        if (_gameobjectOverload == null)
        {
            GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
        }
        else
        {
            _gameobjectOverload.GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
        }
        GetComponent<Collider>().enabled = false;
        return true; //Doesn't really have a purpose for this
    }
}
