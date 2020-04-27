using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionLoader : MonoBehaviour
{
    [SerializeField] string _potionName = "";
    [SerializeField] Text textObject;
    [SerializeField] List<string> recipe = new List<string>();
    Potion _potion;

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

        CreateThisPotion();
    }

    Potion_Template PotionEffect()
    {
        switch (_potionType)
        {
            case PotionType.Speed:
                //intparam = targetDistortion,  floatParam = distort time to lerp
                EventParameter param = new EventParameter() { intParam = -40, floatParam = 2 };
                EventManager.TriggerEvent(CameraEffect.CAMERA_SPEED_DISTORT, param);
                param.intParam = 0;
                ActionDelayer.RunAfterDelay(() => { EventManager.TriggerEvent(CameraEffect.CAMERA_SPEED_DISTORT, param); }, _potionDuration);

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
        textObject.text = _potionName + "\n x" + FlowerLibrary.GetPotionAmount(_potionName);
    }
    void OnEnable()
    {
        if (_potion != null)
        {
            UpdateUI();
        }
    }
    public List<string> GetRecipe()
    {
        return recipe;
    }
    void CreateThisPotion()
    {
        string[] recipe0 = new string[2] { "Tulip", "Tulip" };
        Potion potion0 = new Potion(0, "Speed", 0, recipe0);
    }
    public void AddPotion()
    {
        FlowerLibrary.IncrementPotion(_potionName, 1);
        Debug.Log("Adding one more " + _potionName + ", now there are " + FlowerLibrary.GetPotionAmount(_potionName));

        for (int i = 0; i < recipe.Count; i++)
        {
            FlowerLibrary.IncrementFlower(recipe[i], -1);
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
            PotionEffect().PotionEffectStart(FPSMovement.playerMovement);
            FlowerLibrary.IncrementPotion(_potionName, -1);
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough of " + _potionName);
        }
    }
}
