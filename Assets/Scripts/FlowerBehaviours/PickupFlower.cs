using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlower : MonoBehaviour, Interactable
{
    [SerializeField] int _ID;


    public bool Interact()
    {
        FlowerLibrary.IncrementFlower(_ID);
        Destroy(this.gameObject);
        return true;
    }

}
