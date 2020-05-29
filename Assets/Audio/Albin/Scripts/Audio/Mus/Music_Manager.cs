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
    private bool _isOptions;

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

    private bool _triggeredStop = false;

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
        Init_OptionsMusic();
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

    }

    public void Start_MenuMusic()
    {
        Set_GameStart(0);
        startMenu_Instance.start();
    }

    public void Set_GameStart(float startValue)
    {
        startMenu_Instance.setParameterByID(gameStartParameterId, startValue);
    }

    /// ==================== OPTIONS PAUS ===========================

    private void Init_OptionsMusic()
    {
        pauseMenu_Instance = RuntimeManager.CreateInstance(mus_00_paus);
    }

    public void Play_OptionsMusic()
    {
        _isOptions = true;

        if (_isPlaying)
        {
            Pause_TriggerMusic();
        }

        pauseMenu_Instance.start();
    }

    public void Stop_OptionsMusic()
    {
        pauseMenu_Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _isOptions = false;

        if (_isPaused)
        {
            Unpause_TriggerMusic();
        }
    }

    /// ==================== TRIGGER MUSIC ===========================
    
    public void Init_Music(int track)
    {
        if (!_isPlaying && !_isCooldown)
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
                    trigger_Event = mus_02_dimma_5;
                    break;
            }
            trigger_Instance = RuntimeManager.CreateInstance(trigger_Event);
            Play_TriggerMusic();
        }
    }

    private void Play_TriggerMusic()
    {
        trigger_Instance.start();
        trigger_Instance.release();
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            trigger_Instance.getPlaybackState(out _playbackState);
            trigger_Instance.getPaused(out _isPaused);

            if (_playbackState != PLAYBACK_STATE.STOPPED) { return; }
            else if (_triggeredStop)
            {
                _isPlaying = false;
                StartCoroutine(Start_Cooldown(4));
                _triggeredStop = false;
            }
            else
            {
                _isPlaying = false;
                StartCoroutine(Start_Cooldown(Random.Range(30, 60)));
            }
        }
    }

    private void Pause_TriggerMusic()
    {
        if (_isPlaying)
        {
            trigger_Instance.setPaused(true);
        }
    }

    private void Unpause_TriggerMusic()
    {
        if (_isPaused)
        {
            trigger_Instance.setPaused(false);
        }
    }

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
