using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private string id;
    public string Id { get; set; }

    private Vector3 pos;
    public Vector3 Pos { get; set; }

    private Quaternion rotation;
    public Quaternion Rotation { get; set; }

    //Fyll i all data som måste sparas (inventory)
}
