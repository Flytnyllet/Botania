using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    enum INTERACTION_TYPE { Flower, Door};
	[SerializeField]  INTERACTION_TYPE _interactionType= INTERACTION_TYPE.Flower;
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
