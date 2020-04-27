using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    const string INPUT_INVENTORY = "Inventory";
    [SerializeField] List<GameObject> _bookmarks = new List<GameObject>();
    [SerializeField] List<PageLoader> _flowerPages = new List<PageLoader>();
    [SerializeField] List<PageLoader> _lorePages = new List<PageLoader>();
    [SerializeField] int _flowerOrganizerId;
    [SerializeField] RectTransform _bookmarkTemplate;
    [SerializeField] GameObject _emptyPageTemplate;
    //[SerializeField] GameObject _nextPageTemplate;
    [SerializeField] Vector2[] _bookmarkPositions;
    [SerializeField] Color[] _bookmarkColors;
    int _currentBookmark = 0;
    int _currentPage = 0;
    [SerializeField] GameObject _book;
    //[SerializeField] GameObject _prevPage;
    //[SerializeField] GameObject _nextPage;

    void Awake()
    {
        SetupExtraBookmarks();
        SetupPage(_flowerOrganizerId, _flowerPages);
        foreach (PageLoader page in _flowerPages)
        {

        }

        /*
		if (_book == null)
		{
			_book = transform.Find("Book").gameObject; //slow
		}
		_book.SetActive(false); */
    }

    void Update()
    {
        if (Input.GetButtonDown(INPUT_INVENTORY))
        {
            Debug.Log("Inventory button");
            _book.SetActive(!_book.activeSelf);
            if (_book.activeSelf)
            {
                EventManager.TriggerEvent(EventNameLibrary.OPEN_BOOK, new EventParameter());
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
            }
            else
            {
                EventManager.TriggerEvent(EventNameLibrary.CLOSE_BOOK, new EventParameter());
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
            }
        }
    }

    void SetupPage(int pageParentID, List<PageLoader> pages)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            GameObject page = Instantiate<GameObject>(pages[i].gameObject, _bookmarks[pageParentID].transform);

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
            GameObject emptyPage = Instantiate<GameObject>(_emptyPageTemplate, _bookmarks[pageParentID].transform);
            emptyPage.gameObject.SetActive(false);
            pages.Add(emptyPage.GetComponent<PageLoader>());
        }

    }
    void SetupExtraBookmarks()
    {
        List<GameObject> bookmarks = new List<GameObject>();
        //int[] bmI = new int[_bookmarks.Count]; //BookMarkIndex
        for (int i = 0; i < _bookmarks.Count; i++)
        {
            GameObject bookmark = _bookmarks[i];
            _bookmarks[i] = Instantiate<GameObject>(bookmark, _book.transform);
            _bookmarks[i].transform.SetAsFirstSibling();
            GameObject bookmarkObject = Instantiate<GameObject>(_bookmarkTemplate.gameObject, _book.transform);
            RectTransform bookmarkTransform = bookmarkObject.GetComponent<RectTransform>();
            bookmarkTransform.localPosition += Vector3.right * _bookmarkPositions[i].x + Vector3.up * _bookmarkPositions[i].y;
            bookmarkObject.GetComponent<Image>().color = _bookmarkColors[i];

            //bmI[i] = i;
            bookmarkObject.name = i.ToString();
            Debug.Log("Adding a lisener to a bookmark for index " + i);
            bookmarkObject.GetComponent<Button>().onClick.AddListener(delegate { ToBookmark((int.Parse(bookmarkObject.name))); });
            //bookmarkObject.GetComponent<Button>().
            bookmarks.Add(bookmarkObject);
            //bookmarkObject.transform.position = new Vector3(_bookmarkPositions[i].x, _bookmarkPositions[i].y, 0.0f);
        }
        for (int i = 0; i < bookmarks.Count; i++)
        {
            bookmarks[i].transform.SetAsFirstSibling();
        }
    }
    public void ChangePage(int change)
    {
        switch (_currentBookmark)
        {
            case 1:
                List<Transform> children = _bookmarks[1].GetComponent<AlchemyOrganizer>().GetPages();
                _currentPage = ChangeCurrentPage(children.Count, change);
                ChangePage(children);
                break;

            case 2:
                _currentPage = ChangeCurrentPage(_lorePages.Count, change);
                ChangePage(_lorePages);
                break;

            default:
                _currentPage = ChangeCurrentPage(_flowerPages.Count, change);
                ChangePage(_flowerPages);
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

    void ToBookmark(int index)
    {
        Debug.Log("Current bookmark changed from " + _currentBookmark + " to " + index);
        _bookmarks[_currentBookmark].SetActive(false);
        _currentBookmark = index;
        _currentPage = 0;
        _bookmarks[_currentBookmark].SetActive(true);
    }
    public void FlipPage()
    {
        EventManager.TriggerEvent(EventNameLibrary.FLIP_PAGE, new EventParameter());
    }
}
