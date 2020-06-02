using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    static readonly string PLAYER_INTERACT_ACTION = "Fire1";

    static GameObject _log;
    static GameObject _logEntry;

    [Header("Looking at Pickup Settings")]

    [SerializeField] Image _defaultLookImage;
    [SerializeField] Image _hoverOverPickupImage;

    [Header("Other")]

    [SerializeField] GameObject inventory;
    [SerializeField] float distance = 100;
    [SerializeField] public KeyCode interactKey = KeyCode.E;
    [SerializeField] LayerMask _layerMask;
    void Awake()
    {
        //FlowerLibrary.InitiateLibrary(); // OBS! Do not comment, SUPER important.
    }

    void Start()
    {
        // === Flower Segment ===
        // ==========================================
        // This is debugging
        /*List<string> flowerNames = FlowerLibrary.GetAllFlowerNames();
		string debugFlowerNames = "All flower names: ";
		foreach (string name in flowerNames)
		{
			debugFlowerNames += name + " |";
		}
		Debug.Log(debugFlowerNames);*/
        // ===========================================

        // === Load Log Stuff ===
        // WARNING! No other objects can have the names "Log" and "LogEntry" at the start of a scene.

        //Har avaktiverat logging tills vidare då det inte är konfirmerat add det skall finnas ett logg fönster i spelet
        //Om vi använder det senare föreslåt jag att vi använder eventManager för kommunikationen

        //_log = GameObject.Find("Log");
        //_logEntry = GameObject.Find("LogEntry");
        //if (_log == null || _logEntry == null)
        //{
        //	Debug.LogError("Missing 'Log' or 'LogEntry' GameObject");
        //}

        //Debug.Log("Log Object = " + _log.name);
        //Debug.Log("LogEntry Object = " + _logEntry.name);
    }

    void Update()
    {
        RaycastHit collision;
        bool hit = Physics.Raycast(transform.position, transform.forward * distance, out collision, distance, _layerMask.value);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distance, Color.green, 1f);

		if (hit && transform.eulerAngles.x < 180)
		{

			//Debug.Log(collision.transform.name);
			PickupFlower[] pickupFlowers = collision.transform.GetComponents<PickupFlower>();

			if (FPSMovement.IsSwimming())
			{
				List<PickupFlower> notUnderwater = new List<PickupFlower>();
				for (int i = 0; i < pickupFlowers.Length; i++)
				{
					if (!pickupFlowers[i].IsUnderwater())
					{
						notUnderwater.Add(pickupFlowers[i]);
					}
				}
				if (notUnderwater.Count < 1)
				{
					ChangeLookingCursor(false);
					return;
				}
				else
				{
					pickupFlowers = notUnderwater.ToArray();
				}
			}

			//Currently looking at an interactable
			bool lookingAtFlower = false;
			for (int i = 0; i < pickupFlowers.Length; i++)
			{
				if (pickupFlowers[i].SetEnabled)
				{
					lookingAtFlower = true;
					break;
				}
			}

			ChangeLookingCursor(lookingAtFlower);

			if (Input.GetButtonDown(PLAYER_INTERACT_ACTION))
			{
				IInteractable[] interactables = collision.transform.GetComponents<IInteractable>();
				Debug.Log(collision.transform.name);

				for (int i = 0; i < interactables.Length; i++)
				{
					interactables[i].Interact(this.transform);
				}
			}
		}
		else
		{
			ChangeLookingCursor(false);
		}
    }

    void ChangeLookingCursor(bool lookingAtPickup)
    {
        if (_defaultLookImage.gameObject.activeSelf == lookingAtPickup)
        {
            _defaultLookImage.gameObject.SetActive(!lookingAtPickup);
            _hoverOverPickupImage.gameObject.SetActive(lookingAtPickup);
        }
    }

    public static void AddLogEntry(string entry)
    {
        Debug.Log("Test");
        GameObject newEntry = Instantiate<GameObject>(_logEntry, _log.transform);
        //RectTransform entryRect = newEntry.GetComponent<RectTransform>();
        try { newEntry.GetComponent<Text>().text = entry; }
        catch (UnityException e) { Debug.LogError("Missing Text component on 'LogEntry' object, leading to an error: \n" + e.Message); }
        Destroy(newEntry, 3f);
    }
}
