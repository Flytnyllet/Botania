using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Amb_FollowListener : MonoBehaviour
{
    private StudioListener listener;
    private SphereCollider amb_FollowCollider;
    private EventRef_Follow[] eventRefs;
    private Quaternion _targetRotation;
    private Quaternion _relativeRotation;
    private float _distance;
    private float _lastDistance;
    private float _followValue;
    private double _d_followValue;
    private float _r_followValue;
    private bool isFollow = false;

    [SerializeField]
    private bool debug = default;

    private void Awake()
    {
        listener = FindObjectOfType<StudioListener>();
        amb_FollowCollider = GetComponent<SphereCollider>();
        eventRefs = GetComponentsInChildren<EventRef_Follow>();
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x * 0.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        isFollow = true;

        foreach (EventRef_Follow event_FollowListener in eventRefs)
            event_FollowListener.Set_Parameter(event_FollowListener.isFollowParameterId, 1);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        isFollow = false;
    }

    private void Update()
    {
        foreach (EventRef_Follow followEvent in eventRefs)
        {
            _distance = Vector3.Distance(followEvent.transform.position, listener.transform.position);

            if (!isFollow)
            {
                if (_distance < followEvent.maxDistance && _distance != _lastDistance)
                {
                    _followValue = Mathf.InverseLerp(followEvent.maxDistance, 0, _distance);
                    _d_followValue = Math.Round(_followValue, 1);
                    _r_followValue = Convert.ToSingle(_d_followValue);
                    _lastDistance = _distance;
                }
                _targetRotation = listener.transform.rotation;
                _relativeRotation = Quaternion.Lerp(followEvent.transform.rotation, _targetRotation, _r_followValue);
                followEvent.transform.rotation = _relativeRotation;
            }

            if (isFollow)
            {
                followEvent.transform.rotation = listener.transform.rotation;
            }

            followEvent.transform.position = amb_FollowCollider.ClosestPoint(listener.transform.position);
            followEvent.Set_Parameter(followEvent.isFollowParameterId, _r_followValue);
        }
    }
}
