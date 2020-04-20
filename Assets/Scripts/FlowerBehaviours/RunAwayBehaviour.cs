using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayBehaviour : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] ParticleSystem _particles;
    CharacterController _charCon;
    SphereCollider _collider;
    [SerializeField] float _runTime = 5.0f;
    [SerializeField] float _gravity = -9.81f;


    enum BEHAVIOUR { IDLE, FLEEING }
    BEHAVIOUR _behaviour = BEHAVIOUR.IDLE;

    void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _collider = GetComponent<SphereCollider>();
        _particles.gameObject.SetActive(false);
    }

    //Rotation Matrix
    Vector3 rotateVec3(Vector3 vec, float rot)
    {
        float x = vec.x;
        float y = vec.z;

        vec.x = x * Mathf.Cos(rot) - y * Mathf.Sin(rot);
        vec.z = x * Mathf.Sin(rot) + y * Mathf.Cos(rot);
        return vec;
    }

    void FaceAwayFromTarget(Transform target)
    {
        Vector3 to = target.position;
        Vector3 from = transform.position;
        //magic numbers for making the flower lean back slightly
        to.y = -.5f; 
        from.y = 0;

        var lookDirection = Quaternion.LookRotation(from - to);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, 200.0f * Time.deltaTime);
    }

    //Runs until player is outside the collider for a set amount of time.
    //I fall detta behöver läggas till på spårade objekt så borde detta göras före och efter while-loopen
    IEnumerator RunAway(Transform target, float time, float radius)
    {
        //consider locking if this shit actually becomes complex to the point where you regrett not making a state machine
        _behaviour = BEHAVIOUR.FLEEING;
        float timeLeft = time;
        float gravity = 0;
        _particles.gameObject.SetActive(true); //Throw dirt

        while (timeLeft > 0)
        {
            Vector3 direction = (this.transform.position - target.transform.position).normalized;
            direction.y = gravity;

            FaceAwayFromTarget(target);
            if (_charCon.isGrounded && _charCon.velocity.y < 0)
            {
                gravity = -2;
            }
            //Add sine wave to movemnt direction
            Vector3 rotatedDir = rotateVec3(direction, Mathf.Sin(Time.time) * 0.5f);

            _charCon.Move(rotatedDir * _speed * Time.deltaTime);
            gravity += _gravity * Time.deltaTime;

            //If player is inside trigger
            if (Vector3.Distance(transform.position, target.position) < radius)
            {
                timeLeft = time;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
            yield return null;
        }
        _particles.gameObject.SetActive(false);
        _behaviour = BEHAVIOUR.IDLE;
        float y = transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_behaviour == BEHAVIOUR.IDLE && other.tag == "Player")
        {
            StartCoroutine(RunAway(other.transform, _runTime, _collider.radius));
        }
    }
}