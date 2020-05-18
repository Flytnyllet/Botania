using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_Global_Wind : MonoBehaviour
{
    public static Amb_Global_Wind Instance;

    [EventRef]
    public string event_Ref;
    private EventInstance event_Instance;
    private EventDescription event_Description;
    private bool is3D;

    [ParamRef]
    public string parameter;
    private PARAMETER_DESCRIPTION windIntensityDescription;

    [HideInInspector]
    public PARAMETER_ID ambGlobalOverrideParameterId;

    [SerializeField]
    [Range(0f, 1f)]
    public float wind_IntensityValue;           // kommentera ut ifall WorldState-värde används, kan användas i Debug-syfte

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

        event_Instance = RuntimeManager.CreateInstance(event_Ref);
        event_Description = RuntimeManager.GetEventDescription(event_Ref);
        event_Description.is3D(out is3D);
        if (is3D)
            Init_Attachment(transform, GetComponent<Rigidbody>());

        EventDescription ambGlobalOverrideEventDescription;
        event_Instance.getDescription(out ambGlobalOverrideEventDescription);
        PARAMETER_DESCRIPTION ambGlobalOverrideParameterDescription;
        ambGlobalOverrideEventDescription.getParameterDescriptionByName("amb_global_override", out ambGlobalOverrideParameterDescription);
        ambGlobalOverrideParameterId = ambGlobalOverrideParameterDescription.id;

        RuntimeManager.StudioSystem.getParameterDescriptionByName(parameter, out windIntensityDescription);
    }

    public void Init_Attachment(Transform position, Rigidbody rb)
    {
        RuntimeManager.AttachInstanceToGameObject(event_Instance, position, rb);
    }

    private void Start()
    {
        event_Instance.start();
    }

    private void Update()
    {
        RuntimeManager.StudioSystem.setParameterByID(windIntensityDescription.id, wind_IntensityValue);     //WorldState.Instance.WindSpeed
    }

    public void Set_Parameter(PARAMETER_ID id, float value)
    {
        event_Instance.setParameterByID(id, value);
    }
}