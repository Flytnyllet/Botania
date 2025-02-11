﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct RecipeEntry
{
    public ItemDataContainer ingredient;
    public int amount;
}

public class PotionLoader : MonoBehaviour
{
    //[SerializeField] string _potionName = "";
    [SerializeField] Text textObject = null;
    List<RecipeEntry> recipe = new List<RecipeEntry>();
    [SerializeField] List<ItemDataContainer> recipeIngredients = new List<ItemDataContainer>();
    [SerializeField] List<int> recipeAmounts = new List<int>();
    //Potion _potion = null;
    Potion_Template _potionEffect = null;
    Potion_Template _potionEffect2 = null;
    [SerializeField] ItemDataContainer _item = null;
    [SerializeField] Image imageObject = null;

    //Potion_Template _modifiers;
    enum PotionType
    {
        Speed, Flag, Gravity, Jump, FlagGrav
    }
    [SerializeField] PotionType _potionType = PotionType.Speed;
    [SerializeField] Material _potionCameraEffect;
    [SerializeField] float _potionCameraEffectFadeTime;
    [SerializeField] float _potionDuration = 0;
    [Tooltip("Only used for potions of type 'Flag'")]
    [SerializeField] string _potionFlag = null;
    [Tooltip("Only used for potions of type 'Speed'")]
    [SerializeField] float _potionFactor = 0;
    [Tooltip("Only used for potions of type 'Speed'")]
    [SerializeField] float _potionFlat = 0;

    void Awake()
    {
        SetUpRecipies();

        //_potionName = _item.itemName;

    }

    void SetUpRecipies()
    {
        if (recipe.Count < 1)
        {
            if (recipeAmounts.Count == recipeIngredients.Count)
            {
                for (int i = 0; i < recipeIngredients.Count; i++)
                {
                    
                    RecipeEntry entry;
                    entry.amount = recipeAmounts[i];
                    entry.ingredient = recipeIngredients[i];
                    recipe.Add(entry);
                }
            }
            else
            {
                Debug.Log("In all potions recipe ingredients and recipe amounts must be the same length!");
            }
        }

        if (_potionType != PotionType.FlagGrav)
        {
            _potionEffect = PotionEffect();
        }
        else
        {
            _potionEffect = new SpeedPotion(CharacterStatType.Gravity, _potionFactor, _potionFlat, _potionDuration);
            _potionEffect2 = new FlagPotion(_potionFlag, _potionDuration);
        }
    }

    Potion_Template PotionEffect()
    {
        switch (_potionType)
        {
            case PotionType.Speed:
                return new SpeedPotion(_potionFactor, _potionFlat, _potionDuration);

            case PotionType.Flag:
                return new FlagPotion(_potionFlag, _potionDuration);

            case PotionType.Jump:
                return new SpeedPotion(CharacterStatType.Jump, _potionFactor, _potionFlat, _potionDuration);

            case PotionType.Gravity:
                return new SpeedPotion(CharacterStatType.Gravity, _potionFactor, _potionFlat, _potionDuration);

            default:
                Debug.LogError("No potion type assigned in the potion loader of " + gameObject.name);
                return null;
        }
    }

    void Start()
    {
        imageObject.sprite = _item.itemIcon;
        textObject.text = _item.itemName + "\n x" + FlowerLibrary.GetPotionAmount(_item.itemName);
    }
    void OnEnable()
    {
        UpdateUI();

    }

    public List<ItemDataContainer> GetRecipeIngredients()
    {
        return recipeIngredients;
        /*List<ItemDataContainer> recipeIngredients = new List<ItemDataContainer>();

		for(int i = 0; i < recipe.Count; i++)
		{
			recipeIngredients.Add(recipe[i].ingredient);
		}

		return recipeIngredients;*/
    }

    public List<RecipeEntry> GetRecipe()
    {
        SetUpRecipies();
        return recipe;
    }

    public ItemDataContainer GetPotionItemData()
    {
        return _item;
    }
    public void AddPotion()
    {

        FlowerLibrary.IncrementPotion(_item.itemName, 1);
        Debug.Log("Adding one more " + _item.itemName + ", now there are " + FlowerLibrary.GetPotionAmount(_item.itemName));

        for (int i = 0; i < recipe.Count; i++)
        {
            FlowerLibrary.IncrementFlower(recipe[i].ingredient.itemName, -recipe[i].amount);
        }
        UpdateUI();
    }
    public void AddPotion(RecipeEntry[] changeRecipe)
    {
        recipe = new List<RecipeEntry>(changeRecipe);
        AddPotion();
    }

    void UpdateUI()
    {
        if (textObject != null)
        {
            textObject.text = _item.itemName + "\n x" + FlowerLibrary.GetPotionAmount(_item.itemName);
        }
    }

    public void ActivatePotion()
    {
        if (FlowerLibrary.GetPotionAmount(_item.itemName) > 0)
        {
            if (_potionEffect.PotionEffectStart(FPSMovement.playerMovement))
            {
                EventManager.TriggerEvent(EventNameLibrary.DRINK_POTION, new EventParameter()
                {
                    boolParam = false,
                    materialParam = _potionCameraEffect,
                    floatParam = _potionDuration,
                    floatParam2 = _potionCameraEffectFadeTime
                });
                FlowerLibrary.IncrementPotion(_item.itemName, -1);
                UpdateUI();
            }
            else
            {
                Debug.Log("Potion already active");
            }
        }
        else
        {
            Debug.Log("Not enough of " + _item.itemName);
        }
    }
}