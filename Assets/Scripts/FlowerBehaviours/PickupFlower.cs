using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupFlower : InteractableSaving, IInteractable
{
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

    public bool Interact(Transform interactor)
    {
        if (_enabled)
        {
            //Pickup save system
            PickUp();
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
        float radianMultiplier = Mathf.PI * 2 / duration;
        while (time < duration)
        {
            this.transform.eulerAngles = baseRotation + direction * Mathf.Sin(time * radianMultiplier) * force;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
