using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Amb_Local_Wind : MonoBehaviour
{
    public static Amb_Local_Wind Instance;

    [EventRef]
    public string event_Ref;

    [SerializeField]
    private Amb_Wind_Emitter windEmitterL;
    [SerializeField]
    private Amb_Wind_Emitter windEmitterR;
    [SerializeField]
    private Amb_Wind_Emitter windEmitterB;

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
        Init_Local_Wind();
    }

    public void Init_Local_Wind()
    {
        windEmitterL.Init_Event(event_Ref);
        windEmitterR.Init_Event(event_Ref);
        windEmitterB.Init_Event(event_Ref);
    }
}
