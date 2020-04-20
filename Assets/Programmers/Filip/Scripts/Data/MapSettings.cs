using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MapSettings", menuName = "Generation/MapSettings")]
public class MapSettings : UpdatableData
{
    [SerializeField] MapHeightRegion _heightRegion;
    [SerializeField] MapRegion[] _mapRegions;
    [SerializeField, Range(1, 30)] int _detailLevel = 5;

    public MapHeightRegion HeightRegion { get { return _heightRegion; } private set { _heightRegion = value; } }
    public MapRegion[] MapRegions       { get { return _mapRegions; }   private set { _mapRegions = value; } }
    public int DetailLevel              { get { return _detailLevel; }  private set { _detailLevel = value; } }
}

[System.Serializable]
public class MapHeightRegion
{
    [SerializeField] NoiseSettingsData _noiseData;
    [SerializeField] MapSettingRegion[] _regions;

    public NoiseSettingsData NoiseData { get { return _noiseData; }       private set { _noiseData = value; } }
    public MapSettingRegion[] Regions  { get { return _regions; }         private set { _regions = value; } }
}

[System.Serializable]
public class MapRegion
{
    [SerializeField] NoiseSettingsData _noiseData;
    [SerializeField] Color _color;
    [SerializeField] float _minHeightStart;
    [SerializeField] float _noiseStartPoint;

    public NoiseSettingsData NoiseData { get { return _noiseData; }       private set { _noiseData = value; } }
    public Color Color                 { get { return _color; }           private set { _color = value; } }
    public float MinHeightStart        { get { return _minHeightStart; }  private set { _minHeightStart = value; } }
    public float NoiseStartPoint       { get { return _noiseStartPoint; } private set { _noiseStartPoint = value; } }
}

[System.Serializable]
public struct MapSettingRegion
{
    public Color color;
    public float height;
}
