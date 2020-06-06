using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Music_Manager : MonoBehaviour
{
    public static Music_Manager Instance;

    [EventRef]
    public string mus_00_meny;
    private EventInstance startMenu_Instance;
    private PARAMETER_ID gameStartParameterId;

    [EventRef]
    public string mus_00_paus;
    private EventInstance pauseMenu_Instance;
    public bool IsOptions { get { return _isOptions; } }
    private bool _isOptions = default;

    [EventRef]
    public string mus_01_alchemy;
    [EventRef]
    public string mus_01_biome1_1;
    [EventRef]
    public string mus_01_biome2_2;
    [EventRef]
    public string mus_01_biome3_3;
    [EventRef]
    public string mus_01_biome4_4;
    [EventRef]
    public string mus_02_dimma_5;

    private string trigger_Event;
    private EventInstance trigger_Instance;
    private PLAYBACK_STATE _playbackState;

    public bool IsPaused { get { return _isPaused; } }
    private bool _isPaused = default;

    public bool IsPlaying { get { return _isPlaying; } }
    private bool _isPlaying = false;

    public bool IsCooldown { get { return _isCooldown; } }
    private bool _isCooldown = false;

    public bool StartMenu { get { return _startMenu; } }
    private bool _startMenu = default;

    public bool IsIdleMusic { get { return _isIdleMusic; } }
    private bool _isIdleMusic = default;

    public bool PlayOnlyFirstTime { get { return _playOnlyFirstTime; } }
    private bool _playOnlyFirstTime = true;

    private bool _triggeredStop = false;
    private bool _firstDrink = true;
    
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        Init_StartMenuMusic();
        EventManager.Subscribe(EventNameLibrary.DRINK_POTION, Trigger_Alchemy_Music); 
    }

    void Trigger_Alchemy_Music(EventParameter param = null)
    {
        if (_firstDrink)
            Init_Music(7);
        else if (Random.Range(0, 1) > 0.6f)
            Init_Music(7);
    }


    /// ==================== START MENU ===========================
    private void Init_StartMenuMusic()
    {
        startMenu_Instance = RuntimeManager.CreateInstance(mus_00_meny);

        EventDescription gameStartEventDescription;
        startMenu_Instance.getDescription(out gameStartEventDescription);
        PARAMETER_DESCRIPTION gameStartParameterDescription;
        gameStartEventDescription.getParameterDescriptionByName("game_start", out gameStartParameterDescription);
        gameStartParameterId = gameStartParameterDescription.id;

        Start_StartMenuMusic();
    }

    public void Start_StartMenuMusic()
    {
        _startMenu = true;
        Set_GameStart(0);
        startMenu_Instance.start();
    }

    public void Set_GameStart(float startValue)
    {
        startMenu_Instance.setParameterByID(gameStartParameterId, startValue);
    }

    public void Stop_StartMenuMusic()
    {
        startMenu_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Cooldown_Override(0);
    }

    /// ==================== OPTIONS PAUS ===========================

    //private void Init_OptionsMusic()
    //{
    //    pauseMenu_Instance = RuntimeManager.CreateInstance(mus_00_paus);
    //}

    //public void Play_OptionsMusic()
    //{
    //    if (!_startMenu && !_isOptions)
    //    {
    //        _isOptions = true;

    //        if (_isPlaying)
    //        {
    //            Pause_TriggerMusic();
    //        }

    //        pauseMenu_Instance.start();
    //    }
    //}

    //public void Stop_OptionsMusic()
    //{
    //    pauseMenu_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    //    _isOptions = false;

    //    if (_isPaused)
    //    {
    //        Unpause_TriggerMusic();
    //    }
    //}

    //=========================== UPDATE ========================

    private void Update()
    {
        if (_startMenu)
        {
            startMenu_Instance.getPlaybackState(out _playbackState);
            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else { _startMenu = false; }
        }

        if (_isPlaying)
        {
            trigger_Instance.getPlaybackState(out _playbackState);
            trigger_Instance.getPaused(out _isPaused);

            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else if (_triggeredStop)
            {
                if (_isIdleMusic)
                    _isIdleMusic = false;
                _isPlaying = false;
                StartCoroutine(Start_Cooldown(4));
                _triggeredStop = false;
            }
            else
            {
                _isPlaying = false;
                StartCoroutine(Start_Cooldown(Random.Range(60, 120)));
            }
        }
    }

    /// ==================== TRIGGER MUSIC ===========================

    public void Init_Music(int track)
    {
        if (!_isPlaying && !_isCooldown && !_startMenu)
        {
            switch (track)
            {
                case 1:
                    trigger_Event = mus_01_biome1_1;
                    break;
                case 2:
                    trigger_Event = mus_01_biome2_2;
                    break;
                case 3:
                    trigger_Event = mus_01_biome3_3;
                    break;
                case 4:
                    trigger_Event = mus_01_biome4_4;
                    break;
                case 5:
                    if (_playOnlyFirstTime)
                        trigger_Event = mus_02_dimma_5;
                    break;
                case 6:
                    trigger_Event = mus_00_paus;
                    break;
                case 7:
                    trigger_Event = mus_01_alchemy;
                    break;
            }
            trigger_Instance = RuntimeManager.CreateInstance(trigger_Event);
            Play_TriggerMusic();

            if (trigger_Event == mus_02_dimma_5)
                _playOnlyFirstTime = false;

            if (trigger_Event == mus_00_paus)
                _isIdleMusic = true;

            if (trigger_Event == mus_01_alchemy)
                _firstDrink = false;
        }
    }

    private void Play_TriggerMusic()
    {
        trigger_Instance.start();
        trigger_Instance.release();
        _isPlaying = true;
    }

    //private void Pause_TriggerMusic()
    //{
    //    if (_isPlaying)
    //    {
    //        trigger_Instance.setPaused(true);
    //    }
    //}

    //private void Unpause_TriggerMusic()
    //{
    //    if (_isPaused)
    //    {
    //        trigger_Instance.setPaused(false);
    //    }
    //}

    public void Stop_TriggerMusic()
    {
        if (_isPlaying)
        {
            _triggeredStop = true;
            trigger_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public IEnumerator Start_Cooldown(float time)
    {
        _isCooldown = true;
        yield return new WaitForSeconds(time);
        _isCooldown = false;
    }

    public void Cooldown_Override(int set)
    {
        switch (set)
        {
            case 0:
                _isCooldown = false;
                break;
            case 1:
                _isCooldown = true;
                break;
        }
    }
}
