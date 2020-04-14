using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] Transform _player;
    // This thing is dumb and just for testing, don't use it in its current state in the game.
    void Update()
    {
        transform.LookAt(_player,-Vector3.up);
    }
}
