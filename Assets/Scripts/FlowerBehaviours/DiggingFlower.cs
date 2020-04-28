using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DiggingFlower : MonoBehaviour
{
    //Detta använder animationer med states och kräver controllern "Digging Fllower"
    public enum FlowerState { Idle, Digging, Hidden, Interactable }; //använder nog inte alla
    [SerializeField] PickupFlower _pickupScript;
    FlowerState _flowerState = FlowerState.Idle;
    bool _playerInArea = false;
    [SerializeField] Animator _animator;
    [Tooltip("Skall sättas på objektet som har script och collider för att plockas upp")]
    CapsuleCollider _capsuleCollider;
    SphereCollider _sphereCol;
    [SerializeField] float _hideTime = 3.0f;

    [EventRef]
    public string event_Digging;
    [EventRef]
    public string event_Emerging;

    private void Awake()
    {
        _sphereCol = GetComponent<SphereCollider>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _pickupScript.SetEnabled = false;
        _capsuleCollider.enabled = false; //Se till att bloman inte kan plockas upp innan spelaren har förmågan

    }

    private void OnEnable()
    {
        EventManager.Subscribe("StartInvissible", PlayerInvissible);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe("StartInvissible", PlayerInvissible);
    }
    void PlayerInvissible(EventParameter param)
    {
        _playerInArea = param.boolParam;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !CharacterState.IsAbilityFlagActive(ABILITY_FLAG.INVISSIBLE))
        {
            _playerInArea = true; //I hate this

            if (_flowerState == FlowerState.Idle)
            {
                _animator.Play("Take001"); //Rör sig lite, kallar på animatorn, och gör sig liten.

                RuntimeManager.PlayOneShotAttached(event_Digging, gameObject);

                StartCoroutine(CheckIfAlone());
                _flowerState = FlowerState.Digging;
            }
        }
        else if (other.tag == "Player" && CharacterState.IsAbilityFlagActive(ABILITY_FLAG.INVISSIBLE))
        {
            MakeInteractable();
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
            if (_playerInArea && !CharacterState.IsAbilityFlagActive(ABILITY_FLAG.INVISSIBLE)) time = 0.0f;
            yield return null;
        }
        _animator.Play("Take002");

        RuntimeManager.PlayOneShotAttached(event_Emerging, gameObject);

        //Detta innebär att blomman är i "Idle" när den gräver upp, vilket kan skapa problem i framtiden.
        _flowerState = FlowerState.Idle;
    }

    //Detta är funktionen som ska kallas när en abbility gör blomman användbar, hur detta görs är inte kritiskt i nuläget
    public void MakeInteractable()
    {
        _pickupScript.SetEnabled = true;
        _animator.Play("BaseState");
        _capsuleCollider.enabled = true;
        _flowerState = FlowerState.Interactable;
        Destroy(this);
    }
}