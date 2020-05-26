using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;

//This is called "AlphaAware" but at the moment it simply uses a 2D collider

public class AlphaAwareButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] List<MetaEvent> _actions;
    Image _image;
    [SerializeField] Sprite[] _sprites;

    RectTransform _rect;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect,
            eventData.position, eventData.pressEventCamera, out localCursor))
        {
            localCursor.x *= _image.sprite.texture.width / _rect.sizeDelta.x;
            localCursor.y *= _image.sprite.texture.height / _rect.sizeDelta.y;
            for (int i = 0; i < _sprites.Length; i++)
            {
                if (_sprites[i].texture.GetPixel((int)localCursor.x, (int)localCursor.y).a > 0.01f)
                {
                    _actions[i].Event.Invoke();
                    return;
                }
            }
        }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }


}

[System.Serializable]
public class MetaEvent
{
    public UnityEvent Event;
}
#if UNITY_EDITOR
[CustomEditor(typeof(AlphaAwareButton))]
public class MetaEventSetEditor : Editor
{

    public override void OnInspectorGUI()
    {
        AlphaAwareButton _target = (AlphaAwareButton)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_actions"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_sprites"), true);
        serializedObject.ApplyModifiedProperties();
    }

}
# endif

