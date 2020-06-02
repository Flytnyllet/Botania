using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_Water : MonoBehaviour
{
    [SerializeField] private GameObject inWater;
    [SerializeField] private GameObject nearWater;
    [SerializeField] private GameObject waves;
    [SerializeField] private GameObject underwater;

    private float _playerWas = 1;
    private float _lastPlayerWas;
    private bool _hasPlayed = false;

    private void Update()
    {
        if (transform.position.y < 9.35f)
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(false);
            underwater.SetActive(true);
            if (_playerWas == 1)
            {
                Debug.Log("playerWas was reset. 1 > 0");
                _hasPlayed = false;
            }
            _playerWas = 0;

        }
        else if (transform.position.y > 9.35f && transform.position.y <= 9.6)
        {
            inWater.SetActive(true);
            waves.SetActive(true);
            nearWater.SetActive(false);
            underwater.SetActive(false);
            if (_playerWas == 0)
            {
                Debug.Log("playerWas was reset. 0 > 1");
                _hasPlayed = false;
            }
            _playerWas = 1;
        }
        else if (transform.position.y < 10.8)
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(true);
            underwater.SetActive(false);
            _playerWas = 1;
        }
        else
        {
            inWater.SetActive(false);
            waves.SetActive(false);
            nearWater.SetActive(false);
            underwater.SetActive(false);
            _playerWas = 1;
        }
    }
}
