using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventNameLibrary
{
    public static readonly string CAMERA_EFFECT_EVENT_NAME = "StartCameraEffect";
    public static readonly string SPEED_INCREASE = "SpeedDistort";
    public static readonly string SUPER_HEARING = "SuperHearing";
    public static readonly string DRINK_POTION = "DrinkPotion";
    public static readonly string INVISSIBLE = "Invissible";     //This is used both for starting and ending right now
    public static readonly string OPEN_BOOK = "OPEN_BOOK";
    public static readonly string CLOSE_BOOK = "CLOSE_BOOK";
    public static readonly string FLIP_PAGE = "FLIP_PAGE";
    public static readonly string START_RAIN = "StartRain";      //Finns i EventHoldern på "Rain" && "Cloud" Objektet i PlayerPrefab
    public static readonly string STOP_RAIN = "StopRain";        //Finns i EventHoldern på "Rain" && "Cloud" Objektet i PlayerPrefab
    public static readonly string LIGHTNING_STRIKE = "LightningStrike";   
    public static readonly string MIST = "MIST";

}
