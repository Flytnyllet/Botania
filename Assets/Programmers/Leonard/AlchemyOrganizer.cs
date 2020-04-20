using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyOrganizer : MonoBehaviour
{
	List<ItemLoader> _currentIngredients = new List<ItemLoader>();
	List<List<string>> _recipieList = new List<List<string>>();
	List<PotionLoader> _availablePotions = new List<PotionLoader>();
	[SerializeField] List<Image> _slotImages = new List<Image>();
	[SerializeField] Image _emptySlot;
	PotionLoader _result = new PotionLoader();
	//TestPotion _modifiers = new TestPotion();
	[SerializeField] float _potionDuration = 5;
	

	public void OnIngredientSlot(int slotIndex)
	{
		if(!_slotImages[slotIndex] == _emptySlot)
		{
			RemoveIngredient(slotIndex);
		}
	}
	public void OnIngredient()
	{
		if (_currentIngredients.Count >= 3)
		{

		}
	}

	void ActivatePotion()
	{
		Debug.Log("Activating Potion");
		//_modifiers.SpeedPot(FPSMovement.playerMovement);
		//StartCoroutine(ActivatePotion(_potionDuration));
	}

	/*IEnumerator ActivatePotion(float t)
	{
		_modifiers.SpeedPot(FPSMovement.playerMovement);
		Debug.Log("Speed Potion Applied");
		yield return new WaitForSeconds(t);
		_modifiers.SpeedPotionEnd(FPSMovement.playerMovement);
		Debug.Log("Speed Potion Removed");
	}*/


	void AddIngredient(ItemLoader ingredient)
	{
		if (_currentIngredients.Count >= 3)
		{
			_currentIngredients.Add(ingredient);
			Image ingredientImage = ingredient.GetComponentInChildren<Image>();
			Image empty = _slotImages.Find(x => x == _emptySlot);
			empty = ingredientImage;
		}
		else
		{
			Debug.Log("Slots are full");
		}
	}

	void RemoveIngredient(int index)
	{
		ItemLoader ingredient = _currentIngredients[index];
		_currentIngredients.RemoveAt(index);
		Image ingImage = _slotImages.Find(x => x == ingredient.GetComponentInChildren<Image>());
		ingImage = _emptySlot;
	}

	bool IsRecipe(List<string> recipe)
	{
		return _recipieList.Contains(recipe);
	}
	void AddResult()
	{

	}
	void CraftPotion()
	{

	}
}
