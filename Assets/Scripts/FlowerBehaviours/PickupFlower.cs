using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlower : MonoBehaviour, IInteractable
{
    [SerializeField] string _flowerName;
    [Tooltip("Används i fall ett annat objekt än det lokala skall tas bort vid upplockning")]
    [SerializeField] GameObject _destroyOverload;

    public bool Interact()
    {
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

        if (_destroyOverload == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(_destroyOverload);
        }

        return true; //Doesn't really have a purpose for this (Når man ens booleans efter Destroy()?)
    }
}
