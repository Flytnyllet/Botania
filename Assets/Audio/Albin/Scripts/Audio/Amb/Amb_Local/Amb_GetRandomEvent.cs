using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb_GetRandomEvent : MonoBehaviour
{
    public string amb_List;
    public Amb_Local_Emitter RandomEmitter { get { return _randomEmitter; } }
    private Amb_Local_Emitter _randomEmitter;
    private SphereCollider event_Collider;
    private bool _isWaiting;
    private bool _hasPlayed;

    [SerializeField]
    private Amb_ShyBehaviour amb_ShyBehaviour;
    [SerializeField]
    private Amb_AttachLocalWind amb_AttachLocalWind;

    private bool _debug = false;

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(WaitBeforeChoosing(1.2f));
        }
    }

    private void Update()
    {
        if (!_isWaiting)
        {
            GetRandomEventInstance();
        }
    }

    private void GetRandomEventInstance()
    {
        switch (amb_List)
        {
            case "Amb_Forest":
                _randomEmitter = Amb_Forest.Instance._amb_Forest_List[Random.Range(0, Amb_Forest.Instance.Amb_Forest_Data.Length)];
                break;
            case "Amb_Grassland":
                _randomEmitter = Amb_Grassland.Instance._amb_Grassland_List[Random.Range(0, Amb_Grassland.Instance.Amb_Grassland_Data.Length)];
                break;
        }

        if (!_randomEmitter.IsShy)
        {
            if (amb_ShyBehaviour)
            {
                amb_ShyBehaviour.gameObject.SetActive(false);
            }
        }
        SetRandomCollider(_randomEmitter);
        _isWaiting = true;
    }

    private void SetRandomCollider(Amb_Local_Emitter emitter)
    {
        event_Collider = gameObject.AddComponent<SphereCollider>();
        event_Collider.isTrigger = true;
        event_Collider.center = Vector3.zero;
        event_Collider.radius = emitter.MaxDistance;        //emitter.MaxDistance
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }

        if (amb_AttachLocalWind != null)
            amb_AttachLocalWind.gameObject.SetActive(true);
        if (_randomEmitter.IsShy)
            amb_ShyBehaviour.gameObject.SetActive(true);

        if (!_randomEmitter.IsPlaying)
        {
            //if (!_hasPlayed)
                _randomEmitter.Attach_Local_Emitter(transform, GetComponent<Rigidbody>());
            _randomEmitter.Start_Local_Emitter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }

        amb_AttachLocalWind.gameObject.SetActive(false);

        if (_randomEmitter.IsPlaying)
        {
            _randomEmitter.Stop_Local_Emitter();
            amb_ShyBehaviour.gameObject.SetActive(false);

            if (!_randomEmitter.IsPlaying)
            {
               // *****CHOOSE NEW SOUND??? *****//
               //Destroy(event_Collider);
               //StartCoroutine(WaitBeforeChoosing(4));


                //_hasPlayed = true;
            }
        }
    }

    private IEnumerator WaitBeforeChoosing(float waitTime)
    {
        _isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        _isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_randomEmitter == null) { return; }
        else
        {
            if (_debug)
                Gizmos.DrawWireSphere(transform.position, _randomEmitter.MaxDistance);

            if (_randomEmitter.IsPlaying)
            {
                Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
            }
        }
    }
}