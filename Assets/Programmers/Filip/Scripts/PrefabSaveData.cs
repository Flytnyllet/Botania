using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSaveData : MonoBehaviour
{
    [SerializeField, Tooltip("With this true it will still spawn it but only partially! Make sure this prefab support that!")] bool _spawnPartially = false;
    [SerializeField] bool _groupSpawn = false;

    StoredSaveData _saveData;

    //Content of this is what needs to be saved (right now only contains way to identify object)
    public StoredSaveData SaveData { get { return _saveData; } private set { _saveData = value; } }

    public void SetSaveData(StoredSaveData saveData)
    {
        this._saveData = saveData;
        _saveData.SetPartialSpawn(_spawnPartially);
        _saveData.SetGroupSpawn(_groupSpawn);
    }

    //Call this function on pickup
    public void StoreInPrefabSpawnerSaveData()
    {
        PrefabSpawnerSaveData.AddPickup(_saveData);
    }
}

[System.Serializable]
public struct StoredSaveData
{
    //The key for dictionary which holds chunk coord and index in coord
    ChunkCoordIndex _chunkCoordIndex;

    bool _partialSpawn;
    bool _groupSpawn;

    public ChunkCoordIndex ChunkCoordIndex { get { return _chunkCoordIndex; } private set { _chunkCoordIndex = value; } } //Identify this specific object!
    public bool PartialSpawn { get { return _partialSpawn; } private set { _partialSpawn = value; } }
    public bool GroupSpawn { get { return _groupSpawn; } private set { _groupSpawn = value; } }

    public void SetPartialSpawn(bool partialSpawn)
    {
        this._partialSpawn = partialSpawn;
    }

    public void SetGroupSpawn(bool groupSpawn)
    {
        this._groupSpawn = groupSpawn;
    }

    //Is only created in the prefab spawner and set only there
    public StoredSaveData(Vector2 chunkCoord, Vector2 itemIndex)
    {
        this._partialSpawn = false;
        this._groupSpawn = false;
        this._chunkCoordIndex = new ChunkCoordIndex(chunkCoord, itemIndex);
    }
}

//The key for dictionary which holds chunk coord and index in coord
[System.Serializable]
public struct ChunkCoordIndex
{
    float _chunkCoord_X;
    float _chunkCoord_Y;

    float _itemIndex_X;
    float _itemIndex_Y;

    public Vector2 ChunkCoord
    {
        get
        {
            return new Vector2(_chunkCoord_X, _chunkCoord_Y);
        }
        private set
        {
            _chunkCoord_X = value.x;
            _chunkCoord_Y = value.y;
        }
    }

    public Vector2 ItemIndex
    {
        get
        {
            return new Vector2(_itemIndex_X, _itemIndex_Y);
        }
        private set
        {
            _itemIndex_X = value.x;
            _itemIndex_Y = value.y;
        }
    }

    public ChunkCoordIndex(Vector2 chunkCoord, Vector2 itemIndex)
    {
        _chunkCoord_X = chunkCoord.x;
        _chunkCoord_Y = chunkCoord.y;
        _itemIndex_X = itemIndex.x;
        _itemIndex_Y = itemIndex.y;
    }
}
