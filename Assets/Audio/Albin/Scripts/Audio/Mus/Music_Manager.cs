using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Music_Manager : MonoBehaviour
{
    public static Music_Manager Instance;

    [EventRef]
    public string mus_02_dimma;

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
    }

    public void Stop_All_Music()
    {

    }
}
