using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _thisGameManager;

    private void Awake()
    {
        if (_thisGameManager == null)
        {
            _thisGameManager = this;
        }
        else
            Destroy(gameObject);
    }
}
