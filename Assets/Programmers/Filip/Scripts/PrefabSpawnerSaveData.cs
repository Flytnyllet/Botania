using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabSpawnerSaveData
{
    static Dictionary<ChunkCoordIndex, StoredSaveData> _storedSaveDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();

    public static void AddPickup(StoredSaveData saveDataToStore)
    {
        try
        {
            _storedSaveDataDic.Add(saveDataToStore.ChunkCoordIndex, saveDataToStore);
        }
        catch
        {
            Debug.LogError("There is already a pickup saved at this chunk coord and index!? Probably you have set the size of a pickup to 0, IT MUST BE ABOVE 0!  :  " + saveDataToStore.ChunkCoordIndex.ChunkCoord + "    " + saveDataToStore.ChunkCoordIndex.ItemIndex);
        }
    }

    public static void RemovePickup(StoredSaveData saveDataToStore)
    {
        if (_storedSaveDataDic.ContainsKey(saveDataToStore.ChunkCoordIndex))
            _storedSaveDataDic.Remove(saveDataToStore.ChunkCoordIndex);
    }

    public static bool ContainsChunkCoordIndex(ChunkCoordIndex chunkCoordIndex)
    {
        return _storedSaveDataDic.ContainsKey(chunkCoordIndex);
    }
}
