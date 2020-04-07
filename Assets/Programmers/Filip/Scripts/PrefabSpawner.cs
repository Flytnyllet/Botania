using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    static readonly int DEGREES_360 = 360;
    static readonly float STANDARD_GRID_OFFSET = 0.5f;

    public static void SpawnOnChunk(Biome biome, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        //Generate all noises according to chunk position
        biome.Setup(chunkCoord); 
        SpawnFromSpawnables(biome, biome.Spawnables, heightMap, meshData, meshSettings, container, chunkCoord);
    }

    private static void SpawnFromSpawnables(Biome biome, Spawnable[] spawnables, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Transform container, Vector2 chunkCoord)
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            //First spawn the subspawners prefabs as they are harder to make room for
            if (spawnables[i].SubSpawners.Length > 0)
                SpawnFromSpawnables(biome, spawnables[i].SubSpawners, heightMap, meshData, meshSettings, container, chunkCoord);

            //Get noise specific to this prefab
            float[,] spawnNoise = spawnables[i].GetNoise;

            //+ 1 offset in loops are due to border around mesh
            for (int x = 0; x < meshSettings.ChunkSize - spawnables[i].Size + 1; x++)
            {
                for (int y = 0; y < meshSettings.ChunkSize - spawnables[i].Size + 1; y++)
                {
                    //No use in checking if it can spawn if that square is occopied
                    if (biome.CanObjectSpawn(x, y, spawnables[i].Size))
                    {
                        bool insideNoise = spawnNoise[x, y] > spawnables[i].NoiseStartPoint; //is it inside the noise?
                        bool gradientSpawn = spawnNoise[x, y] + spawnables[i].OffsetNoise[x, y] > spawnables[i].Thickness; //If it is, transition?
                        bool uniformSpread = x % spawnables[i].UniformSpreadAmount == 0 && y % spawnables[i].UniformSpreadAmount == 0; //uniform spread?
                        bool noiseSpread = spawnables[i].SpreadNoise[y, x] > spawnables[i].RandomSpread;

                        //Slope
                        Vector3 normal = meshData.GetNormal(y * (meshSettings.ChunkSize) + x);
                        float slopeAngle = Vector3.Angle(Vector3.up, normal);

                        bool minSlope = (slopeAngle <= spawnables[i].SoftMinSlope * spawnables[i].OffsetNoise[x, y]);
                        minSlope = minSlope && slopeAngle <= spawnables[i].HardMinSlope;
                        bool maxSlope = (slopeAngle >= spawnables[i].SoftMaxSlope * spawnables[i].OffsetNoise[x, y]);
                        maxSlope = maxSlope && slopeAngle >= spawnables[i].HardMaxSlope;

                        //height bools
                        bool minHeight = (heightMap.heightMap[x, y] > -0.001f + heightMap.minValue + spawnables[i].SoftMinHeight * spawnables[i].OffsetNoise[y, x]);
                        minHeight = minHeight && heightMap.heightMap[x, y] > spawnables[i].HardMinHeight;
                        bool maxHeight = (heightMap.heightMap[x, y] < heightMap.maxValue - spawnables[i].SoftMaxHeight * spawnables[i].OffsetNoise[x, y]);
                        maxHeight = maxHeight && heightMap.heightMap[x, y] < spawnables[i].HardMaxHeight;

                        //Things inside the if statement only need to be determined if it should spawn
                        if (insideNoise && gradientSpawn && uniformSpread && noiseSpread && minHeight && maxHeight & minSlope && maxSlope)
                        {
                            //Since the object can spawn, mark it's space as occopied
                            biome.OccupyWithObject(x, y, spawnables[i].Size);

                            //Current local positions in x and y in chunk, used only to spawn from
                            float xPos = x + (STANDARD_GRID_OFFSET * spawnables[i].Size) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh +- 1 corrects it to the right grid position
                            float zPos = y + (STANDARD_GRID_OFFSET * spawnables[i].Size) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh +- 1 corrects it to the right grid position
                            float yPos = heightMap.heightMap[x + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size), y + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size)] + 0.5f;

                            //Position from grid in world
                            Vector3 objectPosition = new Vector3((xPos + chunkCoord.x) * meshSettings.MeshScale, yPos, -(zPos + chunkCoord.y) * meshSettings.MeshScale);
                            //Vector to offset from grid slightly to create less uniform distribution
                            Vector3 offsetVector = new Vector3(spawnables[i].OffsetNoise[x, y] * 2 - 1, 0.0f, spawnables[i].SpreadNoise[x, y] * 2 - 1);

                            objectPosition += offsetVector * spawnables[i].OffsetAmount;

                            //How much along the normal should the object point?
                            float tiltAmount = spawnables[i].SurfaceNormalAmount + (spawnables[i].SpreadNoise[x, y] * 2 - 1) * spawnables[i].PointAlongNormalRandomness;
                            Vector3 finalRotation = LerpToVector(normal, Vector3.up, tiltAmount);
                            //Random rotation based on noise
                            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, finalRotation);

                            GameObject newObject = Instantiate(spawnables[i].Prefab, objectPosition, rotation, container) as GameObject;
                            newObject.transform.RotateAround(objectPosition, newObject.transform.up, spawnables[i].OffsetNoise[x, y] * DEGREES_360 * spawnables[i].RotationAmount);
                        }
                    }               
                }
            }
        }
    }

    private static Vector3 LerpToVector(Vector3 toVector, Vector3 fromVector, float amount)
    {
        amount = amount <= 0 ? 0 : amount;
        return toVector * amount + fromVector * (1 - amount);
    }
}
