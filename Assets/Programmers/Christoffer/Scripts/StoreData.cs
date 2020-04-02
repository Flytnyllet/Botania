using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreData : MonoBehaviour
{
    private static StoreData _current;
    public static StoreData current {
        get {
            if (_current == null)
                _current = new StoreData();
            return _current;
        }
    }

    public PlayerData profile;

    public int cube;
    public int globe;
}
