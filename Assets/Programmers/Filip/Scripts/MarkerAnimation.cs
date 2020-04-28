using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerAnimation : MonoBehaviour
{
    static readonly string ANIMATOR_TYPE_STRING = "MarkerType";

    int _animationIndex = int.MinValue;

    public void Setup(int index)
    {
        _animationIndex = index;
    }

    private void Update()
    {
        if (_animationIndex != int.MinValue)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("This marker does not have an animator!?");
            else
            {
                animator.SetInteger(ANIMATOR_TYPE_STRING, _animationIndex);
                Destroy(this);
            }
        }
    }
}
