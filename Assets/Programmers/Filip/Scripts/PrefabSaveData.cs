using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSaveData : MonoBehaviour
{
    StoredSaveData _saveData;

    //Content of this is what needs to be saved (right now only contains way to identify object)
    public StoredSaveData SaveData { get { return _saveData; } private set { _saveData = value; } }

    public void SetSaveData(StoredSaveData saveData)
    {
        this._saveData = saveData;
    }

    //Call this function on pickup
    public void StoreInPrefabSpawnerSaveData()
    {
        PrefabSpawnerSaveData.AddPickup(_saveData);
    }
}


public struct StoredSaveData
{
    //The key for dictionary which holds chunk coord and index in coord
    ChunkCoordIndex _chunkCoordIndex;

    //ROOM TO ADD NEW INFO TO STORE

    public ChunkCoordIndex ChunkCoordIndex { get { return _chunkCoordIndex; } private set { _chunkCoordIndex = value; } } //Identify this specific object!

    //Is only created in the prefab spawner and set only there
    public StoredSaveData(Vector2 chunkCoord, Vector2 itemIndex)
    {
        this._chunkCoordIndex = new ChunkCoordIndex(chunkCoord, itemIndex);
    }
}

//The key for dictionary which holds chunk coord and index in coord
public struct ChunkCoordIndex
{
    Vector2 _chunkCoord;
    Vector2 _itemIndex;

    public Vector2 ChunkCoord { get { return _chunkCoord; } private set { _chunkCoord = value; } }
    public Vector2 ItemIndex { get { return _itemIndex; } private set { _itemIndex = value; } }

    public ChunkCoordIndex(Vector2 chunkCoord, Vector2 itemIndex)
    {
        this._chunkCoord = chunkCoord;
        this._itemIndex = itemIndex;
    }
}
