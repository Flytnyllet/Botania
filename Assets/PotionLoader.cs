using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionLoader : MonoBehaviour
{
	[SerializeField] int _potionID = -1;
	[SerializeField] Text textObject;
	List<Flower> recipe = new List<Flower>();
	Potion _potion;

	void Start()
	{
		_potion = FlowerLibrary.GetPotionType(_potionID);
		recipe.AddRange(_potion.Recipe);
		textObject.text = _potion.Name + "\n x" + _potion.Amount;
	}

	void OnEnable()
	{
		if (_potion != null)
		{
			textObject.text = _potion.Name + "\n x" + _potion.Amount;
		}
	}

	public void ActivatePotion()
	{
		//ADD FUNCTION CALL TO ACTIVATE EFFECT FROM LIST
	}
	public void AddPotion()
	{
		bool hasIngredients = true;
		for (int i = 0; i < recipe.Count; i++)
		{
			if(recipe[i].Amount < 1)
			{
				hasIngredients = false;
				break;
			}
		}
		for (int i = 0; i < recipe.Count; i++)
		{
			recipe[i].Amount--;
		}
		if (hasIngredients) FlowerLibrary.IncrementPotion(_potionID);
		else Debug.Log("Not enough flowers");
	}
}
