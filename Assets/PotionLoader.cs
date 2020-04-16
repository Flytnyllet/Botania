using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionLoader : MonoBehaviour
{
	[SerializeField] string _potionName = "";
	[SerializeField] Text textObject;
	List<Flower> recipe = new List<Flower>();
	Potion _potion;

	void Awake()
	{
		CreateThisPotion();
	}

	void Start()
	{
		//_potion = FlowerLibrary.GetPotionType(_potionName);
		//recipe.AddRange(_potion.Recipe);
		textObject.text = _potionName + "\n x" + FlowerLibrary.GetPotionAmount(_potionName);
	}

	void OnEnable()
	{
		if (_potion != null)
		{
			textObject.text = _potion.Name + "\n x" + _potion.Amount;
		}
	}

	void CreateThisPotion ()
	{
		string[] recipe0 = new string[2] { "Tulip", "Tulip" };
		Potion potion0 = new Potion(0, "Speed", 0, recipe0);
	}

	public void ActivatePotion()
	{
        p = new TestPotion();
        p.SpeedPot(player);
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
		if (hasIngredients) FlowerLibrary.IncrementPotion(_potionName, 1);
		else Debug.Log("Not enough flowers");
	}
}
