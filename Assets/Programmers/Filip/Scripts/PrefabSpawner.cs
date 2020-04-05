using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    static readonly int DEGREES_360 = 360;

    public static void SpawnOnChunk(Biome biome, HeightMap heightMap, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        float height = 1; //Remove later, only for testing purposes atm, ALSO FOR HEIGHT IN PARAMETER!!!
        //Generate all noises according to chunk position
        biome.Setup(chunkCoord); 
        SpawnFromSpawnables(height, biome, biome.Spawnables, heightMap, meshSettings, container, chunkCoord);
    }

    private static void SpawnFromSpawnables(float height, Biome biome, Spawnable[] spawnables, HeightMap heightMap, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            //Get noise specific to this prefab
            float[,] spawnNoise = spawnables[i].GetNoise;

            for (int x = 0; x < meshSettings.ChunkSize; x++)
            {
                for (int y = 0; y < meshSettings.ChunkSize; y++)
                {
                    //Current local positions in x and y in chunk, used only to spawn from
                    float xPos = x - meshSettings.ChunkSize / 2;
                    float yPos = y - meshSettings.ChunkSize / 2;

                    //Position from grid in world
                    Vector3 objectPosition = new Vector3((xPos + chunkCoord.x) * meshSettings.MeshScale, heightMap.heightMap[x, y] + 0.35f * height, -(yPos + chunkCoord.y) * meshSettings.MeshScale);
                    //Vector to offset from grid slightly to create less uniform distribution
                    Vector3 offsetVector = new Vector3(spawnables[i].OffsetNoise[x, y] * 2 - 1, 0.0f, spawnables[i].SpreadNoise[x, y] * 2 - 1);

                    objectPosition += offsetVector * spawnables[i].OffsetAmount;

                    //Random rotation based on noise
                    Quaternion rotation = Quaternion.Euler(0, spawnables[i].OffsetNoise[x, y] * DEGREES_360, 0);

                    bool insideNoise = spawnNoise[x, y] > spawnables[i].NoiseStartPoint; //is it inside the noise?
                    bool gradientSpawn = spawnNoise[x, y] + spawnables[i].OffsetNoise[x, y] > spawnables[i].Thickness; //If it is, transition?
                    bool uniformSpread = x % spawnables[i].UniformSpreadAmount == 0 && y % spawnables[i].UniformSpreadAmount == 0; //uniform spread?
                    bool noiseSpread = spawnables[i].SpreadNoise[y, x] > spawnables[i].RandomSpread;

                    //height bools
                    bool minHeight = (heightMap.heightMap[x, y] > heightMap.minValue + spawnables[i].SoftMinHeight * spawnables[i].OffsetNoise[y, x]);
                    minHeight = minHeight && heightMap.heightMap[x, y] > spawnables[i].HardMinHeight;
                    bool maxHeight = (heightMap.heightMap[x, y] < heightMap.maxValue - spawnables[i].SoftMaxHeight * spawnables[i].OffsetNoise[x, y]);
                    maxHeight = maxHeight && heightMap.heightMap[x, y] < spawnables[i].HardMaxHeight;

                    if (insideNoise && gradientSpawn && uniformSpread && noiseSpread && minHeight && maxHeight)
                        Instantiate(spawnables[i].Prefab, objectPosition, rotation, container);

                    //if (x % 2 == 0 && y % 2 == 0)
                    //    Instantiate(spawnables[i].Prefab, objectPosition, Quaternion.identity, container);
                }
            }

            if (spawnables[i].SubSpawners.Length > 0)
                SpawnFromSpawnables(height + 1.25f, biome, spawnables[i].SubSpawners, heightMap, meshSettings, container, chunkCoord);
        }
    }
}
