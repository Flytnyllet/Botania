using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    static SaveSystem _thisSaveSystem;

    public static bool Ready { get; private set; } = false;

    [Header("Settings")]

    [SerializeField] string _animatorTriggerName = "Trigger";

    [SerializeField, Range(1, 1200)] float _saveIntervalTime = 300;
    [SerializeField] AnimationCurve _saveIconAlphaCurve;
    [SerializeField, Range(0.01f, 20)] float _saveIconTime = 2.5f;
    [SerializeField] bool _intervallSave = false;
    [SerializeField] bool _menuSave = true;
    [SerializeField, Range(0.01f, 200)] float _menuSaveIntervall = 30;
    [SerializeField] bool _save = true;
    [SerializeField] bool _load = true;

    [SerializeField, Range(0, 15)] int _clearAreaSize = 3;

    [Header("Setup")]

    [SerializeField] Image _saveIcon;
    [SerializeField] MeshSettings _meshSettings;

    Animator _animator;

    Timer _saveTimer;
    Timer _saveIconTimer;
    Timer _menuIntervallTimer;
    bool _canCurrentlyMenuSave = true;

    Color _saveIconStartColor;

    private void Awake()
    {
        if (_thisSaveSystem == null)
        {
            _saveIntervalTime = _saveIntervalTime < _saveIconTime ? _saveIconTime : _saveIntervalTime;
            _menuSaveIntervall = _menuSaveIntervall < _saveIconTime ? _saveIconTime : _menuSaveIntervall;

            _animator = GetComponent<Animator>();

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
            ValidateSave();
            Noise.SetSeed(GetWorldSeed());
            Player.Load();
            PrefabSpawnerSaveData.Load();
            MapGenerator.Load();
        }

        //Used to make clear circle around player on spawn
        PrefabSpawnerSaveData.ClearStartArea(_meshSettings.NumVertsPerLine / 2, _clearAreaSize);

        Ready = true;
    }

    void ValidateSave()
    {
        object seed = Serialization.Load(Saving.FileNames.SEED);

        if (seed == null)
        {
            PrefabSpawnerSaveData.Wipe();
            MapGenerator.Wipe();
        }
    }

    int GetWorldSeed()
    {
        object seed = Serialization.Load(Saving.FileNames.SEED);

        if (seed == null)
            return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        else
            return (int)seed;
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
            Noise.Save();
            PrefabSpawnerSaveData.Save();
            Player.Save();
            MapGenerator.Save();
        }
    }

    IEnumerator SaveIconAlpha()
    {
        _animator.SetTrigger(_animatorTriggerName);

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
