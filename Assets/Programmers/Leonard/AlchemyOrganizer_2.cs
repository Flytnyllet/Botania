using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


enum RecipeState
{
    Unknown, Available //, Unavailable
}

struct Recipe
{
    public GameObject button;
    public GameObject potionButton;
    public GameObject craftButton;
    public Text potionAmount;
    public Image potionImage;
    public Text[] ingredientAmount;
    public Image[] ingredientImages;
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
    List<ItemDataContainer> _allIngredients = new List<ItemDataContainer>();
    [SerializeField] List<Recipe> _recipieList = new List<Recipe>();
    [SerializeField] GameObject _potionHolder = null;
    [SerializeField] List<PotionLoader> _availablePotions = new List<PotionLoader>();
    [SerializeField] List<Transform> _pages = new List<Transform>();
    [SerializeField] GameObject _potionButtonPrefab = null;
    [SerializeField] [Range(1, 10)] int _recipiesPerPage = 4;
    [SerializeField] Sprite _unknownIngredient = null;
    PotionLoader _result = new PotionLoader();

    void Awake()
    {
        OnAwake();
    }

    void OnEnable()
    {
        UpdateUI();
    }

    void OnAwake()
    {
        alchemyOrganizer = this;
        SetupPotionLoaders();
        SetupRecipes();
        CreateIngredientList();
    }

    void CreateIngredientList()
    {
        for (int i = 0; i < _recipieList.Count; i++)
        {
            for (int ii = 0; ii < _recipieList[i].ingredientsData.Length; ii++)
            {
                if (!_allIngredients.Contains(_recipieList[i].ingredientsData[ii].ingredient))
                {
                    _allIngredients.Add(_recipieList[i].ingredientsData[ii].ingredient);
                }
            }
        }
    }

    public static void UpdateAfterLoad()
    {
        try
        {
            string[] allSavedFlowers = FlowerLibrary.GetAllFlowerNames();
            List<ItemDataContainer> allIngredients = alchemyOrganizer._allIngredients;
            for (int i = 0; i < allIngredients.Count; i++)
            {
                if (allSavedFlowers.Contains(allIngredients[i].itemName))
                {
                    DiscoverRecipes(allIngredients[i]);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
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
                    if (recipe.ingredientsData[0].ingredient.itemName == flower.itemName)
                    {
                        alchemyOrganizer.UpdateRecipeToState(recipe, RecipeState.Available);
                    }

                    //bool fullyDiscovered = true;
                    //bool discovered = false;

                    //for (int ii = 0; ii < recipe.ingredientsData.Length; ii++)
                    //{
                    //	if(alchemyOrganizer._discoveredFlowers.Contains(recipe.ingredientsData[ii].ingredient))
                    //	{
                    //		discovered = true;
                    //	}
                    //	else
                    //	{
                    //		fullyDiscovered = false;
                    //	}
                    //}

                    //if (fullyDiscovered == true)
                    //{
                    //	alchemyOrganizer.UpdateRecipeToState(recipe, RecipeState.Available);
                    //}
                    //// Unavailable Removed
                    ////else if (discovered == true && recipe.state != RecipeState.Unavailable)
                    ////{
                    ////	alchemyOrganizer.UpdateRecipeToState(recipe, RecipeState.Unavailable);
                    ////}

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

    public void SetupPotionLoaders()
    {
        if (_availablePotions.Count < 1)
        {
            PotionLoader[] potionLoaders = _potionHolder.GetComponentsInChildren<PotionLoader>(true);
            _availablePotions.AddRange(potionLoaders);
        }
    }

    void SetupRecipes()
    {
        int currentPage = 0;
        Transform targetPage = _pages[currentPage];
        for (int i = 0; i < _availablePotions.Count; i++)
        {
            if (i > 3)
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

            int nrIngredients = newRecipe.ingredientsData.Length;

            newRecipe.ingredientAmount = new Text[nrIngredients];
            newRecipe.ingredientImages = new Image[nrIngredients];
            newRecipe.potionScript = _availablePotions[i];
            newRecipe.craftButton = newRecipe.button.transform.GetChild(0).gameObject;
            newRecipe.potionButton = newRecipe.button.transform.GetChild(1).gameObject;
            newRecipe.buttonText.text = newRecipe.potionData.itemName;
            newRecipe.potionAmount = newRecipe.potionButton.GetComponentInChildren<Text>();
            newRecipe.potionImage = newRecipe.potionButton.transform.GetChild(0).GetComponent<Image>();
            newRecipe.potionImage.sprite = newRecipe.potionData.itemIcon;

            int nrIngredientObjects = newRecipe.button.transform.childCount - 3;
            GameObject ingredientObject = newRecipe.craftButton.transform.GetChild(0).gameObject;
            List<GameObject> ingCollect = new List<GameObject>();

            //Debug.LogFormat("Number of ingredients: {0}", newRecipe.ingredientsData.Length);
            newRecipe.ingredientImages[0] = ingredientObject.GetComponent<Image>();
            newRecipe.ingredientImages[0].sprite = newRecipe.ingredientsData[0].ingredient.itemIcon;
            //Debug.LogFormat("Text object name: {0}", ingredientObject.GetComponentInChildren<Text>().name);
            newRecipe.ingredientAmount[0] = ingredientObject.GetComponentInChildren<Text>();
            ingCollect.Add(ingredientObject);

            if (nrIngredients > nrIngredientObjects)
            {
                for (int ii = nrIngredientObjects; ii < nrIngredients; ii++)
                {
                    Debug.LogFormat("Setup ingredient nr {0} in recipe {1}", ii, i);
                    GameObject newIngObj = Instantiate(ingredientObject, newRecipe.craftButton.transform);
                    newIngObj.transform.localPosition -= Vector3.right * 80 * ii;
                    newRecipe.ingredientImages[ii] = newIngObj.GetComponent<Image>();
                    newRecipe.ingredientImages[ii].sprite = newRecipe.ingredientsData[ii].ingredient.itemIcon;

                    newRecipe.ingredientAmount[ii] = newIngObj.GetComponentInChildren<Text>();

                    ingCollect.Add(ingredientObject);
                }
            }

            _recipieList.Add(newRecipe);
            //int index = i;
            newRecipe.craftButton.GetComponent<Button>().onClick.AddListener(delegate { CraftPotion(newRecipe); }); //_recipieList[index]
            newRecipe.potionButton.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    newRecipe.potionScript.ActivatePotion();
                    UpdateUI();
                    UpdatePotionAvailability(newRecipe);
                });
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
            for (int ii = 0; ii < _recipieList[i].ingredientAmount.Length; ii++)
            {
                try
                {
                    _recipieList[i].ingredientAmount[ii].text = string.Format("{0}/{1}",
                        FlowerLibrary.GetFlowerAmount(_recipieList[i].ingredientsData[ii].ingredient.itemName), _recipieList[i].ingredientsData[ii].amount);
                }
                catch (System.NullReferenceException e)
                {
                    Debug.LogErrorFormat("Encountered NullReference in the AlchemyOrganizerV2 during UpdateUI. This was encountered on the {0}th recipe at the {1}th ingredient", i, ii);
                }
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
        //Debug.LogFormat("Current Recipe State: {0}", recipe.state);

        if (recipe.state == RecipeState.Unknown)
        {
            Debug.Log("Yes");
            recipe.button.SetActive(false);
        }

        // Unavailable Removed
        //else if (recipe.state == RecipeState.Unavailable)
        //{
        //	recipe.button.SetActive(true);
        //	recipe.craftButton.GetComponent<Button>().enabled = false;
        //	recipe.craftButton.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f, 0.5f);

        //	for (int i = 0; i < recipe.ingredientsData.Length; i++)
        //	{
        //		if(_discoveredFlowers.Contains(recipe.ingredientsData[i].ingredient))
        //		{
        //			recipe.ingredientImages[i].sprite = recipe.ingredientsData[i].ingredient.itemIcon;
        //		}
        //		else
        //		{
        //			recipe.ingredientImages[i].sprite = _unknownIngredient;
        //		}
        //	}
        //}
        else
        {
            recipe.button.SetActive(true);
            recipe.craftButton.GetComponent<Button>().enabled = true;
            //recipe.craftButton.GetComponent<Image>().color = new Color(0.4f, 0.75f, 0.25f, 0.5f);

            //for (int i = 0; i < recipe.ingredientsData.Length; i++)
            //{
            //	if (_discoveredFlowers.Contains(recipe.ingredientsData[i].ingredient))
            //	{
            //		recipe.ingredientImages[i].sprite = recipe.ingredientsData[i].ingredient.itemIcon;
            //	}
            //	else
            //	{
            //		recipe.ingredientImages[i].sprite = _unknownIngredient;
            //	}
            //}
        }

        UpdatePotionAvailability(recipe);
    }

    void UpdatePotionAvailability(Recipe recipe)
    {
        if (FlowerLibrary.GetPotionAmount(recipe.potionData.itemName) > 0)
        {
            //recipe.potionButton.GetComponent<Image>().color = new Color(0.4f, 0.75f, 0.25f, 0.5f);
            recipe.potionButton.GetComponent<Button>().enabled = true;
        }
        else
        {
            //recipe.potionButton.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f, 0.5f);
            recipe.potionButton.GetComponent<Button>().enabled = false;
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
        for (int i = 0; i < potionRecipe.ingredientsData.Length; i++)
        {
            ItemDataContainer comparableItem;
            if (potionRecipe.ingredientsData[i].amount <= FlowerLibrary.GetFlowerAmount(potionRecipe.ingredientsData[i].ingredient.itemName))
            {

            }
            else if (FindSameSymbol(potionRecipe.ingredientsData[i].ingredient, potionRecipe.ingredientsData[i].amount, out comparableItem))
            {
                potionRecipe.ingredientsData[i].ingredient = comparableItem;
                potionRecipe.ingredientImages[i].sprite = comparableItem.itemIcon;
            }
            else
            {
                Debug.LogFormat("Not enough of ingredient {0}", potionRecipe.ingredientsData[i].ingredient.itemName);
                return false;
            }
        }

        potionRecipe.potionScript.AddPotion(potionRecipe.ingredientsData);

        UpdatePotionAvailability(potionRecipe);
        UpdateUI();
        return true;
    }

    bool FindSameSymbol(ItemDataContainer data, int amount, out ItemDataContainer hasSame)
    {
        Debug.Log("Test");
        for (int i = 0; i < _allIngredients.Count; i++)
        {
            if (data.symbol == null || _allIngredients[i] == null)
            {
                Debug.Log("Null Fail");
                hasSame = null;
            }
            else if (object.ReferenceEquals(_allIngredients[i].symbol, data.symbol) && FlowerLibrary.GetFlowerAmount(_allIngredients[i].itemName) > amount)
            {
                Debug.Log("Not Null Fail");
                hasSame = _allIngredients[i];
                return true;
            }
        }

        Debug.Log("test");
        hasSame = null;
        return false;
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
        for (int i = 0; i < _recipieList.Count; i++)
        {
            UpdateRecipeToState(_recipieList[i], RecipeState.Available);

        }
        for (int i = 0; i < _allIngredients.Count; i++)
        {
            if (!_discoveredFlowers.Contains(_allIngredients[i]))
            {
                _discoveredFlowers.Add(_allIngredients[i]);
            }
            Debug.LogFormat("Adding flower {0}", _allIngredients[i]);
            FlowerLibrary.IncrementFlower(_allIngredients[i].itemName, 50);
        }

        //Debug.LogFormat("FLÖWERS: {0}", allFlowers.Count);

        //UpdateIngredients();
        UpdateUI();
    }
}
