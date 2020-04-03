using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public static void SpawnOnChunk(Biome biome, HeightMap heightMap, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        biome.Setup();

        for (int x = 0; x < meshSettings.ChunkSize; x++)
        {
            for (int y = 0; y < meshSettings.ChunkSize; y++)
            {
                float xPos = x - meshSettings.ChunkSize / 2;
                float yPos = y - meshSettings.ChunkSize / 2;

                Vector3 objectPosition = new Vector3((xPos + chunkCoord.x) * meshSettings.MeshScale, heightMap.heightMap[x, y] + 0.5f, -(yPos + chunkCoord.y) * meshSettings.MeshScale);

                if (x % 2 == 0 && y % 2 == 0)
                    Instantiate(biome.Spawnables[0].Prefab, objectPosition, Quaternion.identity, container);
            }
        }
    }
}
