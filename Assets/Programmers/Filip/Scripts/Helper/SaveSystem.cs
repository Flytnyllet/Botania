using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    static SaveSystem _thisSaveSystem;

    [SerializeField, Range(1, 1200)] float _saveIntervalTime = 300;
    [SerializeField] bool _save = true;
    [SerializeField] bool _load = true;

    Timer _saveTimer;

    private void Awake()
    {
        if (_thisSaveSystem == null)
        {
            _thisSaveSystem = this;
            _saveTimer = new Timer(_saveIntervalTime);
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
        _saveTimer.Time += Time.deltaTime;

        if (_saveTimer.Expired())
        {
            _saveTimer.Reset();
            Save();
        }
    }

    private void Load()
    {
        if (_load)
            MapGenerator.Load();
    }

    private void Save()
    {
        if (_save)
            MapGenerator.Save();
    }
}
