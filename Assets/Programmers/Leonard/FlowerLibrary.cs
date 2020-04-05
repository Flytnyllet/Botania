using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlowerLibrary 
{
	static List<Flower> _flowerTypes = new List<Flower>();
	static bool _libraryInitiated = false;

	public static void InitiateLibrary()
	{
		int[] baseProgression = new int[3] { 1, 3, 5 };
		string[] tulipProgress = new string[3] {
			"Tulips are a thing.",
			"There are a lot of different tulips, but here they are all orange!",
			"Please eat tulips during your free time!" };
		Flower flower0 = new Flower(0, "Tulip", baseProgression, tulipProgress);
		_flowerTypes.Add(flower0);
		string[] poppyProgress = new string[3] {
			"It's a very nice flower.",
			"This belongs in a museum!",
			"You can make opium from this >->" };
		Flower flower1 = new Flower(1, "Poppy", baseProgression, poppyProgress);
		_flowerTypes.Add(flower1);
		string[] dandelionProgress = new string[3] {
			"This stuff grows everywhere!",
			"Soon it will be able to grow on animals.",
			"Sooooon it will be able to grow on YOU..." };
		Flower flower2 = new Flower(2, "Dandelion", baseProgression, dandelionProgress);
		_flowerTypes.Add(flower2);

		_libraryInitiated = true;
	}

	public static Flower GetFlowerType(int id)
	{
		return _flowerTypes[id];
	}
	public static List<string> GetAllFlowerNames()
	{
		List<string> flowerNames = new List<string>();
		foreach(Flower flower in _flowerTypes)
		{
			flowerNames.Add(flower.Name);
		}
		return flowerNames;
	}
	/*
	public static List<Flower> GetAllFlowers()
	{
		return _flowerTypes;
	}
	*/
	public static void IncrementFlower(int id)
	{
		Flower flower = _flowerTypes[id];
		Debug.Log("Added Flower: " + flower.Name);
		flower.Amount++;
		if(flower.Amount >= flower.ProgressionCurve[flower.UnlockProgress])
		{
			flower.UnlockProgress++;
		}

		//Interactor.AddLogEntry("Added " + flower.Name);
	}
}
