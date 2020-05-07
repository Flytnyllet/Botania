using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Grassland : MonoBehaviour
{
    public static Amb_Grassland Instance;

    [SerializeField]
    private Amb_Data _amb_Data;

    public Dictionary<int, Amb_Local_Emitter> _amb_Grassland_List = new Dictionary<int, Amb_Local_Emitter>();
    public string[] Amb_Grassland_Data { get { return _amb_Grassland_Data; } }
    private string[] _amb_Grassland_Data;

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

        _amb_Grassland_Data = new string[]
        {
            _amb_Data.insects_rnd_crickets1,
            _amb_Data.insects_rnd_crickets2
        };

        Amb_Local_Emitter insects_rnd_crickets1 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Grassland_List.Add(0, insects_rnd_crickets1);
        Amb_Local_Emitter insects_rnd_crickets2 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Grassland_List.Add(1, insects_rnd_crickets2);

        Init_Amb_Grassland_List();
    }

    private void Init_Amb_Grassland_List()
    {
        _amb_Grassland_List[0].Init_Event(_amb_Grassland_Data[0]);
        _amb_Grassland_List[1].Init_Event(_amb_Grassland_Data[1]);

    }
}