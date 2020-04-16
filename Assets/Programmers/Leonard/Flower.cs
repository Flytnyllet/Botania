using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Flower
{
	//int _id;
	string _name;
	int _amount = 0;
	int _unlockProgress = 0;
	int[] _progressionCurve;
	//string[] _loreProgression;

	public Flower(string name = "NoName", int[] progressionCurve = null
		//, string[] loreProgression = null
		)
	{
		this._name = name;
		this._progressionCurve = progressionCurve;
		//this._loreProgression = loreProgression;
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
	/*public string[] LoreProgression
	{
		get { return _loreProgression;  }
	}*/
}
