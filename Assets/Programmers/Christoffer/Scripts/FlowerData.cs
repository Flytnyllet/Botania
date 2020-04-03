﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum FlowerType
{
    //Typerna/Namnen på blommorna
}

[System.Serializable]
public class FlowerData : MonoBehaviour
{
    private string id;
    public string Id { get; set; }
    private FlowerType flowerType;
    public FlowerType FlowerTypes { get; set; }
    private Vector3 pos;
    public Vector3 Pos { get; set; }
    private Quaternion rotation;
    public Quaternion Rotation { get; set; }
    //Fyll i all data som måste sparas
}
