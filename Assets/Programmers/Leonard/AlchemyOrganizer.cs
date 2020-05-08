using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class AlchemyOrganizer : MonoBehaviour
{
    [SerializeField] List<ItemLoader> _allIngredients = new List<ItemLoader>();
    [SerializeField] Dictionary<int, ItemLoader> _currentIngredients = new Dictionary<int, ItemLoader>();
    [SerializeField, Tooltip("Includes icons for all ingredients and potions")]
    List<ItemDataContainer> _items = new List<ItemDataContainer>();
    //ImageDictionary _imageCollection = new ImageDictionary();
    //Dictionary<string, Image> _imageCollection = new Dictionary<string, Image>();
    /*[SerializeField] List<ItemLoader> _currentIngredients = new List<ItemLoader>();
	int nextEmptySlot = 0;*/
    [SerializeField] List<List<ItemDataContainer>> _recipieList = new List<List<ItemDataContainer>>();
    [SerializeField] List<PotionLoader> _availablePotions = new List<PotionLoader>();
    [SerializeField] List<GameObject> _slots = new List<GameObject>();
    [SerializeField] GameObject _potionSlot = null;
    [SerializeField] List<Transform> _pages = new List<Transform>();
    [SerializeField] GameObject _recipePrefab = null;
    PotionLoader _result = new PotionLoader();

    void Awake()
    {
        AddIngredientButton();
    }
    void Start()
    {
        SetupRecipes();
    }
    //private void OnEnable()
    //{
    //    UpdateIngredients();
    //}
    public List<Transform> GetPages()
    {
        return _pages;
    }
    void SetupRecipes()
    {
        for (int i = 0; i < _availablePotions.Count; i++)
        {
            Sprite potionIcon;
            if (GetImageFromCollection(_availablePotions[i].GetPotionItemData(), out potionIcon))
            {
                GameObject go = Instantiate<GameObject>(_recipePrefab, _pages[2]);
                Image[] imageObjects = go.GetComponentsInChildren<Image>();

                Text recipeName = go.GetComponentInChildren<Text>();
                //Debug.Log("Name of recipe object: " + recipeName.name);
                recipeName.text = _availablePotions[i].GetPotionItemData().itemName;

                Sprite ingredientIcon;
                for (int ii = 0; ii < imageObjects.Length; ii++)
                {
                    List<ItemDataContainer> recipe = _availablePotions[i].GetRecipe();
                    if (imageObjects[ii].name.Contains("1"))
                    {
                        if (GetImageFromCollection(recipe[0], out ingredientIcon))
                        {
                            imageObjects[ii].sprite = ingredientIcon;
                        }
                    }
                    else if (imageObjects[ii].name.Contains("2"))
                    {
                        if (recipe.Count > 1)
                        {
                            if (GetImageFromCollection(recipe[1], out ingredientIcon))
                            {
                                imageObjects[ii].sprite = ingredientIcon;
                            }
                        }
                        else
                        {
                            imageObjects[ii].gameObject.SetActive(false);
                        }
                    }
                    else if (imageObjects[ii].name.Contains("3"))
                    {
                        if (recipe.Count > 2)
                        {
                            if (GetImageFromCollection(recipe[2], out ingredientIcon))
                            {
                                imageObjects[ii].sprite = ingredientIcon;
                            }
                        }
                        else
                        {
                            imageObjects[ii].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        imageObjects[ii].sprite = potionIcon;
                    }
                }
            }
        }
    }
    bool GetImageFromCollection(ItemDataContainer identifier, out Sprite image)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] == identifier)
            {
                image = _items[i].itemIcon;
                return true;
            }
        }
        Debug.LogWarning("OBS! Every potion and flower name should be connected to an entry in the Image Collection, this is not true for "
            + identifier);
        image = null;
        return false;
    }

    void AddIngredientButton()
    {
        for (int i = 0; i < _allIngredients.Count; i++)
        {
            ItemLoader itemLoader = _allIngredients[i].GetComponent<ItemLoader>();
            _allIngredients[i].GetComponent<Button>().onClick.AddListener(delegate { AddIngredient(itemLoader); });
        }
    }
    /*
	void ActivatePotion(PotionType type, float duration, int )
	{
		Debug.Log("Activating Potion");
		_modifiers.PotionEffectStart(FPSMovement.playerMovement);
		//StartCoroutine(ActivatePotion(_potionDuration));
	}*/
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
    List<ItemDataContainer> GetIngredientNames()
    {
        List<ItemDataContainer> itemNames = new List<ItemDataContainer>();

        foreach (ItemLoader item in _currentIngredients.Values)
        {
            itemNames.Add(item.GetItemData());
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

    bool IsRecipe(List<ItemDataContainer> recipe)
    {
        if (_recipieList.Count < 1)
        {
            CreateAllRecipes();
        }

        bool isRecipe = false;
        int index = -1;
        for (int i = 0; i < _recipieList.Count; i++)
        {
            if (CompareRecipes(recipe, _recipieList[i]))
            {
                isRecipe = true;
                index = i;
                break;
            }
        }

        if (isRecipe)
        {
            //Debug.Log("Recipe available");
            if (_potionSlot.transform.childCount > 0)
            {
                Destroy(_potionSlot.transform.GetChild(0).gameObject);
            }

            //int index = _recipieList.IndexOf(recipe);
            //Debug.Log("Index of recipe is: " + index);
            //PotionLoader potion = _availablePotions[index];
            _result = _availablePotions[index];
            GameObject imageGO = _result.gameObject.GetComponentInChildren<Image>().gameObject;

            RectTransform image = Instantiate(imageGO, _potionSlot.transform).GetComponent<RectTransform>();
            image.localPosition = Vector3.zero;
        }
        else
        {
			_result = null;
            if (_potionSlot.transform.childCount > 0)
            {
                Destroy(_potionSlot.transform.GetChild(0).gameObject);
            }
        }

        return isRecipe;
    }

    bool CompareRecipes(List<ItemDataContainer> recipeA, List<ItemDataContainer> recipeB)
    {
        //List<ItemDataContainer> checkedItems = new List<ItemDataContainer>();
        if (recipeA.Count != recipeB.Count)
        {
            return false;
        }
		//List<int> checkedIndex = new List<int>();
		List<string> recipeANames = recipeA.Select(recipe => recipe.itemName).ToList();
		List<string> recipeBNames = recipeB.Select(recipe => recipe.itemName).ToList();

		recipeANames.Sort();
		recipeBNames.Sort();


		for (int i = 0; i < recipeA.Count && _recipieList[i] != null; i++)
		{
			if(recipeANames[i] != recipeBNames[i])
			{
				return false;
			}
			//if (!recipeB.Exists(x => x == recipeA[i] &&
			//		//&& !checkedIndex.Contains(i)))
			//{
			//	return false;
			//}
			//else
			//{
			//	checkedIndex.Add(recipeB.IndexOf(recipeA[i]));
			//}
		}

		return true;
		//     for (int i = 0; i < recipeA.Count && _recipieList[i] != null; i++)
		//     {
		//         if (recipeA[i] != recipeB[i])
		//         {
		//             return false;
		//         }
		//         /*if(recipeB.Contains(recipeA[i]))
		//{

		//	checkedStrings.Add(recipeA[i]);
		//}*/
		//     }
	}

    void CreateAllRecipes()
    {
        //string debug = "Creating all recipes, possible recipes are: ";
        //Debug.Log("Number of recipes: " + _availablePotions.Count);
        for (int i = 0; i < _availablePotions.Count; i++)
        {
            //Debug.Log("LOOP");
            List<ItemDataContainer> recipe = _availablePotions[i].GetRecipe();
            _recipieList.Add(recipe);
            /*debug += "\n \t ";
			for (int j = 0; j < recipe.Count; j++)
			{
				debug += recipe[j] + ", ";
			}*/
        }
        //Debug.Log(debug);
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
		bool hasIngredients = true;
		foreach (ItemLoader ingredient in _currentIngredients.Values)
		{
			if (1 > FlowerLibrary.GetFlowerAmount(ingredient.GetItemData().itemName))
			{
				hasIngredients = false;
			}
		}

		if (_result != null && hasIngredients)
		{
			_result.AddPotion();
		}
        //ClearAllIngredients();
        UpdateIngredients();
	}
	public List<PotionLoader> GetAllPotions()
	{
		return _availablePotions;
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
