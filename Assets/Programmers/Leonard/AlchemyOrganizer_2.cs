using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


enum RecipeState
{
	Unknown, Unavailable, Available
}

struct Recipe
{
	public GameObject button;
	public GameObject potionButton;
	public GameObject craftButton;
	public Text potionAmount;
	public Text[] ingredientAmount;
	public ItemDataContainer potionData;
	public PotionLoader potionScript;
	public Text buttonText;
	public RecipeEntry[] ingredientsData;
	public RecipeState state;
}

public class AlchemyOrganizer_2 : MonoBehaviour
{
	//[SerializeField] List<ItemLoader> _allIngredients = new List<ItemLoader>();
	static AlchemyOrganizer_2 alchemyOrganizer;
	List<ItemDataContainer> _discoveredFlowers = new List<ItemDataContainer>();
    [SerializeField] List<Recipe> _recipieList = new List<Recipe>();
	[SerializeField] GameObject _potionHolder = null;
    [SerializeField] List<PotionLoader> _availablePotions = new List<PotionLoader>();
    [SerializeField] List<Transform> _pages = new List<Transform>();
    [SerializeField] GameObject _potionButtonPrefab = null;
	[SerializeField] [Range(1, 10)] int _recipiesPerPage = 4;
    PotionLoader _result = new PotionLoader();

    void Awake()
	{
		alchemyOrganizer = this;
		SetupPotionLoaders();
		SetupRecipes();
	}

    void Start()
    {

    }

	void OnEnable()
	{
		UpdateUI();
	}

	public static void DiscoverRecipes(ItemDataContainer flower)
	{
		if (!alchemyOrganizer._discoveredFlowers.Contains(flower))
		{
			alchemyOrganizer._discoveredFlowers.Add(flower);
			for (int i = 0; i < alchemyOrganizer._recipieList.Count; i++)
			{
				Recipe recipe = alchemyOrganizer._recipieList[i];
				if (recipe.state != RecipeState.Available)
				{
					
					bool fullyDiscovered = true;
					bool discovered = false;

					for (int ii = 0; ii < recipe.ingredientsData.Length; ii++)
					{
						if(alchemyOrganizer._discoveredFlowers.Contains(recipe.ingredientsData[ii].ingredient))
						{
							discovered = true;
						}
						else
						{
							fullyDiscovered = false;
						}
					}

					if (fullyDiscovered == true)
					{
						alchemyOrganizer.UpdateRecipeToState(recipe, RecipeState.Available);
					}
					else if (discovered == true && recipe.state != RecipeState.Unavailable)
					{
						alchemyOrganizer.UpdateRecipeToState(recipe, RecipeState.Unavailable);
					}

				}
				/*for(int ii = 0; ii < alchemyOrganizer._recipieList[i].ingredientsData.Length; ii++)
				{
					if(alchemyOrganizer._recipieList[i].ingredientsData[ii].ingredient.itemName == flower.name)
					{
						bool fullDiscover = true;
						for(int i = 0; i < alchemyOrganizer._recipieList[i])
					}
				}*/
			}
		}
	}

	public List<Transform> GetPages()
    {
        return _pages;
    }

	void SetupPotionLoaders()
	{
		PotionLoader[] potionLoaders = _potionHolder.GetComponentsInChildren<PotionLoader>();
		_availablePotions.AddRange(potionLoaders);
	}

	void SetupRecipes()
    {
		int currentPage = 0;
		Transform targetPage = _pages[currentPage];
        for (int i = 0; i < _availablePotions.Count; i++)
        {
			if(i > 3)
			{
				currentPage = 1;
				targetPage = _pages[currentPage];
			}

			Recipe newRecipe = new Recipe();
			newRecipe.button = Instantiate(_potionButtonPrefab, targetPage);
			newRecipe.buttonText = newRecipe.button.transform.GetChild(2).GetComponent<Text>();
			newRecipe.state = RecipeState.Unknown;
			newRecipe.potionData = _availablePotions[i].GetPotionItemData();
			newRecipe.ingredientsData = _availablePotions[i].GetRecipe().ToArray();
			newRecipe.ingredientAmount = new Text[newRecipe.ingredientsData.Length];
			newRecipe.potionScript = _availablePotions[i];
			newRecipe.craftButton = newRecipe.button.transform.GetChild(0).gameObject;
			newRecipe.potionButton = newRecipe.button.transform.GetChild(1).gameObject;
			newRecipe.buttonText.text = newRecipe.potionData.itemName;
			newRecipe.potionAmount = newRecipe.potionButton.GetComponentInChildren<Text>();

			int nrIngredients = newRecipe.ingredientsData.Length;
			int nrIngredientObjects = newRecipe.button.transform.childCount - 2;
			GameObject ingredientObject = newRecipe.craftButton.transform.GetChild(0).gameObject;
			List<GameObject> ingCollect = new List<GameObject>();

			//Debug.LogFormat("Number of ingredients: {0}", newRecipe.ingredientsData.Length);
			ingredientObject.GetComponent<Image>().sprite = newRecipe.ingredientsData[0].ingredient.itemIcon;
			//Debug.LogFormat("Text object name: {0}", ingredientObject.GetComponentInChildren<Text>().name);
			newRecipe.ingredientAmount[0] = ingredientObject.GetComponentInChildren<Text>();
			ingCollect.Add(ingredientObject);

			if (nrIngredients > nrIngredientObjects)
			{
				for(int ii = nrIngredientObjects; ii < nrIngredients; ii++)
				{
					GameObject newIngObj = Instantiate(ingredientObject, newRecipe.craftButton.transform);
					newIngObj.transform.localPosition -= Vector3.right * 95 * ii;
					newIngObj.GetComponent<Image>().sprite = newRecipe.ingredientsData[ii].ingredient.itemIcon;

					newRecipe.ingredientAmount[ii] = newIngObj.GetComponentInChildren<Text>();

					ingCollect.Add(ingredientObject);	
				}
			}

			_recipieList.Add(newRecipe);
			//int index = i;
			newRecipe.craftButton.GetComponent<Button>().onClick.AddListener(delegate { CraftPotion(newRecipe); } ); //_recipieList[index]
			newRecipe.potionButton.GetComponent<Button>().onClick.AddListener(delegate { newRecipe.potionScript.ActivatePotion(); });
			UpdateRecipeToState(newRecipe);

			UpdateUI();
			//FixRecipeLayout(ingCollect);
		}
    }

	void UpdateUI()
	{
		//List<Transform> recipeHolders = new List<Transform>();

		//recipeHolders.AddRange(_pages[0].transform.GetComponentsInChildren<Transform>().Where(x => x.parent == _pages[0].transform));
		//recipeHolders.AddRange(_pages[1].transform.GetComponentsInChildren<Transform>().Where(x => x.parent == _pages[0].transform));


		for (int i = 0; i < _recipieList.Count; i++)
		{
			_recipieList[i].potionAmount.text = FlowerLibrary.GetPotionAmount(_recipieList[i].potionData.itemName).ToString();

			//Debug.LogFormat("Sanity Check RecipeList: {0}", _recipieList[i].ingredientAmount);
			for(int ii = 0; ii < _recipieList[i].ingredientAmount.Length; ii++)
			{
				_recipieList[i].ingredientAmount[ii].text = string.Format("{0}/{1}", 
					FlowerLibrary.GetFlowerAmount(_recipieList[i].ingredientsData[ii].ingredient.itemName), _recipieList[i].ingredientsData[ii].amount);
			}
		}
	}

	//void FixRecipeLayout(List<GameObject> ingredients)
	//{

	//}
	void UpdateRecipeToState(Recipe recipe, RecipeState state)
	{
		recipe.state = state;
		UpdateRecipeToState(recipe);
	}
	void UpdateRecipeToState(Recipe recipe)
	{
		Debug.LogFormat("Current Recipe State: {0}", recipe.state);

		if (recipe.state == RecipeState.Unknown)
		{
			Debug.Log("Yes");
			recipe.button.SetActive(false);
		}
		else if (recipe.state == RecipeState.Unavailable)
		{
			recipe.button.SetActive(true);
			recipe.craftButton.GetComponent<Button>().enabled = false;
		}
		else
		{
			Debug.Log("No");
			recipe.button.SetActive(true);
			recipe.craftButton.GetComponent<Button>().enabled = true;
		}
	}

	/*void RecipeStateChange(RecipeState newState, Recipe recipe)
	{
		if (newState == RecipeState.Unavailable)
		{
			if (recipe.state != RecipeState.Unknown)
			{
				Debug.LogError("Attempted to change Recipe to state Unavailable when not unknown");
			}
			else
			{
				recipe.state = RecipeState.Unavailable;
				recipe.button.SetActive(true);
				recipe.button.GetComponent<Button>().enabled = false;
			}
		}
		if (newState == RecipeState.Available)
		{
			if (recipe.state == RecipeState.Unknown)
			{
				recipe.state = RecipeState.Available;
				recipe.button.SetActive(true);
			}
			else if (recipe.state == RecipeState.Unavailable)
			{
				recipe.state = RecipeState.Available;
				recipe.button.GetComponent<Button>().enabled = true;
			}
			else
			{
				Debug.LogError("Attempted to change Recipe to Available when already available");
			}
		}
	}
	*/
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

   // void CreateAllRecipes()
   // {
   //     //string debug = "Creating all recipes, possible recipes are: ";
   //     //Debug.Log("Number of recipes: " + _availablePotions.Count);
   //     for (int i = 0; i < _availablePotions.Count; i++)
   //     {
   //         //Debug.Log("LOOP");
   //         List<ItemDataContainer> recipe = _availablePotions[i].GetRecipe();
   //         _recipieList.Add(recipe);
   //         /*debug += "\n \t ";
			//for (int j = 0; j < recipe.Count; j++)
			//{
			//	debug += recipe[j] + ", ";
			//}*/
   //     }
   //     //Debug.Log(debug);
   // }

    //void UpdateIngredients()
    //{
    //    foreach (ItemLoader ingredient in _allIngredients)
    //    {
    //        ingredient.UpdateUI();
    //    }
    //}
	bool CraftPotion(Recipe potionRecipe)
	{
		//Debug.Log("Test Crafting");
		for(int i = 0; i < potionRecipe.ingredientsData.Length; i++)
		{
			if (potionRecipe.ingredientsData[i].amount <= FlowerLibrary.GetFlowerAmount(potionRecipe.ingredientsData[i].ingredient.itemName))
			{
				
			}
			else
			{
				Debug.LogFormat("Not enough of ingredient {0}", potionRecipe.ingredientsData[i].ingredient.itemName);
				return false;
			}
		}

		potionRecipe.potionScript.AddPotion();
		UpdateUI();
		return true;
	}
 //   public void CraftPotion()
 //   {
	//	bool hasIngredients = true;
	//	//foreach (ItemLoader ingredient in _currentIngredients.Values)
	//	//{
	//	//	if (1 > FlowerLibrary.GetFlowerAmount(ingredient.GetItemData().itemName))
	//	//	{
	//	//		hasIngredients = false;
	//	//	}
	//	//}

	//	if (_result != null && hasIngredients)
	//	{
	//		_result.AddPotion();
	//	}
 //       //ClearAllIngredients();
 //       //UpdateIngredients();
	//}
	public List<PotionLoader> GetAllPotions()
	{
		return _availablePotions;
	}
    public void Debug_AddAllIngredients()
    {
		//List<string> allFlowers = new List<string>();
		for(int i = 0; i < _recipieList.Count; i++)
		{
			UpdateRecipeToState(_recipieList[i], RecipeState.Available);

			for(int ii = 0; ii < _recipieList[i].ingredientsData.Length; ii++)
			{
				Debug.LogFormat("Adding flower {0}", _recipieList[i].ingredientsData[ii].ingredient.itemName);
				FlowerLibrary.IncrementFlower(_recipieList[i].ingredientsData[ii].ingredient.itemName, 50);
			}
		}

		//Debug.LogFormat("FLÖWERS: {0}", allFlowers.Count);

		//UpdateIngredients();
		UpdateUI();
    }
}
