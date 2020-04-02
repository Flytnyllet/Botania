using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum ObjectType
{
    cube,
    globe
}

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public int SceneId;
    public float posX, posY, posZ;

    public string id;
    public ObjectType objectType;
    public Vector3 pos;
    public Quaternion rotation;
    //yada yada, fyll i
}
