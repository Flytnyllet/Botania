using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupFlower : InteractableSaving, IInteractable
{
    public delegate void onPickUp(string _text, Sprite sprite= null);
    public static event onPickUp onPickUpEvent;

    bool _enabled = true;
    [SerializeField] bool _dissableTriggerafterPickup = false;
    public bool SetEnabled //kommer byta namn
    {
        get { return _enabled; }
        set { _enabled = value; }
    }
    [SerializeField] ItemDataContainer _flowerData;
    [SerializeField] Texture2D _pickupAlpha;

    [Tooltip("Används i fall ett annat objekt än det lokala skall tas bort vid upplockning")]
    [SerializeField] GameObject[] _gameobjectOverload;
    [SerializeField] UnityEvent _pickupAction;
    [SerializeField] float _pickupAnimationTime = 0.2f;
    [SerializeField] float _pickupAnimationForce = 1.0f;

    private string _flowerPickupSound;
    [SerializeField] private Player_Data _player_Data;
    private Player_Emitter _player_Emitter;

    public bool Interact(Transform interactor)
    {
        if (_enabled)
        {

            //Pickup save system
            PickUp();
            //NotificationObject.name = _flowerData.itemName; // For notification system(not needed anymore, but leave it?)
            //NotificationObject.sprite = _flowerData.itemIcon; // For notification system(not needed anymore, but leave it?)
            if (onPickUpEvent != null)
                onPickUpEvent.Invoke(_flowerData.itemName, _flowerData.itemIcon);
            //transform.LookAt(interactor, Vector3.up);
            string debugFlowerNames = "Trying to pick up a " + _flowerData.itemName
                + ". Accepted flower types are: [";
            string[] flowerTypes = FlowerLibrary.GetAllFlowerNames();
            foreach (string flower in flowerTypes)
            {
                debugFlowerNames += flower + ", ";
            }
            debugFlowerNames += "]";

            //Debug.Log(debugFlowerNames);

            FlowerLibrary.IncrementFlower(_flowerData.itemName, 1);
            if (_gameobjectOverload.Length == 0)
            {
                if (_pickupAlpha != null)
                {
                    GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
                    GetComponent<Collider>().enabled = false; //This may not work since there are multiple colliders
                }
                else { Destroy(this.gameObject); }
            }
            else
            {
                foreach (GameObject gObject in _gameobjectOverload)
                {
                    if (_pickupAlpha != null)
                    {
                        gObject.GetComponent<MeshRenderer>().material.SetTexture("_Alpha", _pickupAlpha);
                    }
                    else { Destroy(gObject); }
                }
                GetComponent<Collider>().enabled = false;//This may not work since there are multiple colliders

            }
            if (_dissableTriggerafterPickup) GetComponent<Collider>().enabled = false;
            StartCoroutine(ShakeFlower(interactor, _pickupAnimationTime, _pickupAnimationForce));
            _pickupAction.Invoke();
			AlchemyOrganizer_2.DiscoverRecipes(_flowerData);
            Play_PickupSound(_flowerData.itemName);
            return true; //Doesn't really have a purpose for this
        }
        return false;
    }
    IEnumerator ShakeFlower(Transform interactor, float duration, float force = 1)
    {
        Vector3 direction = new Vector3();
        Vector3 baseRotation = transform.eulerAngles;
        direction.x = interactor.position.z - this.transform.position.z;
        direction.z = this.transform.position.x - interactor.position.x;
        float time = 0;
        float radianMultiplier = Mathf.PI * 2 / duration * 0.1f;
        while (time < duration)
        {
            float tTime = Mathf.SmoothStep(0, Mathf.PI * 2, time * radianMultiplier);
            this.transform.eulerAngles = baseRotation + direction.normalized * Mathf.Sin(tTime) * force;
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void Play_PickupSound(string flowerName)
    {
        switch (flowerName)
        {
            case "Calm":
                return;
            case "Earth Flower":
                _flowerPickupSound = _player_Data.p_pickup_01_earth;
                break;
            case "Home":
                return;
            case "Magic":
                return;
            case "Mole":
                _flowerPickupSound = _player_Data.p_pickup_05_mole;
                break;
            case "Soul":
                _flowerPickupSound = _player_Data.p_pickup_06_soul;
                break;
            case "Teleporter":
                _flowerPickupSound = _player_Data.p_pickup_03_tp;
                break;
            case "":
                return;
            default:
                return;
        }
        _player_Emitter = Player.FindObjectOfType<Player_Emitter>();
        _player_Emitter.Init_Pickup(_flowerPickupSound);
    }
}
