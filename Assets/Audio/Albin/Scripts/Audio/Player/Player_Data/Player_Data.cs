using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Data/Player_Data", fileName = "Player_Data_Asset")]
public class Player_Data : UnityEngine.ScriptableObject
{
    [Header("P_Mov")]
    [FMODUnity.EventRef]
    public string p_mov_rnd_footsteps = null;

    [Space(10)]

    [Header("P_Book")]
    [FMODUnity.EventRef]
    public string p_book_close = null;

    [FMODUnity.EventRef]
    public string p_book_open = null;

    [FMODUnity.EventRef]
    public string p_book_page = null;
}

