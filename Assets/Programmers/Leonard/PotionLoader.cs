using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionLoader : MonoBehaviour
{
    [SerializeField] string _potionName = "";
    [SerializeField] Text textObject;
    [SerializeField] List<ItemDataContainer> recipe = new List<ItemDataContainer>();
    Potion _potion;
    Potion_Template _potionEffect;
    [SerializeField] ItemDataContainer _item;
    [SerializeField] Image imageObject;

    //Potion_Template _modifiers;
    enum PotionType
    {
        Speed, Flag, Gravity, Jump
    }
    [SerializeField] PotionType _potionType;
    [SerializeField] float _potionDuration;
    [Tooltip("Only used for potions of type 'Flag'")]
    [SerializeField] string _potionFlag;
    [Tooltip("Only used for potions of type 'Speed'")]
    [SerializeField] float _potionFactor;
    [Tooltip("Only used for potions of type 'Speed'")]
    [SerializeField] float _potionFlat;

    void Awake()
    {
        _potionName = _item.itemName;
        _potionEffect = PotionEffect();
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
        textObject.text = _potionName + "\n x" + FlowerLibrary.GetPotionAmount(_potionName);
    }
    void OnEnable()
    {
        if (_potion != null)
        {
            UpdateUI();
        }
    }
    public List<ItemDataContainer> GetRecipe()
    {
        return recipe;
    }

    public ItemDataContainer GetPotionItemData()
    {
        return _item;
    }
    public void AddPotion()
    {
        FlowerLibrary.IncrementPotion(_potionName, 1);
        Debug.Log("Adding one more " + _potionName + ", now there are " + FlowerLibrary.GetPotionAmount(_potionName));

        for (int i = 0; i < recipe.Count; i++)
        {
            FlowerLibrary.IncrementFlower(recipe[i].itemName, -1);
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        textObject.text = _potionName + "\n x" + FlowerLibrary.GetPotionAmount(_potionName);
    }

    public void ActivatePotion()
    {
        if (FlowerLibrary.GetPotionAmount(_potionName) > 0)
        {
            if (_potionEffect.PotionEffectStart(FPSMovement.playerMovement))
            {
                EventManager.TriggerEvent(EventNameLibrary.DRINK_POTION, new EventParameter());
                FlowerLibrary.IncrementPotion(_potionName, -1);
                UpdateUI();
            }
            else
            {
                Debug.Log("Potion already active");
            }
        }
        else
        {
            Debug.Log("Not enough of " + _potionName);
        }
    }
}
