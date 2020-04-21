using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoader : MonoBehaviour
{
	[SerializeField] string _flowerName;
	[SerializeField] Text textObject;
	int itemAmount = 0;
	//Flower _flower;

	/*
    void Start()
    {
		//itemAmount = FlowerLibrary.GetFlowerAmount(_flowerName);
		textObject.text = _flowerName + "\n x" + itemAmount;
		
	}*/

	void OnEnable()
	{
		//if(_flower != null)
		UpdateUI();
	}

	public string GetFlowerName()
	{
		return _flowerName;
	}

	public void UpdateUI()
	{
		itemAmount = FlowerLibrary.GetFlowerAmount(_flowerName);
		textObject.text = _flowerName + "\n x" + itemAmount;
	}
}
