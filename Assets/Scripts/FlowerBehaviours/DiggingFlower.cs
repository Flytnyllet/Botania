using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingFlower : MonoBehaviour, Interactable
{
    //Detta använder animationer med states och kräver controllern "Digging Fllower"
    public enum FlowerState { Idle, Digging, Hidden, Interactable };
    FlowerState _flowerState = FlowerState.Idle;
    bool _playerInArea = false;
    [SerializeField] Animator _animator;

    [SerializeField] float _hideTime = 3.0f;



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = true; //I hate this

            if (_flowerState == FlowerState.Idle)
            {
                _animator.Play("DigDown");
                StartCoroutine(CheckIfAlone());
                _flowerState = FlowerState.Digging;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = false;
        }
    }

    IEnumerator CheckIfAlone()
    {
        float time = 0.0f;
        while (time < _hideTime)
        {
            Debug.Log(_playerInArea);
            time += Time.deltaTime;
            if (_playerInArea) time = 0.0f;
            yield return null;
        }
        _animator.Play("DigUpp");
        //Detta innebär att blomman är i "Idle" när den gräver upp, vilket kan skapa problem i framtiden.
        _flowerState = FlowerState.Idle;
    }

    //Detta är funktionen som ska kallas när en abbility för blomman användbar
    public void MakeInteractable()
    {
        _animator.StopPlayback();
        _animator.WriteDefaultValues();
        _flowerState = FlowerState.Interactable;
    }

    public bool Interact()
    {
        if (_flowerState == FlowerState.Interactable)
        {

            return true;
        }
        return false;
    }
}

