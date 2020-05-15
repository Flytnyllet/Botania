using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursorOnClick : MonoBehaviour
{
    [SerializeField] Texture2D _defaultCursor;
    [SerializeField] Texture2D _clickCursor;

    [SerializeField] Vector2 _hotspot;

    static ChangeCursorOnClick _thisScript;
    bool _currentlyDefault = true;

    private void Awake()
    {
        if (_thisScript == null)
        {
            _thisScript = this;

            if (_defaultCursor == null || _clickCursor == null)
                Debug.LogError("You need to assign cursor textures in: " + this);

            SetCursor(_defaultCursor);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        bool clicked = Input.GetMouseButton(0);

        if (clicked && _currentlyDefault)
        {
            SetCursor(_clickCursor);
            _currentlyDefault = false;
        }
        else if (!clicked && !_currentlyDefault)
        {
            SetCursor(_defaultCursor);
            _currentlyDefault = true;
        }
    }

    void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, _hotspot, CursorMode.Auto);
    }
}
