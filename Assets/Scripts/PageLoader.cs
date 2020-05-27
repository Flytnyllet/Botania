using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// !WARNING, DO NOT LET THESE OBJECTS BE DISABLED ON START UP! THEY ARE REQUIRED TO RUN AT LEAST THEIR STARTING METHODS AS TO ALLOW THE FLOWER SYSTEM TO FUNCTION
public class PageLoader : MonoBehaviour
{
    [System.Serializable]
    protected class PageElement
    {
        [SerializeField] UnityEngine.UI.MaskableGraphic _bookElement = null;
        [SerializeField] int _unlockPoint = 0;

        public UnityEngine.UI.MaskableGraphic GetBookElement { get => _bookElement; }
        public int GetUnlockPoint { get => _unlockPoint; }
    }

    [Tooltip("Används för att bestämma när olika element i sidan skall visas. \n"
        + "Dra in text/image och sätt in hur många blommor som ska plockas upp innan elementer låses upp")]
    [SerializeField] List<PageElement> _pageElements = new List<PageElement>();
    [SerializeField] protected string _flowerName = "Grass";


    protected void OnEnable()
    {
        SetPageElementVisibility();
    }

    //Aktiverar de Objekt vars unlock conditions uppfylls. Just nu bara kravet att plocka ett visst antal blommor
    protected void SetPageElementVisibility()
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

    public void NextPage()
    {
        int i = transform.GetSiblingIndex();
        int nextI = (i + 2) % transform.parent.childCount;
        transform.parent.GetChild(nextI).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void GoToPage(int i)
    {
        BookManager.Instance.ChangePage(i);
    }

}
