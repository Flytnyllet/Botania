using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Mus_Blom_07 : MonoBehaviour
{
    private float _emitterDistance;
    private float _lastEmitterDistance;

    private Vector3 _originalEmitterPosition;
    private Vector3 _currentEmitterPosition;
    private float _moveEmitterTime;
    private float _moveEmitterPosition;
    private Vector3 _lastPosition;

    private float _followValue;
    private double _d_followValue;
    private float _r_followValue;
    private bool isFollow = default;
    //[SerializeField]
    //private bool _blom_07_potion = default;

    private Quaternion _targetRotation;
    private Quaternion _relativeRotation;

    private float _calmTime;

    private StudioListener listener;
    private SphereCollider _followCollider;
    private Mus_Blom_07_Emitter blom_07;
    private Player _player;

    private void Awake()
    {
        listener = FindObjectOfType<StudioListener>();
        _player = FindObjectOfType<Player>();
        _followCollider = GetComponent<SphereCollider>();
        blom_07 = GetComponentInChildren<Mus_Blom_07_Emitter>();
        _originalEmitterPosition = new Vector3(blom_07.transform.position.x, blom_07.transform.position.y, blom_07.transform.position.z);
    }

    private void Start()
    {
        _followCollider.radius = blom_07._maxDistance * 0.9f;
        isFollow = false;
        //_blom_07_potion = false;
    }

    private void Update()
    {
        SetBlom07Mov();

        _emitterDistance = Vector3.Distance(blom_07.transform.position, _player.transform.position);
        _currentEmitterPosition = new Vector3(blom_07.transform.position.x, blom_07.transform.position.y, blom_07.transform.position.z);

        switch (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.SUPERHEARING))
        {
            case false:
                if (!isFollow)
                {
                    if (_emitterDistance < blom_07._maxDistance && _emitterDistance != _lastEmitterDistance)
                    {
                        _followValue = Mathf.InverseLerp(blom_07._maxDistance, 0, _emitterDistance);
                        _d_followValue = Math.Round(_followValue, 1);
                        _r_followValue = Convert.ToSingle(_d_followValue);
                        _lastEmitterDistance = _emitterDistance;
                    }
                    _targetRotation = listener.transform.rotation;
                    _relativeRotation = Quaternion.Lerp(blom_07.transform.rotation, _targetRotation, _r_followValue);
                    blom_07.transform.rotation = _relativeRotation;

                    blom_07.Set_Parameter(blom_07._blom07DistanceParameterId, 0);
                }

                if (isFollow)
                {
                    blom_07.transform.rotation = listener.transform.rotation;
                    blom_07.Set_Parameter(blom_07._blom07DistanceParameterId, 1);
                }

                blom_07.Set_Parameter(blom_07._isFollowParameterId, _r_followValue);
                blom_07.Set_Parameter(blom_07._blom07PotionParameterId, 0);

                blom_07.transform.position = _followCollider.ClosestPoint(_player.transform.position);
                break;
            case true:
                blom_07.Override_Max_Distance(blom_07._maxDistance * 1.25f);
                if (!isFollow)
                {
                    MoveEmitter(_currentEmitterPosition, _followCollider.ClosestPoint(_player.transform.position));

                    blom_07.Set_Parameter(blom_07._blom07DistanceParameterId, 0);
                }

                if (isFollow)
                {
                    blom_07.Set_Parameter(blom_07._blom07DistanceParameterId, 1);

                    MoveEmitter(_currentEmitterPosition, _originalEmitterPosition);
                }
                blom_07.Set_Parameter(blom_07._blom07PotionParameterId, 1);
                blom_07.Set_Parameter(blom_07._isFollowParameterId, 0.55f);
                break;
        }
    }

    private void MoveEmitter(Vector3 currentPos, Vector3 targetPos)
    {
        if (Vector3.Distance(currentPos, targetPos) > 0.005f)
        {
            float step = 10 * Time.deltaTime;
            blom_07.transform.position = Vector3.MoveTowards(currentPos, targetPos, step);
        }
    }

    private void SetBlom07Mov()
    {
        if (_player.transform.position != _lastPosition)
        {
            blom_07.Set_Parameter(blom_07._blom07MovParameterId, 1);
            blom_07.Set_Parameter(blom_07._blom07CalmParameterId, 0);
            _calmTime = 0;
            _lastPosition = _player.transform.position;
        }
        else
        {
            blom_07.Set_Parameter(blom_07._blom07MovParameterId, 0);

            if (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.SUPERHEARING) && isFollow)
            {
                _calmTime = _calmTime + Time.fixedDeltaTime;
                blom_07.Set_Parameter(blom_07._blom07CalmParameterId, Mathf.Floor(_calmTime));
                if (_calmTime < 31)
                    //Debug.Log("Blom_07 har lugnats ner i " + Mathf.Floor(_calmTime) + " sekunder.");
                    if (_calmTime > 31) { }
                //Debug.Log("Och nu kan du plocka blomman.");

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        isFollow = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        isFollow = false;
    }
    private void OnEnable()
    {
        CalmFlowerTargetController.Instance.Subscribe(this.transform);
    }

    private void OnDisable()
    {
        CalmFlowerTargetController.Instance.Unsubscribe(this.transform);
        blom_07.Set_Parameter(blom_07._blom07MovParameterId, 1);
        blom_07.Set_Parameter(blom_07._blom07CalmParameterId, 15);
        blom_07.Stop_Blom_07();
    }
}
