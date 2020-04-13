using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    static readonly int DEGREES_360 = 360;
    static readonly float STANDARD_GRID_OFFSET = 0.5f;

    bool[,] _occupiedGrid;

    public bool[,] OccupiedGrid { get { return _occupiedGrid; } private set { _occupiedGrid = value; } }

    public List<SpawnInfo> SpawnOnChunk(int levelOfDetail, Biome biome, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 chunkPosition, Vector2 chunkCoord)
    {
        _occupiedGrid = new bool[meshSettings.ChunkSize - 1, meshSettings.ChunkSize - 1];

        //Generate all noises according to chunk position
        biome.Setup(chunkPosition); 

        switch(levelOfDetail)
        {
            case (0): { return SpawnFromSpawnables(levelOfDetail, biome, biome.HighLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, false); }
            case (1): { return SpawnFromSpawnables(levelOfDetail, biome, biome.MediumLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, false); }
            case (2): { return SpawnFromSpawnables(levelOfDetail, biome, biome.LowLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, true); }
            default: throw new System.Exception("There should be no levelOfDetail of " + levelOfDetail);
        }
    }

    private List<SpawnInfo> SpawnFromSpawnables(int levelOfDetail, Biome biome, Spawnable[] spawnables, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 chunkPosition, Vector2 chunkCoord, bool firstCall = false)
    {
        List<SpawnInfo> spawnInfo = new List<SpawnInfo>();

        //Spawn Water
        if (firstCall)
            spawnInfo.Add(new SpawnInfo(biome.WaterChunk, new Vector3(chunkPosition.x, biome.WaterHeight, -chunkPosition.y), Quaternion.Euler(90, 0, 0), 0, chunkCoord, Vector2.zero, false, new Vector3((meshSettings.ChunkSize - 1) * meshSettings.MeshScale, (meshSettings.ChunkSize - 1) * meshSettings.MeshScale, 1)));

        for (int i = 0; i < spawnables.Length; i++)
        {
            //First spawn the subspawners prefabs as they are harder to make room for
            if (spawnables[i].SubSpawners.Length > 0)
            {
                List<SpawnInfo> childSpawnInfo = SpawnFromSpawnables(levelOfDetail, biome, spawnables[i].SubSpawners, heightMap, meshData, meshSettings, chunkPosition, chunkCoord);
                spawnInfo.AddRange(childSpawnInfo);
            }

            //Get noise specific to this prefab
            float[,] spawnNoise = spawnables[i].GetNoise;
            float spawnableSizeForGridAlign = spawnables[i].Size == 0 ? 1 : spawnables[i].Size;

            //+ 1 offset in loops are due to border around mesh
            for (int x = 0; x < meshSettings.ChunkSize - spawnableSizeForGridAlign; x++)
            {
                for (int y = 0; y < meshSettings.ChunkSize - spawnableSizeForGridAlign; y++)
                {
                    Vector2 chunkIndex = new Vector2(x, y);

                    //This thing is already picked up! (Size > 0 is just to check if the thing is pickable)
                    if (spawnables[i].Size > 0 && PrefabSpawnerSaveData.ContainsChunkCoordIndex(new ChunkCoordIndex(chunkCoord, chunkIndex)))
                    {
                        //ADD STUFF HERE IF THERE SHOULD BE REGROWTH OR SOMETHING!
                    }
                    else
                    {
                        //No use in checking if it can spawn if that square is occopied
                        if (CanObjectSpawn(x, y, spawnables[i].Size, heightMap.heightMap, spawnables[i].SpawnDifferencial, meshSettings.ChunkSize))
                        {
                            bool insideNoise = spawnNoise[x, y] > spawnables[i].NoiseStartPoint; //is it inside the noise?
                            bool gradientSpawn = spawnNoise[x, y] + spawnables[i].OffsetNoise[x, y] > spawnables[i].Thickness; //If it is, transition?
                            bool uniformSpread = x % spawnables[i].UniformSpreadAmount == 0 && y % spawnables[i].UniformSpreadAmount == 0; //uniform spread?
                            bool noiseSpread = spawnables[i].SpreadNoise[y, x] > spawnables[i].RandomSpread;

                            //Slope
                            Vector3 normal = Vector3.up;
                            //Only on the highest detail level, care about normal
                            if (levelOfDetail == 0)
                                normal = meshData.GetNormal(y * (meshSettings.ChunkSize) + x);
                            float slopeAngle = Vector3.Angle(Vector3.up, normal);

                            bool minSlope = (slopeAngle <= spawnables[i].SoftMinSlope * spawnables[i].OffsetNoise[x, y]);
                            minSlope = minSlope && slopeAngle <= spawnables[i].HardMinSlope;
                            bool maxSlope = (slopeAngle >= spawnables[i].SoftMaxSlope * spawnables[i].OffsetNoise[x, y]);
                            maxSlope = maxSlope && slopeAngle >= spawnables[i].HardMaxSlope;

                            //height bools
                            bool minHeight = (heightMap.heightMap[x, y] > spawnables[i].HardMinHeight + spawnables[i].SoftMinAmount * spawnables[i].OffsetNoise[x, y]);
                            bool maxHeight = (heightMap.heightMap[x, y] <= spawnables[i].HardMaxHeight - spawnables[i].SoftMaxAmount * spawnables[i].OffsetNoise[x, y]);

                            //Things inside the if statement only need to be determined if it should spawn
                            if (insideNoise && gradientSpawn && uniformSpread && noiseSpread && minHeight && maxHeight && minSlope && maxSlope)
                            {
                                //Since the object can spawn, mark it's space as occopied
                                OccupyWithObject(x, y, spawnables[i].Size, meshSettings.ChunkSize);

                                //Current local positions in x and y in chunk, used only to spawn from
                                float xPos = x + STANDARD_GRID_OFFSET + (STANDARD_GRID_OFFSET * spawnableSizeForGridAlign) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh + STANDARD_GRID_OFFSET corrects it to the right grid position
                                float zPos = y + STANDARD_GRID_OFFSET + (STANDARD_GRID_OFFSET * spawnableSizeForGridAlign) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh + STANDARD_GRID_OFFSET corrects it to the right grid position
                                float yPos = heightMap.heightMap[x + (int)(STANDARD_GRID_OFFSET * spawnableSizeForGridAlign) + 1, y + (int)(STANDARD_GRID_OFFSET * spawnableSizeForGridAlign) + 1] + spawnables[i].Height;

                                //Position from grid in world
                                Vector3 objectPosition = new Vector3((xPos + chunkPosition.x) * meshSettings.MeshScale, yPos, -(zPos + chunkPosition.y) * meshSettings.MeshScale);
                                //Vector to offset from grid slightly to create less uniform distribution
                                Vector3 offsetVector = new Vector3(spawnables[i].OffsetNoise[x, y] * 2 - 1, 0.0f, spawnables[i].SpreadNoise[x, y] * 2 - 1);

                                objectPosition += offsetVector * spawnables[i].OffsetAmount;

                                //How much along the normal should the object point?
                                float tiltAmount = spawnables[i].SurfaceNormalAmount + (spawnables[i].SpreadNoise[x, y] * 2 - 1) * spawnables[i].PointAlongNormalRandomness;
                                Vector3 finalRotation = LerpToVector(normal, Vector3.up, tiltAmount);
                                //Random rotation based on noise
                                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, finalRotation);

                                GameObject spawnObject = spawnables[i].GetPrefab(x, y);
                                float localRotationAmount = spawnables[i].OffsetNoise[x, y] * DEGREES_360 * spawnables[i].RotationAmount;

                                spawnInfo.Add(new SpawnInfo(spawnObject, objectPosition, rotation, localRotationAmount, chunkCoord, chunkIndex, spawnables[i]. Size != 0, Vector3.one));
                            }
                        }
                    }            
                }
            }
        }

        return spawnInfo;
    }

    //How much should the vector point towards another vector?
    private Vector3 LerpToVector(Vector3 toVector, Vector3 fromVector, float amount)
    {
        amount = amount <= 0 ? 0 : amount;
        return toVector * amount + fromVector * (1 - amount);
    }

    //Returns true if the object can fit in chunk where it is trying to fit
    public bool CanObjectSpawn(int x, int y, int size, float[,] heightMap, float spawnDifferencial, int chunkSize)
    {
        int maxX = x + size < chunkSize - 1 ? x + size : chunkSize - 1;
        int maxY = y + size < chunkSize - 1 ? y + size : chunkSize - 1;

        float currentMin = float.MaxValue;
        float currentMax = float.MinValue;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                if (_occupiedGrid[checkX, checkY])
                    return false;

                if (currentMin > heightMap[checkX, checkY])
                    currentMin = heightMap[checkX, checkY];
                if (currentMax < heightMap[checkX, checkY])
                    currentMax = heightMap[checkX, checkY];
            }
        }

        if (currentMax - currentMin <= spawnDifferencial)
            return true;
        else
            return false;
    }

    //Tells other objects this spot it taken lol
    public void OccupyWithObject(int x, int y, int size, int chunkSize)
    {
        int maxX = x + size < chunkSize - 1 ? x + size : chunkSize - 1;
        int maxY = y + size < chunkSize - 1 ? y + size : chunkSize - 1;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                _occupiedGrid[checkX, checkY] = true;
            }
        }
    }

    public void SpawnSpawnInfo(List<SpawnInfo> spawnInfo, Transform container)
    {
        for (int i = 0; i < spawnInfo.Count; i++)
        {
            spawnInfo[i].Spawn(container);
        }
    }
}

public class SpawnInfo : MonoBehaviour
{
    GameObject _prefab;
    Vector3 _spawnPosition;
    Quaternion _rotation;
    Vector3 _scale;
    float _localRotationAmount;

    Vector2 _chunkCoord;
    Vector2 _itemIndex;

    bool _correctSizeForPickup;

    public SpawnInfo(GameObject prefab, Vector3 spawnPosition, Quaternion rotation, float localRotationAmount, Vector2 chunkCoord, Vector2 itemIndex, bool correctSizeForPickup, Vector3 scale)
    {
        this._prefab = prefab;
        this._spawnPosition = spawnPosition;
        this._rotation = rotation;
        this._scale = scale;
        this._localRotationAmount = localRotationAmount;

        this._chunkCoord = chunkCoord;
        this._itemIndex = itemIndex;

        this._correctSizeForPickup = correctSizeForPickup;
    }

    public void Spawn(Transform container)
    {
        GameObject newObject = Instantiate(_prefab, _spawnPosition, _rotation, container) as GameObject;
        newObject.transform.RotateAround(_spawnPosition, newObject.transform.up, _localRotationAmount);
        newObject.transform.localScale = _scale;

        PrefabSaveData saveDataScript = newObject.GetComponent<PrefabSaveData>();

        if (saveDataScript != null)
        {
            if (_correctSizeForPickup)
                saveDataScript.SetSaveData(new StoredSaveData(_chunkCoord, _itemIndex));
            else
                Debug.LogError("You are trying to spawn a prefab which should be picked up with the wrong size! Size must not be 0 for pickups!!!");
        }
    }
}
