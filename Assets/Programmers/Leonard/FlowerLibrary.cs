using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlowerLibrary
{
	[System.Serializable]
	struct FlowerEntry
	{
		public string key;
		public FlowerData data;
	}
	[System.Serializable]
	struct PotionEntry
	{
		public string key;
		public int amount;
	}
	const string FILE_NAME_FLOWERS = "Flowers";
	const string FILE_NAME_POTIONS = "Potions";
	[System.Serializable]
    public class FlowerData
    {
        public FlowerData(int startAmount)
        {
            Amount = startAmount;
            Discovered = startAmount;
        }
        //Lämnar dessa som public variabler for now, då förlänging av klassen bara gör saker svårläst (be free to change) //Jens
        public int Amount;
        public int Discovered;
    }

	public static void Wipe()
	{
		_flowerTypes.Clear();
		_potionTypes.Clear();
	}

	public static void Save()
	{
		List<FlowerEntry> flowerEntries = new List<FlowerEntry>();
		foreach(string key in _flowerTypes.Keys)
		{
			FlowerEntry flowerEntry;
			flowerEntry.key = key;
			flowerEntry.data = _flowerTypes[key];
			flowerEntries.Add(flowerEntry);
		}
		List<PotionEntry> potionEntries = new List<PotionEntry>();
		foreach (string key in _potionTypes.Keys)
		{
			PotionEntry potionEntry;
			potionEntry.key = key;
			potionEntry.amount = _potionTypes[key];
			potionEntries.Add(potionEntry);
		}
		Serialization.Save(FILE_NAME_FLOWERS, flowerEntries); 
		Serialization.Save(FILE_NAME_POTIONS, potionEntries); 
	}

	public static void Load()
	{
		Dictionary<string, FlowerData> flowerTypes = new Dictionary<string, FlowerData>();
		object temp = Serialization.Load(FILE_NAME_FLOWERS);
		List<FlowerEntry> flowerEntries = null;
		if (temp != null)
			flowerEntries = (List<FlowerEntry>)temp;
		else
			Debug.LogErrorFormat("Load Data Is Null");
		
		for (int i  = 0; i < flowerEntries.Count; i++)
		{
			flowerTypes.Add(flowerEntries[i].key, flowerEntries[i].data);
		}
		_flowerTypes = flowerTypes;

		Debug.LogFormat("Number of flower types in Load is {0}", flowerEntries.Count.ToString());

		Dictionary<string, int> potionTypes = new Dictionary<string, int>();
		List<PotionEntry> potionEntries = (List<PotionEntry>)Serialization.Load(FILE_NAME_POTIONS);
		for (int i = 0; i < potionEntries.Count; i++)
		{
			potionTypes.Add(potionEntries[i].key, potionEntries[i].amount);
		}
		_potionTypes = potionTypes;

		Debug.LogFormat("Number of potion types in Load is {0}", potionTypes.Count.ToString());
	}

    const string SAVE_FILE_NAME = "FlowerLibrary";
    //static List<Flower> _flowerTypes = new List<Flower>();
    static Dictionary<string, FlowerData> _flowerTypes = new Dictionary<string, FlowerData>();
    static Dictionary<string, int> _potionTypes = new Dictionary<string, int>();
	const int _maxItems = 99;

    // Initialization
    static FlowerLibrary()
    {
        //_flowerTypes = (List<Flower>)Serialization.Load(SAVE_FILE_NAME);
        ////FlowerLibrarySave save =(FlowerLibrarySave)Serialization.Load(SAVE_FILE_NAME);
        //if(_flowerTypes == null)
        //{
        //    _flowerTypes = new List<Flower>();
        //}
        //else
        //{
        //    _flowerTypes = save.flowerTypes;
        //}
        /*
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
		string[] violet = new string[3] {
			"Violet is my favorite color!",
			"Please don't remove my messages.",
			"This book protects me, I should be safe here." };
		Flower flower3 = new Flower(3, "Violet", baseProgression, violet);
		_flowerTypes.Add(flower3);

		*/
        // Initialize Potions
        /*
		Flower[] recipe0 = new Flower[2] { flower0, flower1 };
		Potion potion0 = new Potion(0, "Speed", 0, recipe0);
		_potionTypes.Add(potion0);
		Flower[] recipe1 = new Flower[2] { flower1, flower2 };
		Potion potion1 = new Potion(1, "Jump", 1, recipe1);
		_potionTypes.Add(potion1);
		Flower[] recipe2 = new Flower[3] { flower0, flower1, flower2 };
		Potion potion2 = new Potion(2, "Glide", 2, recipe2);
		_potionTypes.Add(potion2);
		*/
    }

    static void AddFlower(string flower, int amount)
    {
        _flowerTypes.Add(flower, new FlowerData(amount));
    }

    static void AddPotion(string potion, int amount)
    {
        _potionTypes.Add(potion, amount);
    }

    /*public static Flower GetFlowerType(int id)
	{
		return _flowerTypes[id];
	}*/
    public static int GetFlowerAmount(string flowerName)
    {
        FlowerData flower;
        if (_flowerTypes.TryGetValue(flowerName, out flower))
        {
            return flower.Amount;
        }
        else
            return 0;
    }
    public static int GetFlowerDiscoverAmount(string flowerName)
    {
        FlowerData flower;
        if (_flowerTypes.TryGetValue(flowerName, out flower))
        {
            return flower.Discovered;
        }
        else
            return 0;
    }

    public static int GetPotionAmount(string potionName)
    {
        if (_potionTypes.ContainsKey(potionName))
            return _potionTypes[potionName];
        else
            return 0;
    }
    public static string[] GetAllFlowerNames()
    {
        string[] flowerNames = new string[_flowerTypes.Count];
        _flowerTypes.Keys.CopyTo(flowerNames, 0);
        /*for(int i =  0; i < _flowerTypes.Count; i++)
		{
			//Flower flower = _flowerTypes[i];
			flowerNames.Add(_flowerTypes.Keys.);
		}*/
        return flowerNames;
    }
    /*
	public static List<Flower> GetAllFlowers()
	{
		return _flowerTypes;
	}
	*/
    public static void IncrementFlower(string flowerName, int amount)
    {
        if (_flowerTypes.ContainsKey(flowerName))
        {
			if(amount + _flowerTypes[flowerName].Amount > _maxItems)
			{
				_flowerTypes[flowerName].Amount = _maxItems;
				_flowerTypes[flowerName].Discovered += amount;
			}
			else
			{
				_flowerTypes[flowerName].Amount += amount;
				_flowerTypes[flowerName].Discovered += amount;
			}
        }
        else AddFlower(flowerName, amount);

        //Debug.Log($"{flowerName} amount = {_flowerTypes[flowerName].Amount}");

        /*Flower flower = _flowerTypes[flowerName];
		Debug.Log("Added Flower: " + flower.Name);
		flower.Amount++;
		if(flower.UnlockProgress < flower.ProgressionCurve.Length)
		if(flower.Amount >= flower.ProgressionCurve[flower.UnlockProgress])
		{
			flower.UnlockProgress++;
		}*/
        //FlowerLibrarySave save = new FlowerLibrarySave() { flowerTypes = _flowerTypes };
        //Serialization.Save(SAVE_FILE_NAME, save);

        //Interactor.AddLogEntry("Added " + flower.Name);
    }

    public static void IncrementPotion(string name, int amount)
    {
		if (_potionTypes.ContainsKey(name))
		{
			if (_potionTypes[name] + amount > _maxItems)
			{
				_potionTypes[name] = _maxItems;
			}
			else
			{
				_potionTypes[name] += amount;
			}
		}

		else AddPotion(name, amount);
        Debug.Log("Added Potion: " + name);

        //FlowerLibrarySave save = new FlowerLibrarySave() { flowerTypes = _flowerTypes };
        //Serialization.Save(SAVE_FILE_NAME, save);

        //Interactor.AddLogEntry("Added " + flower.Name);
    }
}
