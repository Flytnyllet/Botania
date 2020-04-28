using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_GetRandomEvent : MonoBehaviour
{

    public string amb_List;
    private string[] amb_LocalData;

    [SerializeField]
    private Amb_Data amb_Data = default;

    public string Amb_RandomEvent { get { return _amb_RandomEvent; } }
    private string _amb_RandomEvent;

    private void Awake()
    {
        switch (amb_List)
        {
            case "Amb_Forest":
                amb_LocalData = new string[]
                {
                    amb_Data.bird_rnd_birdsong1,
                    amb_Data.bird_rnd_birdsong2,
                    amb_Data.bird_rnd_birdsong3,
                    amb_Data.bird_rnd_birdsong4,
                    amb_Data.tree_rnd_woodcreak1,
                    amb_Data.tree_rnd_woodcreak2,
                    amb_Data.tree_rnd_woodsnap1,
                    amb_Data.tree_rnd_woodsnap2,
                    amb_Data.insects_rnd_crickets2
                };
                break;
            case "Amb_Grassland":
                amb_LocalData = new string[]
                {
                    amb_Data.insects_rnd_crickets1
                };
                break;
        }
        _amb_RandomEvent = amb_LocalData[Random.Range(0, amb_LocalData.Length)];
    }
}