using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// !WARNING, DO NOT LET THESE OBJECTS BE DISABLED ON START UP! THEY ARE REQUIRED TO RUN AT LEAST THEIR STARTING METHODS AS TO ALLOW THE FLOWER SYSTEM TO FUNCTION
public class PageLoader : MonoBehaviour
{
    [System.Serializable]
    class PageElement
    {
        [SerializeField] UnityEngine.UI.MaskableGraphic _bookElement;
        [SerializeField] int _unlockPoint = 0;

        public UnityEngine.UI.MaskableGraphic GetBookElement { get => _bookElement; }
        public int GetUnlockPoint { get => _unlockPoint; }
    }

    //[SerializeField] List<Image> _imageObject = new List<Image>();
    //[SerializeField] List<Text> _textObject = new List<Text>();
    [Tooltip("Används för att bestämma när olika element i sidan skall visas. \n"
        +"Dra in text/image och sätt in hur många blommor som ska plockas upp innan elementer låses upp")]
    [SerializeField] List<PageElement> _pageElements = new List<PageElement>();
    [SerializeField] Text _amountCounter;
    //Flower _flower;

    //[Header("Flower Creation")]
    [SerializeField] string _flowerName = "Grass";
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
        _amountCounter.text = FlowerLibrary.GetFlowerAmount(_flowerName).ToString();
    }

    void OnEnable()
    {
        _amountCounter.text = FlowerLibrary.GetFlowerAmount(_flowerName).ToString();
        SetPageElementVisibility();
    }


    //Aktiverar de Objekt vars unlock conditions uppfylls. Just nu bara kravet att plocka ett visst antal blommor
    void SetPageElementVisibility()
    {
                                                //Is expanding the condition part of a loop heresy?
        for (int i = 0; i < _pageElements.Count && _pageElements[i].GetBookElement != null; i++)
        {
            if (FlowerLibrary.GetFlowerDiscoverAmount(_flowerName) >= _pageElements[i].GetUnlockPoint)
            {
                _pageElements[i].GetBookElement.gameObject.SetActive(true);
            }
            else
            {
                _pageElements[i].GetBookElement.gameObject.SetActive(false);
            }
        }
    }

    //public Flower CreateThisFlower()
    //{
    //    Flower flower = new Flower(_flowerName, _progressionPoints.ToArray());
    //    return flower;
    //}

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
