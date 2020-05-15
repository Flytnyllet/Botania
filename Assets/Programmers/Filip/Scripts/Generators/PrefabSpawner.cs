using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    static readonly float SPAWNING_DELAY_MULTIPLIER = 1.1f;
    static readonly int DEGREES_360 = 360;
    static readonly float STANDARD_GRID_OFFSET = 0.5f;

    bool _readyToFixNormals = false;
    bool[,] _occupiedGrid;
    List<SpawnInfo> _gameObjectsInChunkWithNoNormals = new List<SpawnInfo>();

    public List<SpawnInfo> SpawnOnChunk(int detailType, int levelOfDetail, Biome biome, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 chunkPosition, Vector2 chunkCoord)
    {
        if (_occupiedGrid == null)
            _occupiedGrid = new bool[meshSettings.ChunkSize - 1, meshSettings.ChunkSize - 1];

        biome.Setup(chunkPosition, detailType);

        switch(detailType)
        {
            case (0): { return SpawnFromSpawnables(detailType, levelOfDetail, biome, biome.HighLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, false); }
            case (1): { return SpawnFromSpawnables(detailType, levelOfDetail, biome, biome.MediumLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, false); }
            case (2): { return SpawnFromSpawnables(detailType, levelOfDetail, biome, biome.LowLODSpawnable, heightMap, meshData, meshSettings, chunkPosition, chunkCoord, true); }
            default: throw new System.Exception("There should be no levelOfDetail of " + detailType);
        }
    }

    private List<SpawnInfo> SpawnFromSpawnables(int detailType, int levelOfDetail, Biome biome, Spawnable[] spawnables, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 chunkPosition, Vector2 chunkCoord, bool firstCall = false)
    {
        List<SpawnInfo> spawnInfo = new List<SpawnInfo>();

        //Spawn Water - water has detail type of highest level even if it spawns in low LOD because it has no need to check normals
        if (firstCall)
            spawnInfo.Add(new SpawnInfo(biome.WaterChunk, 0, new Vector3(chunkPosition.x, biome.WaterHeight, -chunkPosition.y), Vector3.up, 0, 0, chunkCoord, Vector2.zero, false, new Vector3((meshSettings.ChunkSize - 1) * meshSettings.MeshScale, 1, (meshSettings.ChunkSize - 1) * meshSettings.MeshScale), false));

        for (int i = 0; i < spawnables.Length; i++)
        {
            //First spawn the subspawners prefabs as they are harder to make room for
            if (spawnables[i].SubSpawners.Length > 0)
            {
                List<SpawnInfo> childSpawnInfo = SpawnFromSpawnables(detailType, levelOfDetail, biome, spawnables[i].SubSpawners, heightMap, meshData, meshSettings, chunkPosition, chunkCoord);
                spawnInfo.AddRange(childSpawnInfo);
            }

            //This spawnable has nothing to spawn, act only as parent
            if (!spawnables[i].ParentOnly)
            {
                //Get noise specific to this prefab
                float[,] spawnNoise = spawnables[i].GetNoise;
                //Local size occupation
                bool[,] localOccupiedGrid = new bool[_occupiedGrid.GetLength(0), _occupiedGrid.GetLength(1)];

                //+ 1 offset in loops are due to border around mesh
                for (int x = 0; x < meshSettings.ChunkSize - spawnables[i].Size; x++)
                {
                    for (int y = 0; y < meshSettings.ChunkSize - spawnables[i].Size; y++)
                    {
                        Vector2 itemIndex = new Vector2(x, y);
                        ChunkCoordIndex chunkCoordIndex = new ChunkCoordIndex(chunkCoord, itemIndex);
                        bool shouldSpawn = true;
                        bool partialSpawn = false;

                        //This thing is already picked up! (Size > 0 is just to check if the thing is pickable)
                        if (!spawnables[i].OthersCanSpawnInside && PrefabSpawnerSaveData.ContainsChunkCoordIndex(chunkCoordIndex))
                        {
                            StoredSaveData data = PrefabSpawnerSaveData.GetStoredSaveData(chunkCoordIndex);

                            if (data.PartialSpawn)
                                partialSpawn = true;
                            else
                                shouldSpawn = false;
                        }
                        if (shouldSpawn)
                        {
                            bool canObjectSpawnSize = CanObjectSpawnSize(x, y, spawnables[i].Size, meshSettings.ChunkSize, ref localOccupiedGrid) && (CanObjectSpawnSize(x, y, spawnables[i].Spacing, meshSettings.ChunkSize, ref _occupiedGrid) || spawnables[i].OthersCanSpawnInside);
                            bool canObjectSpawnDiff = CanObjectSpawnDiff(x, y, spawnables[i].Size, spawnables[i].OthersCanSpawnInside, heightMap.heightMap, spawnables[i].SpawnDifferencial, meshSettings.ChunkSize);

                            //No use in checking if it can spawn if that square is occopied
                            if (canObjectSpawnSize && canObjectSpawnDiff)
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

                                SpawnablePrefab spawnPrefab = spawnables[i].GetPrefab(x, y);

                                bool shouldSpawnIfFixedHeight = true;
                                if (spawnables[i].SpawnFixedHeight)
                                    shouldSpawnIfFixedHeight = heightMap.heightMap[x + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size) + 1, y + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size) + 1] + spawnPrefab.Height <= spawnPrefab.FixedHeight + spawnPrefab.Height;

                                //Things inside the if statement only need to be determined if it should spawn
                                if (insideNoise && gradientSpawn && uniformSpread && noiseSpread && minHeight && maxHeight && minSlope && maxSlope && shouldSpawnIfFixedHeight)
                                {
                                    //Since the object can spawn, mark it's space as occopied
                                    if (!spawnables[i].OthersCanSpawnInside)
                                        OccupyWithObject(x, y, spawnables[i].Size, meshSettings.ChunkSize, ref _occupiedGrid);

                                    OccupyWithObject(x, y, spawnables[i].Spacing, meshSettings.ChunkSize, ref localOccupiedGrid);

                                    float scale = spawnPrefab.ScaleRandom * spawnables[i].OffsetNoise[x, y] + spawnPrefab.Scale;
                                    Vector3 newScale = new Vector3(scale, scale, scale);

                                    //Current local positions in x and y in chunk, used only to spawn from
                                    float xPos = x + STANDARD_GRID_OFFSET + (STANDARD_GRID_OFFSET * spawnables[i].Size) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh + STANDARD_GRID_OFFSET corrects it to the right grid position
                                    float zPos = y + STANDARD_GRID_OFFSET + (STANDARD_GRID_OFFSET * spawnables[i].Size) - meshSettings.ChunkSize / 2 - 1; //Due to the border around the mesh + STANDARD_GRID_OFFSET corrects it to the right grid position
                                    float yPos = heightMap.heightMap[x + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size) + 1, y + (int)(STANDARD_GRID_OFFSET * spawnables[i].Size) + 1] + spawnPrefab.Height;

                                    //Used if the object should not follow the world geometry and spawn at fixed height -> spawn in water 
                                    // Only allow it if the object won't spawn under the terrain!
                                    if (spawnables[i].SpawnFixedHeight)
                                        yPos = spawnPrefab.FixedHeight + spawnPrefab.Height;

                                    //Position from grid in world
                                    Vector3 objectPosition = new Vector3((xPos + chunkPosition.x) * meshSettings.MeshScale, yPos, -(zPos + chunkPosition.y) * meshSettings.MeshScale);
                                    //Vector to offset from grid slightly to create less uniform distribution
                                    Vector3 offsetVector = new Vector3(spawnables[i].OffsetNoise[x, y] * 2 - 1, 0.0f, spawnables[i].SpreadNoise[x, y] * 2 - 1);

                                    objectPosition += offsetVector * spawnables[i].OffsetAmount;

                                    //How much along the normal should the object point?
                                    float tiltAmount = spawnables[i].SurfaceNormalAmount + (spawnables[i].SpreadNoise[x, y] * 2 - 1) * spawnables[i].PointAlongNormalRandomness;

                                    float localRotationAmount = spawnables[i].OffsetNoise[x, y] * DEGREES_360 * spawnables[i].RotationAmount;


                                    spawnInfo.Add(new SpawnInfo(spawnPrefab.Prefab, detailType, objectPosition, normal, tiltAmount, localRotationAmount, chunkCoord, itemIndex, spawnables[i].Size != 0, newScale, partialSpawn));
                                }
                            }
                        }
                    }
                }
            }
        }

        return spawnInfo;
    }

    //Returns true if the object can fit in chunk where it is trying to fit depending on difference in highest and lowest point in spawn area
    public bool CanObjectSpawnDiff(int x, int y, int size, bool othersCanSpawnInside, float[,] heightMap, float spawnDifferencial, int chunkSize)
    {
        int maxX = x + size < chunkSize ? x + size : chunkSize;
        int maxY = y + size < chunkSize ? y + size : chunkSize;

        float currentMin = float.MaxValue;
        float currentMax = float.MinValue;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
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

    public bool CanObjectSpawnSize(int x, int y, int size, int chunkSize, ref bool[,] grid)
    {
        int maxX = x + size < chunkSize ? x + size : -1;
        int maxY = y + size < chunkSize ? y + size : -1;

        //Object is trying to fit in on the corner on a chunk -> no room!
        if (maxX == -1 || maxY == -1)
            return false;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                if (grid[checkX, checkY])
                    return false;
            }
        }

        return true;
    }

    //Tells other objects this spot it taken lol
    public void OccupyWithObject(int x, int y, int size, int chunkSize, ref bool[,] grid)
    {
        int maxX = x + size < chunkSize ? x + size : chunkSize;
        int maxY = y + size < chunkSize ? y + size : chunkSize;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                grid[checkX, checkY] = true;
            }
        }
    }
    public void SpawnSpawnInfo(List<SpawnInfo> spawnInfo, Transform container, bool highestLOD)
    {
        for (int i = 0; i < spawnInfo.Count - 1; i++)
        {
            StartCoroutine(SpawnWithDelay(spawnInfo[i], container, i % 2 == 0 ? i : spawnInfo.Count - i, spawnInfo.Count - 1, false));

            if (spawnInfo[i].DetailType != 0)
                _gameObjectsInChunkWithNoNormals.Add(spawnInfo[i]);
        }

        if (spawnInfo.Count > 0)
        {
            if (spawnInfo[spawnInfo.Count - 1].DetailType != 0)
                _gameObjectsInChunkWithNoNormals.Add(spawnInfo[spawnInfo.Count - 1]);

            //This will always be called last and have the longest waiting time, when it is done -> Normals are ready to be set
            StartCoroutine(SpawnWithDelay(spawnInfo[spawnInfo.Count - 1], container, spawnInfo.Count - 1, spawnInfo.Count - 1, highestLOD));
        }
    }

    IEnumerator SpawnWithDelay(SpawnInfo spawnInfo, Transform container, int index, int highest, bool last)
    {
        float value = index / (float)highest;

        yield return new WaitForSecondsRealtime(value * SPAWNING_DELAY_MULTIPLIER);
        spawnInfo.Spawn(container);

        if (last)
        {
            yield return new WaitForSecondsRealtime(value * SPAWNING_DELAY_MULTIPLIER / 2);
            _readyToFixNormals = true;
        }
    }

    public void SetNormals(MeshData meshData, int chunkSize)
    {
        StartCoroutine(WaitToFixNormals(meshData, chunkSize));
    }

    IEnumerator WaitToFixNormals(MeshData meshData, int chunkSize)
    {
        //Wait until everyspawnable is spawned until actually fixing their normals (to avoid fixing a null instance)
        //This works 99 % of the time
        while (!_readyToFixNormals)
            yield return null;

        //Hard check for the 1 % time
        bool valid = false;
        
        while (!valid)
        {
            bool moveOut = true;

            for (int i = 0; i < _gameObjectsInChunkWithNoNormals.Count; i++)
            {
                if (!_gameObjectsInChunkWithNoNormals[i].ReadyToSetNormal())
                {
                    moveOut = false;
                    break;
                }
            }

            valid = moveOut;

            yield return null;
        }

        for (int i = 0; i < _gameObjectsInChunkWithNoNormals.Count; i++)
            _gameObjectsInChunkWithNoNormals[i].SetNormal(meshData, chunkSize);

        //No need for this script anymore
        //Destroy(this);
    }
}

public class SpawnInfo
{
    Transform _spawnedTransform;
    GameObject _prefab;
    Vector3 _spawnPosition;
    Quaternion _rotation;
    Vector3 _scale;
    float _localRotationAmount;

    Vector2 _chunkCoord;

    bool _correctSizeForPickup;

    Vector3 _normal;
    float _tiltAmount;

    public Vector2 ItemIndex { get; set; }
    public int DetailType { get; set; }
    bool PartialSpawn { get; set; }

    public SpawnInfo(GameObject prefab, int detailType, Vector3 spawnPosition, Vector3 normal, float tiltAmount, float localRotationAmount, Vector2 chunkCoord, Vector2 itemIndex, bool correctSizeForPickup, Vector3 scale, bool partialSpawn)
    {
        this._prefab = prefab;
        this._spawnPosition = spawnPosition;
        this._normal = normal;
        this._tiltAmount = tiltAmount;
        this._scale = scale;
        this._localRotationAmount = localRotationAmount;

        this._chunkCoord = chunkCoord;
        this.ItemIndex = itemIndex;
        this.DetailType = detailType;

        this._correctSizeForPickup = correctSizeForPickup;

        this.PartialSpawn = partialSpawn;
    }

    public void SetNormal(MeshData meshData, int chunkSize)
    {
        _normal = meshData.GetNormal((int)ItemIndex.y * (chunkSize) + (int)ItemIndex.x);
        Quaternion newRotation = CalculateRotation();

        _spawnedTransform.rotation = newRotation;
        _spawnedTransform.RotateAround(_spawnPosition, _spawnedTransform.up, _localRotationAmount);
    }

    public bool ReadyToSetNormal()
    {
        return _spawnedTransform != null;
    }

    public void Spawn(Transform container)
    {
        GameObject newObject = GameObject.Instantiate(_prefab, _spawnPosition, CalculateRotation(), container) as GameObject;
        newObject.transform.RotateAround(_spawnPosition, newObject.transform.up, _localRotationAmount);
        newObject.transform.localScale = _scale;

        _spawnedTransform = newObject.transform;

        PrefabSaveData[] saveDataScripts = newObject.GetComponentsInChildren<PrefabSaveData>();

        //Enter this loop only for object dealing with saving
        if (saveDataScripts.Length > 0)
        {
            if (_correctSizeForPickup)
            {
                for (int i = 0; i < saveDataScripts.Length; i++)
                {
                    saveDataScripts[i].SetSaveData(new StoredSaveData(_chunkCoord, ItemIndex));
                }
            }
            else
            {
                Debug.LogError("You are trying to spawn a prefab which should be picked up with the wrong size! Size must not be 0 for pickups!!!: " + _prefab.name);
            }

            //This object has already been picked up in saves!
            if (PartialSpawn)
            {
                InteractableSaving[] scripts = newObject.GetComponentsInChildren<InteractableSaving>();

                if (scripts.Length > 0)
                {
                    for (int i = 0; i < scripts.Length; i++)
                    {
                        scripts[i].PickedUpAlready();
                    }
                }
                else
                    Debug.LogError("This object is saved as to be partially picked but does not have a InteractableSaving script on it!!! : " + newObject.name);
            }
        }
    }

    private Quaternion CalculateRotation()
    {
        Vector3 finalRotation = LerpToVector(_normal, Vector3.up, _tiltAmount);
        return Quaternion.FromToRotation(Vector3.up, finalRotation);
    }

    //How much should the vector point towards another vector?
    private Vector3 LerpToVector(Vector3 toVector, Vector3 fromVector, float amount)
    {
        amount = amount <= 0 ? 0 : amount;
        return toVector * amount + fromVector * (1 - amount);
    }
}
