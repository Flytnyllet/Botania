using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Local_Wind : MonoBehaviour
{
    public static Amb_Local_Wind Instance;

    [SerializeField]
    private Amb_Data _amb_Data;

    public Dictionary<int, Amb_Wind_Emitter> _amb_Wind_List = new Dictionary<int, Amb_Wind_Emitter>();
    //public List<Amb_Wind_Emitter> _active_Wind_List = new List<Amb_Wind_Emitter>();
    public string[] Amb_Wind_Data { get { return _amb_Wind_Data; } }
    private string[] _amb_Wind_Data;

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

        _amb_Wind_Data = new string[]
        {
            _amb_Data.amb_tree1_wind
        };

        Amb_Wind_Emitter amb_tree1_wind = gameObject.AddComponent<Amb_Wind_Emitter>();
        _amb_Wind_List.Add(0, amb_tree1_wind);

        Init_Amb_Wind_List();
    }

    private void Init_Amb_Wind_List()
    {
        _amb_Wind_List[0].Init_Event(_amb_Wind_Data[0]);
    }
}
