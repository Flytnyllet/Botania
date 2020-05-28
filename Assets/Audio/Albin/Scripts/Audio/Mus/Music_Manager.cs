using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Music_Manager : MonoBehaviour
{
    public static Music_Manager Instance;

    [EventRef]
    public string mus_00_meny_0;

    [EventRef]
    public string mus_00_paus_1;

    [EventRef]
    public string mus_01_biome2_2;

    [EventRef]
    public string mus_01_biome3_3;

    [EventRef]
    public string mus_01_biome4_4;

    [EventRef]
    public string mus_02_dimma_5;

    public bool IsSilent { get { return _isSilent; } }
    private bool _isSilent;

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

        Init_MusicPlayer();
    }

    private void Init_MusicPlayer()
    {

    }

    public void Play_Music()
    {

    }

    public void Stop_All_Music()
    {

    }
}
