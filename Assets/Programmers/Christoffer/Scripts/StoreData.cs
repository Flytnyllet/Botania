using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class StoreData
{
    //private static StoreData _current;
    //public static StoreData current {
    //    get {
    //        if (_current == null)
    //            _current = new StoreData();
    //        return _current;
    //    }
    //}

    static private List<Flower> flowers;
    static public List<Flower> Flowers { get; private set; }

    static private PlayerData playerData;
    static public PlayerData PlayerData { get; private set; }

}