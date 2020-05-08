using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogChangeBiome : MonoBehaviour
{
    static FogChangeBiome _thisFogChangeBiome;

    [SerializeField, Tooltip("0 => red trees, 1 => birch, 2 => ?, 3 => planes")] Color[] _biomeFogColors;
    [SerializeField, Range(0.01f, 5)] float _checkForBiomeChangeTime = 0.05f;
    [SerializeField, Range(0.01f, 25)] float _changeFogColorTime = 3.0f;

    Timer _checkTimer;
    Timer _changeTimer;

    BiomeTypes _currentBiomeType;
    bool _currentlySet = false;
    Color oldColor;

    private void Awake()
    {
        if (_thisFogChangeBiome == null)
        {
            _thisFogChangeBiome = this;
            _checkTimer = new Timer(_checkForBiomeChangeTime, _checkForBiomeChangeTime);
            _changeTimer = new Timer(_changeFogColorTime);
            oldColor = RenderSettings.fogColor;
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        //Sets current biome
        _checkTimer.Time += Time.deltaTime;

        if (_checkTimer.Expired())
        {
            BiomeTypes thisBiome = Player.GetCurrentBiome();
            if (thisBiome != _currentBiomeType)
            {
                _currentBiomeType = thisBiome;
                _changeTimer.Reset();
                oldColor = RenderSettings.fogColor;
                _currentlySet = false;
            }

            _checkTimer.Reset();
        }

        //Change fog
        if (!_currentlySet)
        {
            _changeTimer.Time += Time.deltaTime;
            RenderSettings.fogColor = Color.Lerp(oldColor, _biomeFogColors[(int)_currentBiomeType], _changeTimer.Ratio());

            if (_changeTimer.Expired())
            {
                _currentlySet = true;
            }
        }
    }
}
