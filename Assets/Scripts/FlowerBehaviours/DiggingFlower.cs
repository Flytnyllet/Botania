using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingFlower : MonoBehaviour
{
    //Detta använder animationer med states och kräver controllern "Digging Fllower"
    public enum FlowerState { Idle, Digging, Hidden, Interactable };
    FlowerState _flowerState = FlowerState.Idle;
    bool _playerInArea = false;
    [SerializeField] Animator _animator;
    [Tooltip("Skall sättas på objektet som har script och collider för att plockas upp")]
    [SerializeField] CapsuleCollider _capsuleCollider;
    [SerializeField] float _hideTime = 3.0f;

    private void Awake()
    {
        _capsuleCollider.enabled = false; //Se till att bloman inte kan plockas upp innan spelaren har förmågan
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = true; //I hate this

            if (_flowerState == FlowerState.Idle)
            {
                _animator.Play("DigDown"); //Rör sig lite, kallar på animatorn, och gör sig liten.
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
            time += Time.deltaTime;
            if (_playerInArea) time = 0.0f;
            yield return null;
        }
        _animator.Play("DigUpp");
        //Detta innebär att blomman är i "Idle" när den gräver upp, vilket kan skapa problem i framtiden.
        _flowerState = FlowerState.Idle;
    }

    //Detta är funktionen som ska kallas när en abbility gör blomman användbar, hur detta görs är inte kritiskt i nuläget
    public void MakeInteractable()
    {
        _animator.StopPlayback();
        _animator.WriteDefaultValues();
        _capsuleCollider.enabled = true;
        _flowerState = FlowerState.Interactable;
    }
}