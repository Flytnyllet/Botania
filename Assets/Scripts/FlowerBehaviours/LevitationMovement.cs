using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationMovement : MonoBehaviour
{
    [SerializeField] Vector3 _movementDirection;
    [SerializeField, Range(0, 1.57f)] float _randomDirectionOffset;
    [SerializeField] MeshRenderer _renderer;
    Velocity _velocity;
    Vector3 _position;
    private void Awake()
    {
        _position = transform.localPosition;
        AddRandomDirectionOffset();
        if (_renderer != null)
        {
            _renderer.material.SetFloat("_Random", Random.Range(0.0f, 6.28f));
        }
        _velocity = new Velocity { transform = this.transform, direction = _movementDirection };
    }

    void AddRandomDirectionOffset()
    {
        float sincosVal = Random.Range(-_randomDirectionOffset, _randomDirectionOffset);
        float sin = Mathf.Sin(sincosVal);
        float cos = Mathf.Cos(sincosVal);
        float x = _movementDirection.x * cos - _movementDirection.y * sin;
        float z = _movementDirection.x * sin + _movementDirection.y * cos;
        _movementDirection.x = x;
        _movementDirection.z = z;
    }
    void Move(float f)
    {
        _position += _movementDirection * f;
    }
    private void OnEnable()
    {
        try
        {
            BatchMovement.Instance.Subscribe(Move);
        }
        catch
        {
            Debug.LogError("BatchMovement.cs not found on scene, simply add that script to any singleton purpuse object");
        }
    }
    private void OnDisable()
    {
        BatchMovement.Instance.UnSubscribe(Move);
    }

    private void FixedUpdate()
    {
        //transform.position += _movementDirection * Time.deltaTime;
    }
}
