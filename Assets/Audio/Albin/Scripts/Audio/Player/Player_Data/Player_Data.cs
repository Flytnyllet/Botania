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
    public string p_mov_land = null;

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
    public string p_pickup_calm = null;

    [FMODUnity.EventRef]
    public string p_pickup_earth = null;

    [FMODUnity.EventRef]
    public string p_pickup_home = null;

    [FMODUnity.EventRef]
    public string p_pickup_invisible = null;

    [FMODUnity.EventRef]
    public string p_pickup_mole = null;

    [FMODUnity.EventRef]
    public string p_pickup_soul = null;

    [FMODUnity.EventRef]
    public string p_pickup_tp = null;

    [FMODUnity.EventRef]
    public string p_pickup_vitsippa = null;

    [FMODUnity.EventRef]
    public string p_pickup_levitation = null;

    [FMODUnity.EventRef]
    public string p_pickup_sight = null;

    [FMODUnity.EventRef]
    public string p_pickup_magic = null;

    [FMODUnity.EventRef]
    public string p_pickup_underwater = null;

    [FMODUnity.EventRef]
    public string p_pickup_water = null;
}

