using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTeleportation : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _teleportationRange;
    [SerializeField] float _heighCheck;
    [SerializeField] FlowerTeleportationTrailMovement _trailObject;
    [SerializeField] ParticleSystem _particles;

    Vector3 groundOffset = new Vector3(0, 0.05f, 0);
    Vector3 _objectHeight;
    CapsuleCollider _collider;
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _objectHeight = new Vector3(0, _collider.bounds.size.y, 0);
        _collider.enabled = false;
    }

    void ReleaseTrailObject(Vector3 pos)
    {
        bool trailActive = _trailObject.gameObject.activeSelf;
        _trailObject.gameObject.SetActive(true);
        _trailObject.transform.parent = null;
        //GameObject gObject = Instantiate(_particles.gameObject, this.transform);
        //gObject.SetActive(true);
        //gObject.transform.parent = this.transform.parent;
        //_particles.gameObject.SetActive(true);

        pos.y -= 0.05f;
        transform.position = pos + _objectHeight * 0.5f;

        /*if (!trailActive)*/ StartCoroutine(_trailObject.followTarget(this.transform));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _particles.gameObject.SetActive(true);
            _particles.Emit(20);
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
                if (Physics.Raycast(position + groundOffset, Vector3.down, out hit, _heighCheck, _layerMask))
                {
                    Vector3 hitPos = hit.point;
                    if (!Physics.CheckCapsule(hitPos + groundOffset, hitPos + _objectHeight * 1.1f, _collider.bounds.size.x))
                    {
                        ReleaseTrailObject(hitPos);
                        break;
                    }
                }
                if (Physics.Raycast(position + groundOffset, Vector3.up, out hit, _heighCheck, _layerMask))
                {
                    Vector3 hitPos = hit.point;
                    if (!Physics.CheckCapsule(hitPos + groundOffset, hitPos + _objectHeight * 1.1f, this.transform.localScale.y))
                    {
                        ReleaseTrailObject(hitPos);
                        break;
                    }
                }
                Debug.DrawRay(position, Vector3.down* _heighCheck, Color.red, 5);
                Debug.DrawRay(position, Vector3.up* _heighCheck, Color.red, 5);
            }
            if (i == 100)
            {
                Debug.LogError("Could not find a teleportation spot in 100 attempts, consider changing the heightCheck variable");
            }
        }
    }
}
