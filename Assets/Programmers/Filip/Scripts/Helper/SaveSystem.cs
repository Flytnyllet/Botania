using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    static SaveSystem _thisSaveSystem;

    [Header("Settings")]

    [SerializeField, Range(1, 1200)] float _saveIntervalTime = 300;
    [SerializeField] AnimationCurve _saveIconAlphaCurve;
    [SerializeField, Range(0.01f, 5)] float _saveIconTime = 2.5f;
    [SerializeField] bool _intervallSave = false;
    [SerializeField] bool _menuSave = true;
    [SerializeField, Range(0.01f, 200)] float _menuSaveIntervall = 30;
    [SerializeField] bool _save = true;
    [SerializeField] bool _load = true;

    [Header("Setup")]

    [SerializeField] Image _saveIcon;


    Timer _saveTimer;
    Timer _saveIconTimer;
    Timer _menuIntervallTimer;
    bool _canCurrentlyMenuSave = true;

    Color _saveIconStartColor;

    private void Awake()
    {
        if (_thisSaveSystem == null)
        {
            _menuSaveIntervall = _menuSaveIntervall < _saveIconTime ? _saveIconTime : _menuSaveIntervall;

            _thisSaveSystem = this;
            _saveTimer = new Timer(_saveIntervalTime);
            _saveIconTimer = new Timer(_saveIconTime);
            _menuIntervallTimer = new Timer(_menuSaveIntervall);

            _saveIconStartColor = _saveIcon.color;
            ResetIconColor();
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        if (_menuSave)
        {
            _menuIntervallTimer.Time += Time.deltaTime;

            if (_menuIntervallTimer.Expired() && !_canCurrentlyMenuSave)
                _canCurrentlyMenuSave = true;
        }

        if (_intervallSave)
        {
            _saveTimer.Time += Time.deltaTime;

            if (_saveTimer.Expired())
            {
                _saveTimer.Reset();
                Save();
            }
        }
    }

    //Load in stuff from memory, called on startup
    private void Load()
    {
        if (_load)
        {
            PrefabSpawnerSaveData.Load();
            MapGenerator.Load();
        }
    }

    public static void SaveStatic()
    {
        if (_thisSaveSystem._save && _thisSaveSystem._canCurrentlyMenuSave)
        {
            _thisSaveSystem._canCurrentlyMenuSave = false;
            _thisSaveSystem._menuIntervallTimer.Reset();
            _thisSaveSystem.Save();
        }
    }

    //Saves things that should be saved
    private void Save()
    {
        if (_save)
        {
            StartCoroutine(SaveIconAlpha());
            PrefabSpawnerSaveData.Save();
            MapGenerator.Save();
        }
    }

    IEnumerator SaveIconAlpha()
    {
        while (!_saveIconTimer.Expired())
        {
            _saveIconTimer.Time += Time.deltaTime;

            float point = _saveIconAlphaCurve.Evaluate(_saveIconTimer.Ratio());
            Color newColor = _saveIconStartColor;
            newColor.a = point;
            _saveIcon.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        _saveIconTimer.Reset();
    }

    private void ResetIconColor()
    {
        Color noAlpha = _saveIconStartColor;
        noAlpha.a = 0.0f;
        _saveIcon.color = noAlpha;
    }
}
