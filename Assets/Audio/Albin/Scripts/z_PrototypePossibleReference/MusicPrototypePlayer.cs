using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicPrototypePlayer : MonoBehaviour
{
    [EventRef]
    public string event_Ref;

    private void Start()
    {
        RuntimeManager.PlayOneShot(event_Ref, transform.position);
    }

}
