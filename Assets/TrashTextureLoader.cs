using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTextureLoader : MonoBehaviour
{
    public GroundMaterialGenerator materialGenerator;
    int i = 1;

    void Start()
    {
        int ChunkCoordX = Mathf.RoundToInt(transform.position.x / (GetComponent<MeshFilter>().mesh.bounds.size.x));
        int ChunkCoordZ = Mathf.RoundToInt(transform.position.z / (GetComponent<MeshFilter>().mesh.bounds.size.z));

        Vector2 coord = new Vector2(-ChunkCoordX, ChunkCoordZ);
        int size = (int)(GetComponent<MeshFilter>().mesh.bounds.size.x * transform.localScale.x);
        GetComponent<MeshRenderer>().material = materialGenerator.MakeMaterial(size, coord * 10);
    }


    private void Update()
    {

        //i += 1;
        //Vector2 pos = transform.position;
        //pos.x *= i;
        //GetComponent<MeshRenderer>().material = materialGenerator.MakeMaterial(1, pos);

    }
}
