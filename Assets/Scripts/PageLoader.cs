using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageLoader : MonoBehaviour
{
	[SerializeField] List<Image> _imageObject = new List<Image>();
	[SerializeField] List<Text> _textObject = new List<Text>();
	[SerializeField] Text _amountCounter;
	Flower _flower;
	[SerializeField] string _flowerLoadName = "NotAFlower";

	[SerializeField] bool _isFlowerPage = true;

	void Start()
	{
		if (_isFlowerPage)
		{
			List<string> flowerNames = FlowerLibrary.GetAllFlowerNames();
			Debug.Log("flower count: " + flowerNames.Count);

			Debug.Log("getting flowers");
			//if ("Page_" + flowerNames[i] == gameObject.name)
			if (flowerNames.Contains(_flowerLoadName))
			{
				Debug.Log("Flower got");
				int i = flowerNames.IndexOf(_flowerLoadName);
				_flower = FlowerLibrary.GetFlowerType(i);
			}
			else
			{
				Debug.LogError("Page load failure. Either the page name is incorrect or there is no flower by the name of " + _flowerLoadName);
			}

			int ind = _textObject.Count;
			Debug.Log("Number of text object: " + _textObject.Count);
			for (int i = 0; i < ind; i++)
			{
				Debug.Log("Added Text");
				_textObject[i].text = _flower.LoreProgression[i];
			}

			_amountCounter.text = _flower.Amount.ToString();
		}
	}

	void OnEnable()
	{
		if (_isFlowerPage && _flower != null)
		{
			_amountCounter.text =
				_flower.Amount.ToString();
		}
	}

	public void NextPage()
	{
		int i = transform.GetSiblingIndex();
		int nextI = (i + 2) % transform.parent.childCount;
		transform.parent.GetChild(nextI).gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	void Update()
	{

	}
}
