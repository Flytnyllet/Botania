using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlowerLibrary 
{
	static List<Flower> _flowerTypes = new List<Flower>();
	static bool _libraryInitiated = false;

	public static void InitiateLibrary()
	{
		int[] baseProgression = new int[4] { 1, 3, 5, 15 };
		Flower flower0 = new Flower(0, "Tulip", baseProgression);
		_flowerTypes.Add(flower0);
		Flower flower1 = new Flower(1, "Poppy", baseProgression);
		_flowerTypes.Add(flower1);
		Flower flower2 = new Flower(2, "Dandelion", baseProgression);
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

	public static void IncrementFlower(int id)
	{
		Flower flower = _flowerTypes[id];
		Debug.Log("Added Flower: " + flower.Name);
		flower.Amount++;
		if(flower.Amount >= flower.ProgressionCurve[flower.UnlockProgress])
		{
			flower.UnlockProgress++;
		}
	}
}
