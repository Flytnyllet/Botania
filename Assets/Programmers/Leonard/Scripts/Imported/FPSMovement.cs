using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
	CharacterController charCon;
    [SerializeField] CharacterState _stateObject;
    [SerializeField] float _speed = 12f;
    [SerializeField] float _jumpForce = 1f;
    [SerializeField] float _gravity = -9.81f;
		
	Vector3 _velocity;

    [SerializeField] KeyCode _changeCamera = KeyCode.P;
	Vector3[] _camModePositions = new Vector3[2] 
	{   new Vector3(0, 0.8f, 0.15f),
		new Vector3(0, 2.2f, -3.5f) };
	int _currentCam = 1;
	Transform _playerCam;

	public LayerMask layerMask;

	void Start()
	{
		charCon = GetComponent<CharacterController>();
		_playerCam = transform.Find("PlayerCam");
	}

	void Update()
	{
        if (_stateObject.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
        {
            //Input
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector3 jump = new Vector3(0, 1f * _jumpForce, 0);
            //Debug.DrawRay
            bool groundRay = Physics.Raycast(transform.position, Vector3.down * 2, 2f, layerMask);

            if (_velocity.y > 0)
            {
                groundRay = false;
            }	

            //Jumping
            //if (Input.GetButtonDown("Jump")) 
            if (Input.GetButtonDown("Jump") && groundRay)
            {
                Debug.Log("JUMP!");
                Launch(jump);
            }
            //Walking
            else
            {
                Walking(x, y);
            }

            if (Input.GetKeyDown(_changeCamera))
            {
                _currentCam = (_currentCam + 1) % _camModePositions.Length;
                _playerCam.localPosition = _camModePositions[_currentCam];
            }

            //Gravity
            charCon.Move(_velocity * Time.deltaTime);
            if (!charCon.isGrounded)
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
            else
            {
                _velocity.y = 0;
            }
        }
	}

	void Walking(float horizontal, float vertical)
	{
		Vector3 move =
			_playerCam.right * horizontal +
			_playerCam.forward * vertical;
		charCon.Move(move * _speed * Time.deltaTime);
	}

	void Launch(Vector3 launchVector)
	{
		_velocity += launchVector;
	}
}
