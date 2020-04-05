using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayBehaviour : MonoBehaviour
{
	[SerializeField] float _speed;
	CharacterController _charCon;
	Transform _target;
	[SerializeField] float _gravity = -9.81f;

	Vector3 _velocity;


	void Start()
	{
		_charCon = GetComponent<CharacterController>();
		_target = GameObject.Find("Player").transform;
	}

	void Update()
	{
		float tempY = _velocity.y;
		Vector3 heading = transform.position - _target.position;
		float distance = heading.magnitude;
		_velocity = heading / distance;
		_velocity.y = tempY;
		_velocity.x += Mathf.Sin(Time.time % 360);

		_charCon.Move(_velocity * Time.deltaTime * _speed);
		if (!_charCon.isGrounded)
		{
			_velocity.y += _gravity * Time.deltaTime;
		}
		else
		{
			_velocity.y = 0;
		}
	}

	/*
	void Activate(Transform target)
	{

	}
	*/
}
