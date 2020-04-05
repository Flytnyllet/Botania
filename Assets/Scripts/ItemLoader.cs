using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoader : MonoBehaviour
{
	[SerializeField] int _flowerID = -1;
	[SerializeField] Text textObject;
	Flower _flower;

    void Start()
    {
		_flower = FlowerLibrary.GetFlowerType(_flowerID);
		textObject.text = _flower.Name + "\n x" + _flower.Amount;
    }

	void OnEnable()
	{
		if(_flower != null)
		{
			textObject.text = _flower.Name + "\n x" + _flower.Amount;
		}
	}
}
