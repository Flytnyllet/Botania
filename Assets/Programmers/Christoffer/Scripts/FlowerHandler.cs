using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerHandler : MonoBehaviour
{
    public FlowerType flowerType;
    public FlowerData flowerData;

    public void Start()
    {
        if (string.IsNullOrEmpty(flowerData.Id))
        {
            flowerData.Id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString();
            flowerData.FlowerTypes = flowerType;
            StoreData.current.Flowers.Add(flowerData);
        }
        //Events here what will happen on start
    }

    private void Update()
    {
        //Update when interacted with
    }

    void DestroyThis()
    {
        //Destroy on load/Replace flowers?
    }

}
