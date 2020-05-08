using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Local_Manager : MonoBehaviour
{
    public static Amb_Local_Manager Instance;

    [SerializeField]
    private GameObject amb_01;
    [SerializeField]
    private GameObject amb_02;





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
    }
}
