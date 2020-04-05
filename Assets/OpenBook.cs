using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBook : MonoBehaviour
{
    //Quick and Dirty thing to open the book, this should be probably be replaced entierly.
    const string BOOK_OBJECT_NAME = "Book";

    [SerializeField] GameObject _bookObject;
    private void Awake()
    {
        if (_bookObject == null)
        {
            _bookObject = transform.Find(BOOK_OBJECT_NAME).gameObject; //slow
        }
    }

    void Update()
    {

        if (Input.GetButtonDown("Inventory"))
        {
            _bookObject.SetActive(!_bookObject.activeSelf);
        }
    }
}
