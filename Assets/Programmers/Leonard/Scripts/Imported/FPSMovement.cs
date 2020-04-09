using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
	const string DUCK_BUTTON = "Duck";

	[Header("Movement")]
	CharacterController charCon;
    [SerializeField] float _speed = 12f;
    [SerializeField] float _jumpForce = 1f;
    [SerializeField] float _gravity = -9.81f;
	public Vector3 _velocity;
	[SerializeField] float _crawlSpeedFactor = 0.5f;
	[SerializeField] float _duckDistance = 0.4f;
	[SerializeField] float _slidingSpeedFactor = 0.5f;

	[Header("Bobbing")]
	[SerializeField] float _bobbingAmount = 0.05f;
	[SerializeField] float _bobbingSpeed = 1f;
	float _bobTimer = 0;
	float defPosY;
	Vector3 slopeDirection;

	Transform _playerCam;

	public LayerMask layerMask;
	/* !OBS Weird bug causing script to disable itself when awake is used.
	void Awake()
	{
		
	}*/

	void Start()
	{
		charCon = GetComponent<CharacterController>();
		_playerCam = transform.Find("PlayerCam");
		CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);

		defPosY = _playerCam.localPosition.y;
	}

	void Update()
	{
        if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
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

			if (Input.GetButtonDown(DUCK_BUTTON))
			{
				Ducking(-_duckDistance);
			}

			//Jumping
			//if (Input.GetButtonDown("Jump")) 
			if (Input.GetButtonDown("Jump") && groundRay)
			{
				Debug.Log("JUMP!");
				Launch(jump);
			}
			//Ducking
			else if (Input.GetButton(DUCK_BUTTON) && GetGroundSlope(transform.position, Vector3.down, 3f) > 15f)
			{
				Debug.Log("SLIDING!");
				Sliding(x);
			}
			//Walking
			else
            {
                Walking(x, y);
            }

			if(Input.GetButtonUp(DUCK_BUTTON))
			{
				Ducking(_duckDistance);
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


			// Bobbing
			HeadBob(x*_speed, y*_speed);
		}
	}

	void Walking(float horizontal, float vertical)
	{
        Vector3 lookDir =_playerCam.forward;
        lookDir.y = 0;
        Vector3 move =
			_playerCam.right * horizontal +
            lookDir * vertical;
		charCon.Move(move * _speed * Time.deltaTime);
	}

	void Ducking(float duckDirection)
	{
		Vector3 ducking  = new Vector3(0, duckDirection, 0);
		_playerCam.localPosition += ducking;
		defPosY += duckDirection;
	}

	void Sliding(float z)
	{
		Vector3 lookDir = _playerCam.forward;
		lookDir.y = 0; 
		Vector3 move = new Vector3(slopeDirection.x, 0f, slopeDirection.z);
		Vector3 strafe = Vector3.Cross(move, Vector3.up);   //Normalize Directin
		Debug.Log(strafe);
		move += strafe*-z;
		//Debug.Log(move * _speed * _slidingSpeedFactor * Time.deltaTime);
		charCon.Move(move * _speed * _slidingSpeedFactor * Time.deltaTime);
	}

    void Teleport()
    {

    }

	void Launch(Vector3 launchVector)
	{
		_velocity += launchVector;
	}

	//Head Bobbing
	void HeadBob (float x, float z)
	{
		if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
		{
			//Debug.Log("Is Headbobbing");
			//Player is moving
			_bobTimer += Time.deltaTime * _bobbingSpeed;
			_playerCam.localPosition = new Vector3(_playerCam.localPosition.x, defPosY + Mathf.Sin(_bobTimer) * _bobbingAmount, _playerCam.localPosition.z);
		}
		else
		{
			//Debug.Log("Is Idlebobbing");
			//Idle
			_bobTimer = 0;
			_playerCam.localPosition = new Vector3(_playerCam.localPosition.x, Mathf.Lerp(_playerCam.localPosition.y, defPosY, Time.deltaTime * _bobbingSpeed), _playerCam.localPosition.z);
		}
	}

	float GetGroundSlope(Vector3 rayStart, Vector3 rayDirection, float rayDistance)
	{
		float terrainAngle = 0;

		Ray myRay = new Ray(rayStart, rayDirection); 
		RaycastHit hit;

		if (Physics.Raycast(myRay, out hit, rayDistance))
		{
			if (hit.collider.gameObject.tag == "ground") // Our Ray has hit the ground
			{
				terrainAngle = Vector3.Angle(Vector3.up, hit.normal);
				slopeDirection = hit.normal;
			}
		}

		return terrainAngle;
	}
}
