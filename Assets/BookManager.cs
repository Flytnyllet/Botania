using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
	const string INPUT_INVENTORY = "Inventory";
	[SerializeField] List<GameObject> _bookmarks = new List<GameObject>();
	[SerializeField] List<GameObject> _lorePages = new List<GameObject>();
	[SerializeField] List<GameObject> _flowerPages = new List<GameObject>();
	[SerializeField] GameObject _bookmarkTemplate;
	[SerializeField] Vector2[] _bookmarkPositions;
	int _currentBookmark = 0;
	int _currentPage = 0;

    void Start()
    {
        
    }

	void Update()
	{
		if (Input.GetButtonDown(INPUT_INVENTORY))
		{
			gameObject.SetActive(!gameObject.activeSelf);
		}
	}

	void OnEnable()
	{
		CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
	}
	void OnDisable()
	{
		CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
	}

	void SetupPage(int pageParentID, List<GameObject> pages)
	{
		for(int i = 0; i<pages.Count; i++)
		{
			GameObject page = Instantiate<GameObject>(pages[i], _bookmarks[pageParentID].transform);
			if(i == _currentPage || i == _currentPage+1)
			{

			}
			else
			{
				page.SetActive(false);
			}
		}

	}
	void SetupBookmark()
	{
		for(int i = 0; i < _bookmarks.Count; i++)
		{
			GameObject bookmark = _bookmarks[i];
			Instantiate<GameObject>(bookmark, transform);
			GameObject bookmarkObject = Instantiate<GameObject>(_bookmarkTemplate);
			bookmarkObject.transform.position = new Vector3(_bookmarkPositions[i].x, _bookmarkPositions[i].y, 0.0f);
		}
	}
	int NextPage(int pageCount)
	{
		if ((pageCount % 2) == 0) return (_currentPage + 2) % pageCount;
		else return (_currentPage + 2) % (pageCount + 1);
	}
	void ToBookmark()
	{

	}
}
