using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionWheelManager : MonoBehaviour
{
	[SerializeField] BookManager _bookManager = null;
	[SerializeField] GameObject _wheelRegion = null;
	[SerializeField] int _nrOfRegions = 4;
	List<PotionLoader> _allPotions = null;

	// Start is called before the first frame update
	void Start()
	{
		_allPotions = _bookManager.GetBookmark(1).GetComponent<AlchemyOrganizer>().GetAllPotions();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.anyKeyDown)
		{
			string frameKeys = Input.inputString;
			Debug.Log(frameKeys);

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
