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

	void Awake()
	{
		CreateThisPotion();
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
	void CreateThisPotion ()
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
}
