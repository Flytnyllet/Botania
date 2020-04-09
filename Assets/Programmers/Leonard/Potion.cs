using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
	int _id;
	int _amount;
	string _name;
	int _effectID;
	Flower[] _recipe;

	public Potion(int id, string name, int effectID, Flower[] recipe)
	{
		_id = id;
		_name = name;
		_effectID = effectID;
		_recipe = recipe;
	}

	public int Amount
	{
		get => _amount;
		set => _amount = value;
	}

	public string Name
	{
		get => _name;
	}
	public int Id
	{
		get => _id;
	}

	public Flower[] Recipe
	{
		get => _recipe;
	}
}
