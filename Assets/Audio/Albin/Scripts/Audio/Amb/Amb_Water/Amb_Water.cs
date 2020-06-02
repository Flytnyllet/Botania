using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Water : MonoBehaviour
{
    [SerializeField] private GameObject inWater;
    [SerializeField] private GameObject nearWater;
    [SerializeField] private GameObject waves;
    [SerializeField] private GameObject underwater;

    private void Update()
    {
        if (transform.position.y < 9.0f)
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(false);
            underwater.SetActive(true);
        }
        else if (transform.position.y > 9.0f && transform.position.y <= 9.6)
        {
            inWater.SetActive(true);
            waves.SetActive(true);
            nearWater.SetActive(false);
            underwater.SetActive(false);
        }
        else if (transform.position.y < 10.8)
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(true);
            underwater.SetActive(false);
        }
        else
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(false);
            underwater.SetActive(false);
        }
    }
}
