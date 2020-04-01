using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    enum InteractionType { Flower, Door};
	[SerializeField] InteractionType INTERACTION_TYPE = InteractionType.Flower;
	[SerializeField] int _type; // Used for flower type, might be useful for locked doors etc.


    void Start()
    {
        
    }

	public void Interact()
	{
		FlowerLibrary.IncrementFlower(_type);
		Destroy(gameObject);
	}
}
