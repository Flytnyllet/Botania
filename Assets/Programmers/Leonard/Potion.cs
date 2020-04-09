using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
	int _id;
	int _amount;
	string _name;
	int _effectID;

	public Potion(int id, string name, int effectID)
	{
		_id = id;
		_name = name;
		_effectID = effectID;
	}

	public int _Amount
	{
		get => _amount;
		set => _amount = value;
	}

}
