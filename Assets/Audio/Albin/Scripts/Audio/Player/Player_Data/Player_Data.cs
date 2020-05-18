using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Data/Player_Data", fileName = "Player_Data_Asset")]
public class Player_Data : UnityEngine.ScriptableObject
{
    [Header("P_Mov")]
    [FMODUnity.EventRef]
    public string p_mov_rnd_footsteps = null;

    [FMODUnity.EventRef]
    public string p_mov_jump = null;

    [FMODUnity.EventRef]
    public string p_mov_swim = null;

    [Space(10)]

    [Header("P_Inv")]
    [FMODUnity.EventRef]
    public string p_book_close = null;

    [FMODUnity.EventRef]
    public string p_book_open = null;

    [FMODUnity.EventRef]
    public string p_book_page = null;

    [Space(5)]

    [FMODUnity.EventRef]
    public string p_potion_create = null;

    [FMODUnity.EventRef]
    public string p_potion_teleport = null;

    [Space(10)]

    [Header("P_Pickup")]
    [FMODUnity.EventRef]
    public string p_pickup_01_earth = null;

    [FMODUnity.EventRef]
    public string p_pickup_03_tp = null;

    [FMODUnity.EventRef]
    public string p_pickup_05_mole = null;

    [FMODUnity.EventRef]
    public string p_pickup_06_soul = null;

    [FMODUnity.EventRef]
    public string p_pickup_07_calm = null;
}

