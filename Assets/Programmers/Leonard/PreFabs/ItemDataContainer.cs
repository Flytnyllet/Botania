using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Flower Data Container")]
public class ItemDataContainer : ScriptableObject
{
	public string itemName;
	public Sprite itemIcon;
	public Sprite symbol;
}
