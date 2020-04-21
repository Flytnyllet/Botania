using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// !WARNING, DO NOT LET THESE OBJECTS BE DISABLED ON START UP! THEY ARE REQUIRED TO RUN AT LEAST THEIR STARTING METHODS AS TO ALLOW THE FLOWER SYSTEM TO FUNCTION
public class FlowerPage : PageLoader
{

    //[SerializeField] List<Image> _imageObject = new List<Image>();
    //[SerializeField] List<Text> _textObject = new List<Text>();

    [SerializeField] Text _amountCounter;
    //Flower _flower;

    //[Header("Flower Creation")]
    //[SerializeField] List<int> _progressionPoints = new List<int>();
    //[TextArea] [SerializeField] List<string> _loreDescriptions = new List<string>();

    void Awake()
    {
        /*_flower = CreateThisFlower();
		FlowerLibrary.AddFlower(_flower);*/
    }

    void Start()
    {

        /*List<string> flowerNames = FlowerLibrary.GetAllFlowerNames();
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
		}*/

        /*
		int ind = _textObject.Count;
		Debug.Log("Number of text object: " + _textObject.Count);
		for (int i = 0; i < ind; i++)
		{
			Debug.Log("Added Text");
			_textObject[i].text = _flower.LoreProgression[i];
		}
		*/
    }

    new void OnEnable()
    {
        SetPageElementVisibility();
        _amountCounter.text = FlowerLibrary.GetFlowerAmount(_flowerName).ToString();

    }



    //public Flower CreateThisFlower()
    //{
    //    Flower flower = new Flower(_flowerName, _progressionPoints.ToArray());
    //    return flower;
    //}

}
