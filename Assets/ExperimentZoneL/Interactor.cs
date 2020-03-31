using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
	[SerializeField] float distance = 100;
	[SerializeField] public KeyCode interactKey = KeyCode.E;
	[SerializeField] public int interactLayerMask = -1;

	void Start()
	{
		FlowerLibrary.InitiateLibrary();
		List<string> flowerNames = FlowerLibrary.GetAllFlowerNames();

		string debugFlowerNames = "All flower names: ";
		foreach(string name in flowerNames)
		{
			debugFlowerNames += name + " |";
		}
		
		Debug.Log(debugFlowerNames);
	}

	void Update()
    {
		if (Input.GetKeyDown(interactKey))
		{
			RaycastHit collision;
			bool hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * distance, out collision, distance);
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward)*distance, Color.green, 1f);

			if (hit && collision.collider.tag == "Flower")
			{
				Debug.Log("PickUp");
				
				collision.collider.gameObject.GetComponent<Interactable>().Interact();
			}
		}
    }
}
