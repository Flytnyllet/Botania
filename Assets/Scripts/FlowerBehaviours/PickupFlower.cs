using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlower : MonoBehaviour, Interactable
{
    [SerializeField] string _flowerName;

	public bool Interact()
    {
		string debugFlowerNames = "Trying to pick up a " + _flowerName
			+ ". Accepted flower types are: [";
		string[] flowerTypes = FlowerLibrary.GetAllFlowerNames();
		foreach (string flower in flowerTypes)
		{
			debugFlowerNames += flower+", ";
		}
		debugFlowerNames += "]";

		Debug.Log(debugFlowerNames);

		FlowerLibrary.IncrementFlower(_flowerName,1);

        Destroy(this.gameObject);
        return true;
    }

}
