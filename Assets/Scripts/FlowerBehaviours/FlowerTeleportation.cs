using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTeleportation : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string event_Teleportation;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] PickupFlower _pickupScript;
    [SerializeField] float _teleportationRange;
    [SerializeField] float _heighCheck;
    [SerializeField] FlowerTeleportationTrailMovement _trailObject;
    [SerializeField] ParticleSystem _particles;
    [SerializeField] int _Maxjumps = 3;
    int _jumps = 0;

    Vector3 groundOffset = new Vector3(0, 0.05f, 0);
    Vector3 _objectHeight;
    Vector3 _objectAltitudeOffset;
    CapsuleCollider _capCollider;
    SphereCollider _sphereCollider;
    private void Awake()
    {
        _pickupScript.SetEnabled = false;
        _capCollider = GetComponent<CapsuleCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
        _objectHeight = new Vector3(0, _capCollider.bounds.size.y, 0);
        _objectAltitudeOffset = new Vector3(0, _capCollider.bounds.center.y, 0);
        _capCollider.enabled = false;
    }

    void ReleaseTrailObject(Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(event_Teleportation, gameObject);
        _particles.gameObject.SetActive(true);
        _particles.Emit(20);
        bool trailActive = _trailObject.gameObject.activeSelf;
        _trailObject.gameObject.SetActive(true);
        _trailObject.transform.parent = null;

        pos -= _objectAltitudeOffset;
        _jumps++;
        transform.position = pos + _objectHeight * 0.5f;

        if (!trailActive)
            _trailObject.StartMovement(this.transform);

        if (_Maxjumps <= _jumps)
        {
            _sphereCollider.enabled = false;
            _capCollider.enabled = true;
            _pickupScript.SetEnabled = true;
            _sphereCollider.enabled = false;
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RaycastHit hit;
            int i;
            for (i = 0; i < 100; i++)
            {
                float rand = Random.Range(0, 6.28f);
                Vector2 newPos = _teleportationRange * new Vector2(Mathf.Sin(rand), Mathf.Cos(rand));
                Vector3 position = transform.position;
                position.x += newPos.x;
                position.z += newPos.y;
                // Does the ray intersect any objects excluding the player layer
                bool HIT = Physics.Raycast(position + groundOffset, Vector3.down, out hit, _heighCheck, _layerMask, QueryTriggerInteraction.Ignore);
                if (HIT && hit.transform.tag != "Flower")
                {
                    Vector3 hitPos = hit.point;
                    if (!Physics.CheckCapsule(hitPos + groundOffset, hitPos + _objectHeight * 1.1f, _capCollider.bounds.size.x, 0, QueryTriggerInteraction.Ignore))
                    {
                        ReleaseTrailObject(hitPos);
                        break;
                    }
                    Debug.DrawRay(hitPos, hitPos + _objectHeight, Color.green, 5);
                }
                HIT = Physics.Raycast(position + groundOffset, Vector3.up, out hit, _heighCheck, _layerMask, QueryTriggerInteraction.Ignore);
                if (HIT && hit.transform.tag != "Flower")
                {
                    Vector3 hitPos = hit.point;
                    if (!Physics.CheckCapsule(hitPos + groundOffset, hitPos + _objectHeight * 1.1f, this.transform.localScale.y, 0, QueryTriggerInteraction.Ignore))
                    {
                        ReleaseTrailObject(hitPos);
                        break;
                    }
                    Debug.DrawRay(hitPos, hitPos + _objectHeight, Color.green, 5);
                }
                Debug.DrawRay(position, Vector3.down * _heighCheck, Color.red, 5);
                Debug.DrawRay(position, Vector3.up * _heighCheck, Color.red, 5);
            }
            if (i == 100)
            {
                Debug.LogError("Could not find a teleportation spot in 100 attempts, consider changing the heightCheck variable");
            }
        }
    }
}
