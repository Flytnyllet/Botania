using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTextureLoader : MonoBehaviour
{
    public GroundMaterialGenerator materialGenerator;

    void Start()
    {
        GetComponent<MeshRenderer>().material = materialGenerator.MakeMaterial(transform.position);
    }

}
