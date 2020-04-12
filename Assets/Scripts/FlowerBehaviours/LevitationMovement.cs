using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationMovement : MonoBehaviour
{
    CharacterController _charCon;
    float _randomStartValue;

    [SerializeField] float _rotationSpeed = 10;
    [SerializeField] float _verticalRangeRadious = 0.1f;
    [SerializeField] float _verticalTimeMultiplier = 0.4f;
    [SerializeField] float _hoptizontalXRangeRadious = 0.05f;
    [SerializeField] float _hoptizontalXTimeMultiplier = 0.12f;
    [SerializeField] float _hoptizontalZRangeRadious = 0.12f;
    [SerializeField] float _hoptizontalZTimeMultiplier = 0.07f;


    private void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _randomStartValue = +Random.Range(0, 3.14f);
    }
    void Update()
    {
        float vertical = Mathf.Sin(Time.time* _verticalTimeMultiplier+ _randomStartValue) * Time.deltaTime * _verticalRangeRadious;
        float horizontalX = Mathf.Cos(Time.time* _hoptizontalXTimeMultiplier + _randomStartValue) * Time.deltaTime * _hoptizontalXRangeRadious;
        float horizontalZ = Mathf.Cos(Time.time* _hoptizontalZTimeMultiplier + _randomStartValue) * Time.deltaTime * _hoptizontalZRangeRadious;
        _charCon.Move(new Vector3(horizontalX, vertical, horizontalZ));
        this.transform.Rotate(new Vector3(0, _rotationSpeed*Time.deltaTime,0 ));

    }
}
