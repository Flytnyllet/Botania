using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationMovement : MonoBehaviour
{
    [SerializeField] Vector3 _movementDirection;
    [SerializeField, Range(0, 1.57f)] float _randomDirectionOffset;
    [SerializeField] MeshRenderer _renderer;
    Velocity _velocity;
    private void Awake()
    {
        AddRandomDirectionOffset();
        if (_renderer != null)
        {
            _renderer.material.SetFloat("_Random", Random.Range(0.0f, 6.28f));
        }
        _velocity = new Velocity { tran = this.transform, direction = _movementDirection };
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

    private void OnEnable()
    {
        TrashMultiMoverScript.Instance.Subscribe(_velocity);
    }
    private void OnDisable()
    {
        TrashMultiMoverScript.Instance.UnSubscribe(_velocity);
    }

    private void FixedUpdate()
    {
        //System.Threading.Tasks.Task.Run(() => { transform.position += _movementDirection * Time.deltaTime; });
    }
}
