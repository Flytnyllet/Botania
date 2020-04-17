using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyOrganizer : MonoBehaviour
{
	List<ItemLoader> _currentIngredients = new List<ItemLoader>();
	List<List<string>> _recipieList = new List<List<string>>();
	List<PotionLoader> _availablePotions = new List<PotionLoader>();
	PotionLoader _result = new PotionLoader();

	public void AddIngredient(ItemLoader ingredient)
	{
		if(_currentIngredients.Count >= 3)
		{
			_currentIngredients.Add(ingredient);
		}
		else
		{
			Debug.Log("Slots are full");
		}
	}

	public void RemoveIngredient(int index)
	{
		_currentIngredients.RemoveAt(index);
	}

	bool IsRecipe(List<string> recipe)
	{
		return _recipieList.Contains(recipe);
	}
	public void AddResult()
	{

	}
	public void CraftPotion()
	{

	}
}
