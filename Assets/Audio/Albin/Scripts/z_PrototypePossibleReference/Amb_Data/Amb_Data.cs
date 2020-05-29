using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Amb_Data", fileName = "Amb_Data_Asset")]
public class Amb_Data : ScriptableObject
{
    [Header("Amb_Local_Wind")]
    [FMODUnity.EventRef]
    public string amb_tree1_wind = null;

    [Space(10)]

    [Header("Amb_Forest")]

    [FMODUnity.EventRef]
    public string bird_rnd_birdsong1 = null;

    [FMODUnity.EventRef]
    public string bird_rnd_birdsong2 = null;

    [FMODUnity.EventRef]
    public string bird_rnd_birdsong3 = null;

    [FMODUnity.EventRef]
    public string bird_rnd_birdsong4 = null;

    [Space(10)]

    [FMODUnity.EventRef]
    public string tree_rnd_woodcreak = null;

    [FMODUnity.EventRef]
    public string tree_rnd_woodsnap = null;

    [Space(10)]

    [Header("Amb_Grassland")]

    [FMODUnity.EventRef]
    public string insects_rnd_crickets1 = null;

    [FMODUnity.EventRef]
    public string insects_rnd_crickets2 = null;
}

