using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public static void SpawnOnChunk(Biome biome, HeightMap heightMap, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        float height = 1;
        biome.Setup(chunkCoord);
        SpawnFromSpawnables(height, biome, biome.Spawnables, heightMap, meshSettings, container, chunkCoord);
    }

    private static void SpawnFromSpawnables(float height, Biome biome, Spawnable[] spawnables, HeightMap heightMap, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            float[,] spawnNoise = spawnables[i].GetNoise;
           // Debug.Log("SpawnNoise Length: " + spawnNoise.GetLength(0) + " chunksize: " + meshSettings.ChunkSize);

            for (int x = 0; x < meshSettings.ChunkSize; x++)
            {
                for (int y = 0; y < meshSettings.ChunkSize; y++)
                {
                    float xPos = x - meshSettings.ChunkSize / 2;
                    float yPos = y - meshSettings.ChunkSize / 2;

                    Vector3 objectPosition = new Vector3((xPos + chunkCoord.x) * meshSettings.MeshScale, heightMap.heightMap[x, y] + height, -(yPos + chunkCoord.y) * meshSettings.MeshScale);
                    Quaternion rotation = Quaternion.Euler(0, biome.OffsetNoise[x, y] * 360, 0);

                    if (spawnNoise[x, y] > 0.01f && x % 2 == 0 && y % 2 == 0)
                        Instantiate(spawnables[i].Prefab, objectPosition, rotation, container);

                    //if (x % 2 == 0 && y % 2 == 0)
                    //    Instantiate(spawnables[i].Prefab, objectPosition, Quaternion.identity, container);
                }
            }

            if (spawnables[i].SubSpawners.Length > 0)
                SpawnFromSpawnables(height + 1, biome, spawnables[i].SubSpawners, heightMap, meshSettings, container, chunkCoord);
        }
    }
}
