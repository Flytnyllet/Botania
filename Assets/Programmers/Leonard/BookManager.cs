using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public static BookManager Instance;

    const string INPUT_INVENTORY = "Inventory";
    const string INPUT_MAP = "Map";
    const string INPUT_FLOWERS = "Flowers";
    const string INPUT_LORE = "Lore";
    const string INPUT_ALCHEMY = "Alchemy";
    const string OPEN_WHEEL = "Wheel";
    [SerializeField] Sprite[] _BookSprites = null;

    [SerializeField] List<GameObject> _bookmarks = new List<GameObject>();
    [SerializeField] List<PageLoader> _flowerPages = new List<PageLoader>();
    [SerializeField] List<PageLoader> FLOWERPAGE_INDEX = new List<PageLoader>();
    [SerializeField] List<PageLoader> _lorePages = new List<PageLoader>();
    [SerializeField] List<PageLoader> _indexPages = new List<PageLoader>();
    [SerializeField] int _flowerOrganizerId = 1;
    [SerializeField] RectTransform _bookmarkTemplate = null;
    [SerializeField] GameObject _emptyPageTemplate = null;
    [SerializeField] Vector2[] _bookmarkPositions = null;
    [SerializeField] Color[] _bookmarkColors = null;
    int _currentBookmark = 0;
    int _currentPage = 0;
    [SerializeField] public GameObject _book = null;
    [SerializeField] GameObject _map = null;
    [SerializeField] GameObject _potionWheel = null;

    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.DRINK_POTION, CloseBook);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.DRINK_POTION, CloseBook);
    }
    void CloseBook(EventParameter param = null)
    {
        _book.SetActive(false);
        MapGenerator.Display(false);
        EventManager.TriggerEvent(EventNameLibrary.CLOSE_BOOK, new EventParameter());
        CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
		StartCoroutine(TurnGOOff(_book));
		SetupBookmarks();
        foreach (PageLoader page in _flowerPages)
        {
            FLOWERPAGE_INDEX.Add(page);
        }
        SetupPage(_flowerOrganizerId, FLOWERPAGE_INDEX);

        /*
		if (_book == null)
		{
			_book = transform.Find("Book").gameObject; //slow
		}
		_book.SetActive(false); */
    }

    private void Start()
    {
        _bookmarks.Add(_map);
        SetupBookTabs();
    }

    void Update()
    {
        if (!WaypointMarker.InputFieldFocus())
        {
            InputHandling();
        }
        //if (Input.GetButtonDown(INPUT_LORE) && _book.activeSelf) { CloseBook(); }
    }

    void InputHandling()
    {
        if (OpenBookmark(_currentBookmark, INPUT_INVENTORY)) ;

        else if (OpenBookmark(4, INPUT_MAP)) ;

        else if (OpenBookmark(2, INPUT_LORE)) ;

        else if (OpenBookmark(1, INPUT_FLOWERS)) ;

        else if (OpenBookmark(3, INPUT_ALCHEMY)) ;

        else if (Input.GetButtonDown(OPEN_WHEEL) && !_book.activeSelf && CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
        {
            _potionWheel.SetActive(true);
            //_potionWheel.SetActive(!_potionWheel.activeSelf);
            //if(_potionWheel.activeSelf)
            //{
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
            //}
            //else
            //{
            //	CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
            //}
        }
        else if (Input.GetButtonUp(OPEN_WHEEL) && _potionWheel.activeSelf)
        {
            _potionWheel.SetActive(false);
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
        }
    }

    bool OpenBookmark(int index, string input)
    {
        if (Input.GetButtonDown(input) && !_potionWheel.activeSelf)
        {
            if (_book.activeSelf && _currentBookmark == index)
            {
                CloseBook(new EventParameter());
            }
            else if (_book.activeSelf)
            {
                ToBookmark(index, true);
            }
            else if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
            {
                OpenBook(index);
            }
            else { return false; }
            return true;
        }
        return false;
    }

    void OpenBook(int index)
    {
        if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
        {
            _book.SetActive(true);
            EventManager.TriggerEvent(EventNameLibrary.OPEN_BOOK, new EventParameter());
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
            ToBookmark(index);
        }
    }

    void SetupPage(int pageParentID, List<PageLoader> pages)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            GameObject page = Instantiate(pages[i].gameObject, _bookmarks[pageParentID].transform);

            if (i == _currentPage || i == _currentPage + 1)
            {
                page.SetActive(true);
            }
            else
            {
                page.SetActive(false);
            }
            pages[i] = page.GetComponent<PageLoader>();

            //Flower flower = pages[i].CreateThisFlower();
            //FlowerLibrary.AddFlower(flower.Name, 0);
        }
        if (pages.Count % 2 == 1)
        {
            GameObject emptyPage = Instantiate(_emptyPageTemplate, _bookmarks[pageParentID].transform);
            emptyPage.gameObject.SetActive(false);
            pages.Add(emptyPage.GetComponent<PageLoader>());
        }

    }
    void SetupBookmarks()
    {
        for (int i = _bookmarks.Count - 1; i >= 0; i--)
        {
            GameObject bookmark = _bookmarks[i];
            _bookmarks[i] = Instantiate(bookmark, _book.transform);
            _bookmarks[i].transform.SetAsFirstSibling();
            _bookmarks[i].gameObject.SetActive(true);
			if(i != _currentBookmark)
			{
				StartCoroutine(TurnGOOff(_bookmarks[i]));
			}
            //CreateBookmarkObject(i, bookmark, bookmarks);
        }

        //_bookmarks[_currentBookmark].SetActive(true);
    }

	IEnumerator TurnGOOff(GameObject go)
	{
		yield return null;
		go.SetActive(false);
	}

    void SetupBookTabs()
    {
        List<GameObject> bookmarks = new List<GameObject>();

        for (int i = 0; i < _bookmarks.Count; i++)
        {
            //CreateBookmarkObject(i, _bookmarks[i], bookmarks);
        }
        for (int i = 0; i < bookmarks.Count; i++)
        {
            bookmarks[i].transform.SetAsFirstSibling();
        }
    }

    void CreateBookmarkObject(int i, GameObject bookmark, List<GameObject> bookmarks)
    {
        GameObject bookmarkObject = Instantiate<GameObject>(_bookmarkTemplate.gameObject, _book.transform);
        RectTransform bookmarkTransform = bookmarkObject.GetComponent<RectTransform>();
        bookmarkTransform.localPosition += Vector3.right * _bookmarkPositions[i].x + Vector3.up * _bookmarkPositions[i].y;
        bookmarkObject.GetComponent<Image>().color = _bookmarkColors[i];

        bookmarkObject.name = i.ToString();
        Debug.Log("Adding a lisener to a bookmark for index " + i);
        bookmarkObject.GetComponent<Button>().onClick.AddListener(delegate { ToBookmark((int.Parse(bookmarkObject.name))); });
        //bookmarkObject.GetComponent<Button>().
        bookmarks.Add(bookmarkObject);
        //bookmarkObject.transform.position = new Vector3(_bookmarkPositions[i].x, _bookmarkPositions[i].y, 0.0f);
    }

    public void SetCurrentFlowerPage(int target)//Jens var här
    {
        PageLoader[] flowerPages = _bookmarks[_flowerOrganizerId].GetComponentsInChildren<PageLoader>(true);
        flowerPages[_currentPage].gameObject.SetActive(false);
        flowerPages[_currentPage + 1].gameObject.SetActive(false);
        _currentPage = target;
        flowerPages[_currentPage].gameObject.SetActive(true);
        flowerPages[_currentPage + 1].gameObject.SetActive(true);
    }

    public void ChangePage(int change)
    {
        switch (_currentBookmark)
        {
            /*case 1:
                List<Transform> children = _bookmarks[1].GetComponent<AlchemyOrganizerV">().GetPages();
                _currentPage = ChangeCurrentPage(children.Count, change);
                ChangePage(children);
                break;*/

            case 1:
                _currentPage = ChangeCurrentPage(FLOWERPAGE_INDEX.Count, change);
                ChangePage(FLOWERPAGE_INDEX);
                break;

            case 2:
                _currentPage = ChangeCurrentPage(_lorePages.Count, change);
                ChangePage(_lorePages);
                break;

            default:
                _currentPage = ChangeCurrentPage(FLOWERPAGE_INDEX.Count, change);
                ChangePage(FLOWERPAGE_INDEX);
                break;
        }
    }
    void ChangePage(List<Transform> transforms)
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            if (i == _currentPage || i == _currentPage + 1)
            {
                Debug.Log("Set page true");
                transforms[i].gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Set page false");
                transforms[i].gameObject.SetActive(false);
            }
        }
    }
    void ChangePage(List<PageLoader> pages)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == _currentPage || i == _currentPage + 1)
            {
                Debug.Log("Set page true");
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Set page false");
                pages[i].gameObject.SetActive(false);
            }
        }
    }
    int ChangeCurrentPage(int pageCount, int change)
    {
        Debug.Log("Change page");
        if ((pageCount % 2) == 0)
        {
            int curPag = (_currentPage + 2 * change) % pageCount;
            curPag = curPag < 0 ? curPag + pageCount : curPag;

            return curPag;
        }
        else
        {
            Debug.Log("Is Odd PageCount");
            return ((_currentPage + 2 * change) % pageCount);
        }
    }

    void ToBookmark(int index, bool playSounds)
    {
        if (index == _flowerOrganizerId && _currentBookmark == _flowerOrganizerId) //Jens var här
        {
            SetCurrentFlowerPage(0);
            //return;
        }
        if (index != _currentBookmark && playSounds)
        {
            FlipPageSoundEffect();
        }
        if (_currentBookmark != _bookmarks.Count - 1)
        {
            _bookmarks[_currentBookmark].SetActive(false);
        }

        _currentPage = 0;
        _currentBookmark = index;
        if (index == _bookmarks.Count - 1)
        {
            MapGenerator.Display(true);
        }
        else
        {
            MapGenerator.Display(false);
            _bookmarks[_currentBookmark].SetActive(true);
        }
        _book.GetComponent<Image>().sprite = _BookSprites[index];
    }

    public void ToBookmark(int index)
    {
        if (index == _flowerOrganizerId && _currentBookmark == _flowerOrganizerId)//Jens var här
        {
            SetCurrentFlowerPage(0);
            //return;
        }
        if (_currentBookmark != _bookmarks.Count - 1)
        {
            _bookmarks[_currentBookmark].SetActive(false);
        }

        _currentPage = 0;
        _currentBookmark = index;
        if (index == _bookmarks.Count - 1)
        {
            MapGenerator.Display(true);
        }
        else
        {
            MapGenerator.Display(false);
            _bookmarks[_currentBookmark].SetActive(true);
        }
        _book.GetComponent<Image>().sprite = _BookSprites[index];
    }

    public void FlipPageSoundEffect()
    {
        EventManager.TriggerEvent(EventNameLibrary.FLIP_PAGE, new EventParameter());
    }

    public GameObject GetBookmark(int i)
    {
        return _bookmarks[i];
    }
}
