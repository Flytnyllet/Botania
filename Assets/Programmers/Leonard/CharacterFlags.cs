using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CharacterFlags
{
	List<string> _flags = new List<string>();


	public void HasFlag(string effect)
	{
		_flags.Contains(effect);
	}
	public void AddFlag(string effect, float time)
	{
		Debug.Log("Added Flag: " + effect + " to player effects");
		_flags.Add(effect);
		Task.Run(async () =>
		{
			await Task.Delay(System.TimeSpan.FromSeconds(time));
			Debug.Log("Flag: " + effect + " effect has ended");
			RemoveFlag(effect);
		});
	}
	void RemoveFlag(string effect)
	{
		_flags.Remove(effect);
	}
	//public void Add
}
