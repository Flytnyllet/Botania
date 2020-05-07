using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Forest : MonoBehaviour
{
    public static Amb_Forest Instance;

    [SerializeField]
    private Amb_Data _amb_Data;

    public Dictionary<int, Amb_Local_Emitter> _amb_Forest_List = new Dictionary<int, Amb_Local_Emitter>();
    public string[] Amb_Forest_Data { get { return _amb_Forest_Data; } }
    private string[] _amb_Forest_Data;

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

        _amb_Forest_Data = new string[]
        {
            _amb_Data.bird_rnd_birdsong1,
            _amb_Data.bird_rnd_birdsong2,
            _amb_Data.bird_rnd_birdsong3,
            _amb_Data.bird_rnd_birdsong4,
            _amb_Data.tree_rnd_woodcreak1,
            _amb_Data.tree_rnd_woodcreak2,
            _amb_Data.tree_rnd_woodsnap1,
            _amb_Data.tree_rnd_woodsnap2,
            _amb_Data.insects_rnd_crickets2
        };

        Amb_Local_Emitter bird_rnd_birdsong1 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(0, bird_rnd_birdsong1);
        Amb_Local_Emitter bird_rnd_birdsong2 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(1, bird_rnd_birdsong2);
        Amb_Local_Emitter bird_rnd_birdsong3 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(2, bird_rnd_birdsong3);
        Amb_Local_Emitter bird_rnd_birdsong4 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(3, bird_rnd_birdsong4);
        Amb_Local_Emitter tree_rnd_woodcreak1 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(4, tree_rnd_woodcreak1);
        Amb_Local_Emitter tree_rnd_woodcreak2 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(5, tree_rnd_woodcreak2);
        Amb_Local_Emitter tree_rnd_woodsnap1 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(6, tree_rnd_woodsnap1);
        Amb_Local_Emitter tree_rnd_woodsnap2 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(7, tree_rnd_woodsnap2);
        Amb_Local_Emitter insects_rnd_crickets2 = gameObject.AddComponent<Amb_Local_Emitter>();
        _amb_Forest_List.Add(8, insects_rnd_crickets2);

        Init_Amb_Forest_List();
    }

    private void Init_Amb_Forest_List()
    {
        _amb_Forest_List[0].Init_Event(_amb_Forest_Data[0]);
        _amb_Forest_List[1].Init_Event(_amb_Forest_Data[1]);
        _amb_Forest_List[2].Init_Event(_amb_Forest_Data[2]);
        _amb_Forest_List[3].Init_Event(_amb_Forest_Data[3]);
        _amb_Forest_List[4].Init_Event(_amb_Forest_Data[4]);
        _amb_Forest_List[5].Init_Event(_amb_Forest_Data[5]);
        _amb_Forest_List[6].Init_Event(_amb_Forest_Data[6]);
        _amb_Forest_List[7].Init_Event(_amb_Forest_Data[7]);
        _amb_Forest_List[8].Init_Event(_amb_Forest_Data[8]);
    }
}