using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyOrganizer : MonoBehaviour
{
	[SerializeField] List<ItemLoader> _allIngredients = new List<ItemLoader>();
	[SerializeField] Dictionary<int, ItemLoader> _currentIngredients = new Dictionary<int, ItemLoader>();
	/*[SerializeField] List<ItemLoader> _currentIngredients = new List<ItemLoader>();
	int nextEmptySlot = 0;*/
	[SerializeField] List<List<string>> _recipieList = new List<List<string>>();
	[SerializeField] List<PotionLoader> _availablePotions = new List<PotionLoader>();
	[SerializeField] List<GameObject> _slots = new List<GameObject>();
	[SerializeField] GameObject _potionSlot;
	PotionLoader _result = new PotionLoader();
	TestPotion _modifiers = new TestPotion();
	[SerializeField] float _potionDuration = 5;


	void Awake()
	{
		AddIngredientButton();
	}

	void AddIngredientButton()
	{
		for (int i = 0; i < _allIngredients.Count; i++)
		{
			ItemLoader itemLoader = _allIngredients[i].GetComponent<ItemLoader>();
			_allIngredients[i].GetComponent<Button>().onClick.AddListener(delegate { AddIngredient(itemLoader); });
		}
	}


	void ActivatePotion()
	{
		Debug.Log("Activating Potion");
		_modifiers.SpeedPot(FPSMovement.playerMovement);
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
		if (FlowerLibrary.GetFlowerAmount(ingredient.GetFlowerName()) > 0)
		{
			Debug.Log("Name of ingredient object: " + ingredient.gameObject.name + "\n Current number of ingredients: " + _currentIngredients.Count);
			if (_currentIngredients.Count < 3)
			{
				int key = FindNextEmptyKey();
				_currentIngredients.Add(key, ingredient);

				GameObject imageGO = ingredient.GetComponentInChildren<Image>().gameObject;
				GameObject slotImage = Instantiate(imageGO, _slots[key].transform);

				IsRecipe(GetIngredientNames());

				Debug.Log("Ingredient index: " + key);

				/*Image ingredientImage = ingredient.GetComponentInChildren<Image>();
				Image empty = _slotImages.Find(x => x == _emptySlot);
				//Debug.Log("SlotImage: " + empty.gameObject);

				empty.gameObject.GetComponent<Image>().sprite = ingredientImage.sprite;
				empty.gameObject.GetComponent<Image>().col	or = ingredientImage.color;*/
			}
			else
			{
				Debug.Log("Slots are full");
			}
		}
		else
		{
			Debug.Log("Not enough ingredients");
		}
	}

	int FindNextEmptyKey()
	{
		int i = 0;
		while (i < 10)
		{
			if (!_currentIngredients.ContainsKey(i))
				return i;
			i++;
		}
		return -1;
	}

	public void ClearAllIngredients()
	{
		foreach (int key in _currentIngredients.Keys)
		{
			Destroy(_slots[key].transform.GetChild(_slots[key].transform.childCount - 1).gameObject);
			//if (_slots[key].transform.childCount > 0)
		}
		_currentIngredients.Clear();

		IsRecipe(GetIngredientNames());
	}

	public void RemoveIngredient(int index)
	{
		Image image = _slots[index].GetComponentInChildren<Image>();
		Destroy(_slots[index].transform.GetChild(_slots[index].transform.childCount - 1).gameObject);
		_currentIngredients.Remove(index);

		IsRecipe(GetIngredientNames());
		/*for(int i = 0; i < _allIngredients.Count; i++)
		{
			if(_allIngredients[i].GetComponentInChildren<Image>() == image)
			{
				_currentIngredients.Remove(_allIngredients[i]);
			}
		}*/
	}

	List<string> GetIngredientNames()
	{
		List<string> itemNames = new List<string>();

		foreach (ItemLoader item in _currentIngredients.Values)
		{
			itemNames.Add(item.GetFlowerName());
		}
		return itemNames;
	}

	/*List<string> GetItemNames()
	{
		List<string> itemNames = new List<string>();
		for (int i = 0; i < _allIngredients.Count; i++)
		{
			itemNames.Add(_allIngredients[i].GetFlowerName());
		}
		return itemNames;
	}*/

	bool IsRecipe(List<string> recipe)
	{
		if (_recipieList.Count < 1)
		{
			CreateAllRecipes();
		}

		bool isRecipe = false;
		int index = -1;
		for(int i = 0; i < _recipieList.Count; i++)
		{
			if (CompareRecipes(_recipieList[i], recipe))
			{
				isRecipe = true;
				index = i;
			}
		}

		if (isRecipe)
		{
			Debug.Log("Recipe available");
			if (_potionSlot.transform.childCount > 0)
			{
				Destroy(_potionSlot.transform.GetChild(0).gameObject);
			}
			
			//int index = _recipieList.IndexOf(recipe);
			Debug.Log("Index of recipe is: " + index);
			//PotionLoader potion = _availablePotions[index];
			_result = _availablePotions[index];
			GameObject imageGO = _result.gameObject.GetComponentInChildren<Image>().gameObject;

			RectTransform image = Instantiate(imageGO, _potionSlot.transform).GetComponent<RectTransform>();
			image.localPosition = Vector3.zero;
		}
		else
		{
			if (_potionSlot.transform.childCount > 0)
			{
				Destroy(_potionSlot.transform.GetChild(0).gameObject);
			}
		}

		return isRecipe;
	}

	bool CompareRecipes(List<string> recipeA, List<string> recipeB)
	{
		List<string> checkedStrings = new List<string>();
		if (recipeA.Count != recipeB.Count)
		{
			return false;
		}
		for(int i = 0; i < recipeA.Count; i++)
		{
			if(recipeA[i] != recipeB[i])
			{
				return false;
			}
			/*if(recipeB.Contains(recipeA[i]))
			{
				
				checkedStrings.Add(recipeA[i]);
			}*/
		}
		return true;
	}

	void CreateAllRecipes()
	{
		string debug = "Creating all recipes, possible recipes are: ";
		Debug.Log("Number of recipes: " + _availablePotions.Count);
		for (int i = 0; i < _availablePotions.Count; i++)
		{
			Debug.Log("LOOP");
			List<string> recipe = _availablePotions[i].GetRecipe();
			_recipieList.Add(recipe);
			debug += "\n \t ";
			for (int j = 0; j < recipe.Count; j++)
			{
				debug += recipe[j] + ", ";
			}
		}
		Debug.Log(debug);
	}

	void UpdateIngredients()
	{
		foreach (ItemLoader ingredient in _allIngredients)
		{
			ingredient.UpdateUI();
		}
	}
	public void CraftPotion()
	{
		_result.AddPotion();
		ClearAllIngredients();
		UpdateIngredients();

	}
	public void Debug_AddAllIngredients()
	{
		for (int i = 0; i < _allIngredients.Count; i++)
		{
			FlowerLibrary.IncrementFlower(_allIngredients[i].GetFlowerName(), 50);
		}
		UpdateIngredients();
	}
}
