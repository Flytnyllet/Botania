using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoader : MonoBehaviour
{
	[SerializeField] string _flowerName = null;
	[SerializeField] Text textObject = null;
	[SerializeField] Image imageObject = null;
	int itemAmount = 0;
	[SerializeField] ItemDataContainer _item = null;
	//Flower _flower;

	private void Awake()
	{
		_flowerName = _item.itemName;
	}
	
    void Start()
    {
		imageObject.sprite = _item.itemIcon;
		//itemAmount = FlowerLibrary.GetFlowerAmount(_flowerName);
		//textObject.text = _flowerName + "\n x" + itemAmount;
	}

	void OnEnable()
	{
		//if(_flower != null)
		UpdateUI();
	}

	public string GetFlowerName()
	{
		return _flowerName;
	}
	public ItemDataContainer GetItemData()
	{
		return _item;
	}

	public void UpdateUI()
	{
		itemAmount = FlowerLibrary.GetFlowerAmount(_flowerName);
		textObject.text = _flowerName + "\n x" + itemAmount;
	}
}
