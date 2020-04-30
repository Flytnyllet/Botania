using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DiggingFlower : MonoBehaviour
{
    //Detta använder animationer med states och kräver controllern "Digging Fllower"
    public enum FlowerState { Idle, Digging, Hidden, Interactable }; //använder nog inte alla
    FlowerState _flowerState = FlowerState.Idle;
    [SerializeField] PickupFlower _pickupScript;
    bool _playerInArea = false;
    [SerializeField] Animator _animator;
    [Tooltip("Skall sättas på objektet som har script och collider för att plockas upp")]
    CapsuleCollider _capsuleCollider;
    SphereCollider _sphereCol;
    [SerializeField] float _hideTime = 3.0f;
    [SerializeField] string[] _animationId;

    [EventRef]
    public string event_Digging;
    [EventRef]
    public string event_Emerging;

    private void Awake()
    {
        _sphereCol = GetComponent<SphereCollider>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        MakeInteractable();
    }

    //Har inte fixat så att events kan ta imot methoder utan parametrar, så det här känns som den mest eleganta lösningen
    void playerWentInvisible(EventParameter param) { MakeInteractable(); }
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.INVISSIBLE, playerWentInvisible);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.INVISSIBLE, playerWentInvisible);
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
            StartCoroutine(DigAndWait());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = false;
        }
    }

    IEnumerator DigAndWait()
    {
        if (_flowerState == FlowerState.Idle)
        {
            _flowerState = FlowerState.Digging;
            _animator.Play(_animationId[0]); //Rör sig lite, kallar på animatorn, och gör sig liten.

            if(event_Digging != null)
            RuntimeManager.PlayOneShotAttached(event_Digging, gameObject);

            float time = 0.0f;
            while (time < _hideTime)
            {
                time += Time.deltaTime;
                if (_playerInArea && !CharacterState.IsAbilityFlagActive(ABILITY_FLAG.INVISSIBLE)) time = 0.0f;
                yield return null;
            }
            _animator.Play(_animationId[1]);

            if (event_Digging != null)
                RuntimeManager.PlayOneShotAttached(event_Emerging, gameObject);

            //Detta innebär att blomman är i "Idle" när den gräver upp, vilket kan skapa problem i framtiden.
            _flowerState = FlowerState.Idle;
        }
    }

    //Detta är funktionen som ska kallas när en abbility gör blomman användbar, hur detta görs är inte kritiskt i nuläget
    void MakeInteractable()
    {
        if (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.INVISSIBLE))
        {
            _pickupScript.SetEnabled = true;
            //_animator.Play("BaseState");
            _capsuleCollider.enabled = true;
            _sphereCol.enabled = false;
            _flowerState = FlowerState.Interactable;
        }
        else
        {
            _pickupScript.SetEnabled = false;
            _capsuleCollider.enabled = false;
            _sphereCol.enabled = true;
            _flowerState = FlowerState.Idle;
            StartCoroutine(DigAndWait());
        }
    }
}