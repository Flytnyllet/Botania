using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrefabSpawnerSaveData
{
    static Dictionary<ChunkCoordIndex, StoredSaveData> _storedSaveDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();
    static Dictionary<ChunkCoordIndex, StoredSaveData> _spawnAreaDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();

    public static void ClearStartArea(int startIndex, int clearSize)
    {
        for (int x = startIndex - clearSize; x < startIndex + clearSize; x++)
        {
            for (int y = startIndex - clearSize; y < startIndex + clearSize; y++)
            {
                StoredSaveData data = new StoredSaveData(Vector2.zero, new Vector2(x, y));
                _spawnAreaDataDic.Add(data.ChunkCoordIndex, data);
            }
        }
    }

    public static void Save()
    {
        List<StoredSaveData> saveData = _storedSaveDataDic.Select(value => value.Value).ToList();
        Serialization.Save(Saving.FileNames.PREFAB_SPAWNING, saveData);
    }

    public static void Load()
    {
        _storedSaveDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();

        List<StoredSaveData> saveData = (List<StoredSaveData>)Serialization.Load(Saving.FileNames.PREFAB_SPAWNING);

        if (saveData != null)
        {
            for (int i = 0; i < saveData.Count; i++)
            {
                AddPickup(saveData[i]);
            }
        }
    }

    public static void Wipe()
    {
        List<StoredSaveData> saveData = new List<StoredSaveData>();

        Serialization.Save(Saving.FileNames.PREFAB_SPAWNING, saveData);
    }

    public static void AddPickup(StoredSaveData saveDataToStore)
    {
        try
        {
            _storedSaveDataDic.Add(saveDataToStore.ChunkCoordIndex, saveDataToStore);
        }
        catch
        {
            //If an object is marked as group spawn it contains multiple pickups with same save index. Picking one will save it as everyone in it has been picked
            if (!saveDataToStore.GroupSpawn)
                Debug.LogError("There is already a pickup saved at this chunk coord and index!? Probably you have set the size of a pickup to 0, IT MUST BE ABOVE 0!  :  " + saveDataToStore.ChunkCoordIndex.ChunkCoord + "    " + saveDataToStore.ChunkCoordIndex.ItemIndex);
        }
    }

    //Used to say the pickup may spawn again
    public static void RemovePickup(StoredSaveData saveDataToStore)
    {
        if (_storedSaveDataDic.ContainsKey(saveDataToStore.ChunkCoordIndex))
            _storedSaveDataDic.Remove(saveDataToStore.ChunkCoordIndex);
    }

    //Used only by spawning to check if it should spawn
    public static StoredSaveData GetStoredSaveData(ChunkCoordIndex index)
    {
        return _storedSaveDataDic[index];
    }

    public static bool InsideSpawnArea(ChunkCoordIndex chunkCoordIndex)
    {
        return _spawnAreaDataDic.ContainsKey(chunkCoordIndex);
    }

    //Checks if it should spawn depending on key (used in spawning script only)
    public static bool ContainsChunkCoordIndex(ChunkCoordIndex chunkCoordIndex)
    {
        return _storedSaveDataDic.ContainsKey(chunkCoordIndex);
    }
}
