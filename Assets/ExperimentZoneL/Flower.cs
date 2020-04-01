using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower
{
	int _id;
	string _name;
	int _amount = 0;
	int _unlockProgress = 0;
	int[] _progressionCurve;
	string[] _loreProgression;

	public Flower(int id, string name = "NoName", int[] progressionCurve = null, string[] loreProgression = null)
	{
		this._name = name;
		this._progressionCurve = progressionCurve;
	}

	public string Name
	{
		get { return _name; }
	}

	public int Amount
	{
		get { return _amount; }
		set { _amount = value; }
	}
	public int UnlockProgress
	{
		get { return _unlockProgress; }
		set { _unlockProgress = value; }
	}

	public int[] ProgressionCurve
	{
		get { return _progressionCurve; }
		//set { _progressionCurve = value; }
	}
}
