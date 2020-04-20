using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBook : MonoBehaviour
{
    //
    //Quick and Dirty thing to open the book, this should probably be replaced entierly.
    //
    const string BOOK_OBJECT_NAME = "Book";
	const string INPUT_INVENTORY = "Inventory";

	[SerializeField] GameObject _bookObject;
	[SerializeField] List<PageLoader> pages = new List<PageLoader>();
    [SerializeField] bool _startLoaded = false;
    private void Awake()
    {
		//foreach (PageLoader page in pages)
		//{
		//	Flower flower = page.CreateThisFlower();
		//	FlowerLibrary.AddFlower(flower.Name, 0);
		//}

        if (_bookObject == null)
        {
            _bookObject = transform.Find(BOOK_OBJECT_NAME).gameObject; //slow
        }
        _bookObject.SetActive(_startLoaded);

    }

    void Update()
    {
		if (Input.GetButtonDown(INPUT_INVENTORY))
		{
            if (!_bookObject.activeSelf)
            {
                _bookObject.SetActive(true);
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
            }
            else
            {
                _bookObject.SetActive(false);
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
            }
		}

	}
}
