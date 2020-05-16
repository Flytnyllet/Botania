using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionWheelManager : MonoBehaviour
{
	const string MENU_ACTIVATE_BUTTON = "Use Wheel";
	[SerializeField] BookManager _bookManager = null;
	[SerializeField] GameObject _wheelRegion = null;
	[SerializeField] [Range(3, 8)] int _nrOfRegions = 4;
	List<PotionLoader> _allPotions = null;
	[SerializeField] [Range(50f, 200f)] float regionDistanceFromCenter = 100f;
	int selectedRegion = -1;
	RectTransform rect = null;
	[SerializeField] float selectedScale = 1.2f;
	[SerializeField] [Range(0.05f, 0.8f)] float selectionDistance = 0.46f;
    Camera _mainCam;
	//Color selectedColor = Color.green;
	Vector3[] regionPositions = new Vector3[5] 
		{ Vector3.up, new Vector3(1, 1, 0).normalized, Vector3.right, new Vector3(1, -1, 0).normalized, Vector3.down }; 


	// Start is called before the first frame update
	void Start()
	{
        _mainCam = Camera.main; //slow
        rect = GetComponent<RectTransform>();
		_allPotions = _bookManager.GetBookmark(1).GetComponent<AlchemyOrganizer>().GetAllPotions();
		SetUpRegions();
	}

	// Update is called once per frame
	void Update()
	{
		HandleInputs();

		//var screenPoint = Vector3.zero + Input.mousePosition;
		//screenPoint = (Camera.main.ScreenToViewportPoint(screenPoint) - new Vector3(0.5f, 0.5f)) * 2f;
		//screenPoint = Vector3.right * screenPoint.x * rect.rect.width + Vector3.forward * screenPoint.y * rect.rect.height + Vector3.forward*10f;
		Vector2 screenPoint = (Input.mousePosition / _mainCam.pixelHeight) * 2 - new Vector3(1f * _mainCam.aspect, 1f);//Camera.main.ScreenToViewportPoint(Input.mousePosition) *2 - new Vector3(1f, 1f);
        //Debug.Log(screenPoint.normalized);
		//Debug.Log("Mouse to close to center at: " + screenPoint);

		if (screenPoint.magnitude > selectionDistance ) // && screenPoint.magnitude < selectionDistance*2)
		{
			//Debug.Log("screenPoint magnitude : " + screenPoint.magnitude);
			int? region;
			if (GetClosestPotion(screenPoint.normalized, out region))
			{
				SelectPotion((int)region);
			}
			else
			{
				//Debug.Log("Same Region Selected");
			}

			ActivateSelectedRegion(MENU_ACTIVATE_BUTTON);
		}
		else
		{
			SelectPotion(-1);
			//Debug.Log("Mouse too close to center: " + screenPoint + " " + Input.mousePosition + "  " + screenPoint.magnitude.ToString() + " \n mouse position is: " + Input.mousePosition);
		}
	}

	void UpdateUI()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			PotionLoader potion = _allPotions[i];
			string potionName = potion.GetPotionItemData().itemName;

			Transform child = transform.GetChild(i);
			child.GetComponentInChildren<Text>().text = potionName + "\n x" + FlowerLibrary.GetPotionAmount(potionName);

			//go.GetComponent<Button>().onClick.AddListener( delegate { ActivateSelectedRegion(MENU_ACTIVATE_BUTTON); });
		}
	}

	void SetUpRegions()
	{
		if(regionPositions.Length != _allPotions.Count)
		{
			Debug.LogWarning("Either all potions don't have regions or all regions don't have a potion in PotionWheelManager");
		}
		else
		{
			for(int i = 0; i < _allPotions.Count; i++)
			{
				PotionLoader potion = _allPotions[i];
				string potionName = potion.GetPotionItemData().itemName;
				//int ind = i;

				GameObject go = Instantiate(_wheelRegion, transform);
				go.transform.position = transform.position + regionPositions[i] * regionDistanceFromCenter;
				//GameObject image = Instantiate(_wheelRegion, go.transform);
				
				go.GetComponentInChildren<Text>().text = potionName + "\n x" + FlowerLibrary.GetPotionAmount(potionName);
				Image imageSprite = go.transform.GetChild(0).GetComponent<Image>();
				imageSprite.sprite = potion.GetPotionItemData().itemIcon;
				
				go.name = i.ToString() + "Potion ";
				//go.GetComponent<Button>().onClick.AddListener( delegate { ActivateSelectedRegion(MENU_ACTIVATE_BUTTON); });
			}
		}
	}

	/// <summary>
	/// Selects closest potion region gameObject based on the target position.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// the target position, should generally be the mouse.
	/// <returns></returns>
	bool GetClosestPotion(Vector2 targetPosition, out int? closest)
	{
		closest = null;
		float distance = 3000;
		for(int i = 0; i < transform.childCount; i++)
		{
			Vector2 childPosition = transform.GetChild(i).position / _mainCam.pixelHeight * 2f - new Vector3(1f * _mainCam.aspect, 1f); //Camera.main.ScreenToViewportPoint(transform.GetChild(i).position) * 2f  - new Vector3(1f, 1f);
			//(Camera.main.ToViewportPoint(transform.GetChild(i).position) - new Vector3(0.5f, 0.5f)) * 2f;
			//Debug.Log("During closest potion calculation the child position is: " + childPosition);
			float newDistance = Vector3.Distance(targetPosition, childPosition);
			if (newDistance < distance)
			{
				distance = newDistance;
				closest = i;
				//Debug.Log("New closest potion: " + closest + " at a distance of " + distance);
			}
		}
		
		bool changedValue = selectedRegion != closest;

		return changedValue;
	}

	void SelectPotion(int potionIndex)
	{
		if (selectedRegion != -1)
		{
			//PotionLoader oldPotion = _allPotions[selectedRegion];
			transform.GetChild(selectedRegion).localScale /= selectedScale;
		}
		//PotionLoader newPotion = _allPotions[potionIndex];
		selectedRegion = potionIndex;

		if (selectedRegion != -1)
		{
			transform.GetChild(selectedRegion).localScale *= selectedScale;
		}
	}

	void ActivateSelectedRegion(string button)
	{
		if (Input.GetButtonDown(button))
		{
			Debug.LogWarning("Region Activated");
			_allPotions[selectedRegion].ActivatePotion();
		}
	}

	void HandleInputs()
	{
		if (Input.anyKeyDown)
		{
			string frameKeys = Input.inputString;
			
			for (int i = 0; i < frameKeys.Length; i++)
			{
				switch (frameKeys.ToCharArray()[i])
				{
					case '1':
						_allPotions[0].ActivatePotion();
						break;
					case '2':
						_allPotions[1].ActivatePotion();
						break;
					case '3':
						_allPotions[2].ActivatePotion();
						break;
					case '4':
						_allPotions[3].ActivatePotion();
						break;
					case '5':
						_allPotions[4].ActivatePotion();
						break;
				}
			}

		}
	}
}
