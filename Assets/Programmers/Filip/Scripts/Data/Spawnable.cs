using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawnable", menuName = "Generation/Spawning/Spawnable")]
public class Spawnable : UpdatableData
{
    [Header("Affect by parent")]
    [SerializeField, Tooltip("Should almost always be set to MULTIPLY (area within parent)")] NoiseMergeType _noiseMergeType;


    [Header("Noise settings")]

    [SerializeField, Tooltip("Settings to create the noise the prefab should spawn after")] NoiseSettingsData _noiseSettingsData;


    [Header("General Settings")]

    [SerializeField, Tooltip("If true it will not spawn anything by itself, only act as parent noise")] bool _parentOnly = false;

    [SerializeField, Range(0, 1), Tooltip("How much random rotation should be applied (probably keep it on 1)")] float _rotationAmount = 1.0f;
    [SerializeField, Range(0, 1), Tooltip("At which point in the noise gradient should object start spawning? Low = smooth edges, high = sharp af")] float _noiseStartPoint;
    [SerializeField, Range(0, 2), Tooltip("How thick the area of spawn should be")] float _thickness = 0.75f;
    [SerializeField, Range(1, 80), Tooltip("Uniform spread amount (use for general spreading and not for fine tuning) (low = low spread, high = high spread)")] int _uniformSpreadAmount = 1;
    [SerializeField, Range(0, 1), Tooltip("How much random spawn spread there should be")] float _randomSpread = 0.5f;
    [SerializeField, Range(0, 4), Tooltip("How much should the spawning avert from the grid it is based on? (high values might cause clipping!!!)")] float _offsetAmount = 0.75f;
    [SerializeField, Tooltip("Should the object spawn at certain height?")] bool _spawnFixedHeight = false;

    [Header("Size Settings")]

    [SerializeField, Range(1, 30), Tooltip("How many squares does this object occupy?")] int _size;
    [SerializeField, Tooltip("Do not have this enabled for pickups!!! It will crash!")] bool _othersCanSpawnInside = false;
    [SerializeField, Range(0, 50), Tooltip("How high can the difference between highest and lowest point in spawn area be for it to spawn?")] float _spawnDifferencial;


    [Header("Height Spawn Settings")]

    [SerializeField, Range(0, 100), Tooltip("At which height should objects start spawning? (hard amount)")] float _hardMinHeight = 0;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects start spawning? (soft amount)")] float _softMinAmount = 0;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects stop spawning? (hard amount)")] float _hardMaxHeight = 100;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects stop spawning? (soft amount)")] float _softMaxAmount = 0;
    

    [Header("Slope Spawn Settings")]

    [SerializeField, Range(0, 1), Tooltip("How much should the object point along the surface normal?")] float _surfaceNormalAmount = 1.0f;
    [SerializeField, Range(0, 2), Tooltip("When pointing along normal, how much randomness around it?")] float _pointAlongNormalRandomness;
    [SerializeField, Range(0, 90), Tooltip("At which angle should it kind of stop spawning objects (min)")] float _softMinSlope;
    [SerializeField, Range(0, 90), Tooltip("At which angle should it definitely stop spawning objects (min)")] float _hardMinSlope;
    [SerializeField, Range(0, 90), Tooltip("At which angle should it kind of stop spawning objects (min)")] float _softMaxSlope;
    [SerializeField, Range(0, 90), Tooltip("At which angle should it definitely stop spawning objects (min)")] float _hardMaxSlope;


    [Header("Drop")]

    [SerializeField, Tooltip("Which object should spawn?")] SpawnablePrefab[] _prefabs;
    [SerializeField, Tooltip("Sub areas within this objects noise in which new objects can spawn")] Spawnable[] _subSpawners;

    float[,] _noise;
    float[,] _offsetNoise;
    float[,] _spreadNoise;

    int _prefabMaxProbability;

    public NoiseMergeType NoiseMergeType              { get { return _noiseMergeType; }             private set { _noiseMergeType = value; } }
    public NoiseSettingsData NoiseSettingsData        { get { return _noiseSettingsData; }          private set { _noiseSettingsData = value; } }
    public Spawnable[] SubSpawners                    { get { return _subSpawners; }                private set { _subSpawners = value; } }

    public bool ParentOnly                            { get { return _parentOnly; }                 private set { _parentOnly = value; } }

    public float RotationAmount                       { get { return _rotationAmount; }             private set { _rotationAmount = value; } }
    public float NoiseStartPoint                      { get { return _noiseStartPoint; }            private set { _noiseStartPoint = value; } }
    public float Thickness                            { get { return _thickness; }                  private set { _thickness = value; } }
    public int UniformSpreadAmount                    { get { return _uniformSpreadAmount; }        private set { _uniformSpreadAmount = value; } }
    public float RandomSpread                         { get { return _randomSpread; }               private set { _randomSpread = value; } }
    public float OffsetAmount                         { get { return _offsetAmount; }               private set { _offsetAmount = value; } }
    public bool SpawnFixedHeight                      { get { return _spawnFixedHeight; }           private set { _spawnFixedHeight = value; } }

    public int Size                                   { get { return _size; }                       private set { _size = value; } }
    public bool OthersCanSpawnInside                  { get { return _othersCanSpawnInside; }       private set { _othersCanSpawnInside = value; } }
    public float SpawnDifferencial                    { get { return _spawnDifferencial; }          private set { _spawnDifferencial = value; } }

    public float SoftMinAmount                        { get { return _softMinAmount; }              private set { _softMinAmount = value; } }
    public float HardMinHeight                        { get { return _hardMinHeight; }              private set { _hardMinHeight = value; } }
    public float SoftMaxAmount                        { get { return _softMaxAmount; }              private set { _softMaxAmount = value; } }
    public float HardMaxHeight                        { get { return _hardMaxHeight; }              private set { _hardMaxHeight = value; } }

    public float SurfaceNormalAmount                  { get { return _surfaceNormalAmount; }        private set { _surfaceNormalAmount = value; } }
    public float PointAlongNormalRandomness           { get { return _pointAlongNormalRandomness; } private set { _pointAlongNormalRandomness = value; } }
    public float SoftMinSlope                         { get { return _softMinSlope; }               private set { _softMinSlope = value; } }
    public float HardMinSlope                         { get { return _hardMinSlope; }               private set { _hardMinSlope = value; } }
    public float SoftMaxSlope                         { get { return _softMaxSlope; }               private set { _softMaxSlope = value; } }
    public float HardMaxSlope                         { get { return _hardMaxSlope; }               private set { _hardMaxSlope = value; } }

    public float[,] GetNoise                          { get { return _noise; }                      private set { _noise = value; } }
    public float[,] OffsetNoise                       { get { return _offsetNoise; }                private set { _offsetNoise = value; } }
    public float[,] SpreadNoise                       { get { return _spreadNoise; }                private set { _spreadNoise = value; } }

    //These two functions are only used so different threads don't base on the same data and fs up
    public Spawnable(Spawnable spawnable)
    {
        this._noiseMergeType = spawnable._noiseMergeType;
        this._noiseSettingsData = spawnable._noiseSettingsData;
        this._subSpawners = spawnable._subSpawners;
        this._parentOnly = spawnable._parentOnly;
        this._rotationAmount = spawnable._rotationAmount;
        this._noiseStartPoint = spawnable._noiseStartPoint;
        this._thickness = spawnable._thickness;
        this._uniformSpreadAmount = spawnable._uniformSpreadAmount;
        this._randomSpread = spawnable._randomSpread;
        this._offsetAmount = spawnable._offsetAmount;
        this._spawnFixedHeight = spawnable._spawnFixedHeight;
        this._size = spawnable._size;
        this._othersCanSpawnInside = spawnable._othersCanSpawnInside;
        this._spawnDifferencial = spawnable._spawnDifferencial;
        this._softMinAmount = spawnable._softMinAmount;
        this._hardMinHeight = spawnable._hardMinHeight;
        this._softMaxAmount = spawnable._softMaxAmount;
        this._hardMaxHeight = spawnable._hardMaxHeight;
        this._surfaceNormalAmount = spawnable._surfaceNormalAmount;
        this._pointAlongNormalRandomness = spawnable._pointAlongNormalRandomness;
        this._softMinSlope = spawnable._softMinSlope;
        this._hardMinSlope = spawnable._hardMinSlope;
        this._softMaxSlope = spawnable._softMaxSlope;
        this._hardMaxSlope = spawnable._hardMaxSlope;
        this._noise = spawnable._noise;
        this._offsetNoise = spawnable._offsetNoise;
        this._spreadNoise = spawnable._spreadNoise;
        this._prefabs = spawnable._prefabs;
    }
    public static Spawnable[] CopySpawnables(Spawnable[] spawnables)
    {
        Spawnable[] newSpawnable = new Spawnable[spawnables.Length];
        for (int i = 0; i < spawnables.Length; i++)
        {
            newSpawnable[i] = new Spawnable(spawnables[i]);

            if (newSpawnable[i]._subSpawners != null)
                newSpawnable[i]._subSpawners = CopySpawnables(newSpawnable[i]._subSpawners);
        }

        return newSpawnable;
    }

    // Used to calculate all the different noises for every spawable
    public void Setup(float[,] parentNoise, int chunkSize, NoiseSettingsData offsetNoiseSettings, Vector2 center, Vector2 offsetNoiseOffset)
    {
        if (parentNoise != null)
            _noise = Noise.MergeNoise(chunkSize, chunkSize, 1, _noiseSettingsData.NoiseSettingsDataMerge, parentNoise, _noiseMergeType, new Vector2(center.x, -center.y));
        else
            _noise = Noise.GenerateNoiseMap(chunkSize, chunkSize, 1, _noiseSettingsData.NoiseSettingsDataMerge, new Vector2(center.x, -center.y));

        _noise = Noise.Clamp(_noise, _noiseSettingsData);

        _offsetNoise = Noise.GenerateNoiseMap(chunkSize, chunkSize, 1, offsetNoiseSettings.NoiseSettingsDataMerge, new Vector2(center.x, -center.y) + offsetNoiseOffset);
        _spreadNoise = Noise.GenerateNoiseMap(chunkSize, chunkSize, 1, offsetNoiseSettings.NoiseSettingsDataMerge, new Vector2(center.x, -center.y) + offsetNoiseOffset * 2);

        _prefabMaxProbability = SpawnablePrefab.GetMaxSize(_prefabs);

        for (int i = 0; i < _subSpawners.Length; i++)
        {
            _subSpawners[i].Setup(_noise, chunkSize, offsetNoiseSettings, new Vector2(center.x, -center.y), offsetNoiseOffset * 2);
        }
    }

    //Returns a prefab in the prefab selection based on noise and probability in world position
    public SpawnablePrefab GetPrefab(int x, int y)
    {
        float randomValue = _spreadNoise[x, y] * _prefabMaxProbability;
        int compareValue = 0;

        for (int i = 0; i < _prefabs.Length; i++)
        {
            if (randomValue <= compareValue + _prefabs[i].Probability)
                return _prefabs[i];

            compareValue += _prefabs[i].Probability;
        }

        Debug.LogError("There are no prefabs here! " + this);
        return null;
    }
}

[System.Serializable]
public class SpawnablePrefab
{
    [SerializeField] GameObject _prefab;
    [SerializeField, Range(1, 100), Tooltip("Not based on percent but the overall combined probabilities")] int _probability;
    [SerializeField, Tooltip("At what height should this object spawn relative to it's origin?")] float _height = 0.5f;
    [SerializeField, Tooltip("If spawn at fixed height, which height?")] float _fixedHeight = 0.0f;
    [SerializeField, Range(0, 10)] float _scale = 1.0f;
    [SerializeField, Range(0, 10), Tooltip("How much should the objects differ in scale?")] float _scaleRandom = 0.0f;

    public GameObject Prefab { get { return _prefab; }      private set { _prefab = value; } }
    public int Probability   { get { return _probability; } private set { _probability = value; } }
    public float Height { get { return _height; } private set { _height = value; } }
    public float FixedHeight { get { return _fixedHeight; } private set { _fixedHeight = value; } }
    public float Scale { get { return _scale; } private set { _scale = value; } }
    public float ScaleRandom { get { return _scaleRandom; } private set { _scaleRandom = value; } }

    //Return total combined probability
    public static int GetMaxSize(SpawnablePrefab[] spawnablePrefabs)
    {
        int size = 0;
        for (int i = 0; i < spawnablePrefabs.Length; i++)
        {
            size += spawnablePrefabs[i].Probability;
        }

        return size;
    }
}

/* BIOME 1
 
    Biome_1_LOD_0 (NONE)
        
    Biome_1_LOD_1 (NONE)
        - Trees (NONE)
            - Rare-0
            - Soul
            - Mole
            - Teleport

    Biome_1_LOD_2 (NONE)
        - Trees

*/
/* BIOME 2
   Biome_2_LOD_0(NONE)

   Biome_2_LOD_1 (NONE)

   Biome_2_LOD_2 (NONE)
*/
/* BIOME 3
   Biome_3_LOD_0(NONE)

   Biome_3_LOD_1 (NONE)

   Biome_3_LOD_2 (NONE)
*/
/* BIOME ZERO
   Biome_ZERO_LOD_0(NONE)

   Biome_ZERO_LOD_1 (NONE)
       - Earth flower

   Biome_ZERO_LOD_2 (NONE)
*/
/* NONE
   Biome_NONE_LOD_0(NONE)
       - Grass High LOD    

   Biome_NONE_LOD_1 (NONE)
       - Seaweed
       - Beach
       - Bord och stolar
       - Grass Low LOD

   Biome_NONE_LOD_2 (NONE)
*/
