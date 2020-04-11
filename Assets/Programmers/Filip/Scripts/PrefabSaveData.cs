using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSaveData : MonoBehaviour
{
    public StoredSaveData _saveData;

    public void SetSaveData(StoredSaveData saveData)
    {
        this._saveData = saveData;
    }

    public void StoreInPrefabSpawnerSaveData()
    {
        PrefabSpawnerSaveData.AddPickup(_saveData);
    }
}

// REMOVE SERILIZE THINGS LATER, ONLY USED TO VISUALIZE IN TESTING!!!


public struct StoredSaveData
{
    ChunkCoordIndex _chunkCoordIndex;

    public ChunkCoordIndex ChunkCoordIndex { get { return _chunkCoordIndex; } private set { _chunkCoordIndex = value; } } //Identify this specific object!

    //ROOM TO ADD NEW INFO TO STORE

    public StoredSaveData(Vector2 chunkCoord, Vector2 itemIndex)
    {
        this._chunkCoordIndex = new ChunkCoordIndex(chunkCoord, itemIndex);
    }
}

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
