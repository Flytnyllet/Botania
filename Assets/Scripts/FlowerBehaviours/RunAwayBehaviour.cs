using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayBehaviour : MonoBehaviour
{
    [SerializeField] float _speed;
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
    //Runs until player is outside the collider for a set amount of time.
    IEnumerator RunAway(Transform target, float time, float radius)
    {
        float timeLeft = time;
        _behaviour = BEHAVIOUR.FLEEING;
        Debug.Log("running");
        Vector3 direction = (this.transform.position - target.transform.position).normalized;
        while (timeLeft > 0)
        {
            if (_charCon.isGrounded && _charCon.velocity.y < 0)
            {
                direction.y = -2;
            }
            Vector3 rotatedDir = rotateVec3(direction, Mathf.Sin(Time.time));

            _charCon.Move(rotatedDir * _speed * Time.deltaTime);
            direction.y += _gravity * Time.deltaTime;
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
        _behaviour = BEHAVIOUR.IDLE;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_behaviour == BEHAVIOUR.IDLE && other.tag == "Player")
        {
            StartCoroutine(RunAway(other.transform, _runTime, _collider.radius));
        }
    }
}

