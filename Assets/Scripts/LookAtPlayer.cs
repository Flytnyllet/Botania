using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform _target;
    private void Start()
    {
        _target = Player.GetPlayerTransform();
    }
    // This thing is dumb and just for testing, don't use it in its current state in the game.
    void FixedUpdate()
    {
        transform.LookAt(_target, -Vector3.up);
    }
}
