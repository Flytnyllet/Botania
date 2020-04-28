using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
	// FAKE SINGLETON
	public static FPSMovement playerMovement;

	// Tag Handling (Replace with LayerMasks)
	const string DUCK_BUTTON = "Duck";
	[SerializeField] string GROUND_TAG = "null";
	[SerializeField] string WATER_TAG = "null";

	[Header("Movement")]
	CharacterController charCon;
	public CharacterStats _speed;
	public CharacterStats _jumpForce;
	public CharacterStats _gravity;
	public CharacterFlags _flags;
	public Vector3 _velocity;
	[SerializeField] float _crawlSpeedFactor = 0.5f;
	[SerializeField] float _duckDistance = 0.4f;
	[SerializeField] float _slidingSpeedFactor = 0.5f;
	//Vector3 _slopeDirection;
	[SerializeField] float _groundRayExtraDist = 3f;
	[SerializeField] float _allowedJumpDistance = 0.2f;
	float _groundRayDistance;
	[SerializeField] float _minSlidingAngle = 25f;
	[SerializeField] float _slopeWalkCorrection = 2f;
	[SerializeField] float _strafingSpeedFactor = 0.8f;
	[SerializeField] float _jumpTimeout = 0.3f;
	[SerializeField] LayerMask layerMask;
	float _lastJump = 0;
	Vector3 _cameraStartPosition;
	bool _inAir = false;
	bool _isDucking = false;

	[Header("Bobbing")]
	[SerializeField] float _bobbingAmount = 0.05f;
	[SerializeField] float _bobbingSpeed = 1f;
	float _bobTimer = 0;
	float _defPosY;

	Transform _playerCam;
	//public LayerMask layerMask;

	// !OBS Weird bug causing script to disable itself when awake is used.
	void Awake()
	{
		playerMovement = this;
	}

	void Start()
	{
		charCon = GetComponent<CharacterController>();
		_playerCam = transform.Find("PlayerCam");
		_cameraStartPosition = _playerCam.localPosition;
		_defPosY = _cameraStartPosition.y;
		CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
	}

	void Update()
	{
		if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED || CharacterState.Control_State == CHARACTER_CONTROL_STATE.MENU)
		{
			// == Variables ==
			//Input
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");
			Vector3 jump = new Vector3(0, 1f * _jumpForce.Value, 0);

			//Ground Detection
			float terrainAngle;
			RaycastHit groundDetection;
			bool grounded = GroundRay(transform.position, Vector3.down, charCon.bounds.size.y / 2 + _groundRayExtraDist, out groundDetection);

			// == Functions ==
			//_inAir = !charCon.isGrounded;
			if(charCon.isGrounded)
			{
				_inAir = false;
			}
			/*if (groundDetection.distance <= charCon.bounds.size.y / 2 + _allowedJumpDistance)
			{
				_inAir = false;
			}*/

			// Everything that can be done while grounded
			if (grounded)
			{
				terrainAngle = Vector3.Angle(Vector3.up, groundDetection.normal);
				Vector3 slopeDirection = groundDetection.normal;

				// Jump, otherwise Slide, otherwise Walk
				if (Input.GetButtonDown("Jump") && groundDetection.distance <= charCon.bounds.size.y / 2 + _allowedJumpDistance  && !_inAir)
				{
					Debug.Log("JUMP!");
					_velocity.y = 0;
					Launch(jump);
					_inAir = true;
				}
                //else if (Input.GetButton(DUCK_BUTTON) && terrainAngle > 10f)
                //{
                //	Debug.Log("SLIDING!");
                //	Sliding(x, slopeDirection);
                //}
                else
                {
                    Walking(x, y, groundDetection);
				}
			}
			else
			{
				Strafing(x, y);
			}

			//Ducking
			if (Input.GetButtonDown(DUCK_BUTTON))
				Ducking(-_duckDistance);
			else if (Input.GetButtonUp(DUCK_BUTTON))
				Ducking(0);

			//Gravity
			charCon.Move(_velocity * Time.deltaTime);
			if (!charCon.isGrounded) _velocity.y += _gravity.Value * Time.deltaTime;
			else _velocity.y = 0;

			// Bobbing
			HeadBob(x * _speed.Value, y * _speed.Value);

			//bool groundRay = Physics.Raycast(transform.position, Vector3.down * 2, 2f);
			/*if (_velocity.y > 0)
                groundRay = false; */
		}
	}

	void Strafing(float horizontal, float vertical)
	{
		Vector3 lookDir = _playerCam.forward;
		lookDir.y = 0;
		Vector3 move =
			_playerCam.right.normalized * horizontal +
			lookDir.normalized * vertical;
		charCon.Move(move * _speed.Value *_strafingSpeedFactor * Time.deltaTime);
	}

	void Walking(float horizontal, float vertical, RaycastHit ground)
	{
		Vector3 lookDir = _playerCam.forward;
		lookDir.y = 0;
		Vector3 move =
			_playerCam.right.normalized * horizontal +
			lookDir.normalized * vertical;
		charCon.Move(move * _speed.Value * Time.deltaTime);

		//Post move distance to ground check
		if (ground.distance <= _slopeWalkCorrection && !_inAir)
		{
			charCon.Move(Vector3.down * ground.distance);
		}
	}

	void Ducking(float duckDirection)
	{
		Vector3 ducking = new Vector3(0, duckDirection, 0);
		_playerCam.localPosition = _cameraStartPosition + ducking;
		_defPosY = _cameraStartPosition.y + duckDirection;
	}

	void Sliding(float z, Vector3 slopeDirection)
	{
		Vector3 lookDir = _playerCam.forward;
		lookDir.y = 0;
		Vector3 move = new Vector3(slopeDirection.x, 0f, slopeDirection.z);
		Vector3 strafe = Vector3.Cross(move, Vector3.up);   //Normalize Directin
		Debug.Log("Sliding");
		move += strafe * -z;
		//Debug.Log(move * _speed * _slidingSpeedFactor * Time.deltaTime);
		charCon.Move(move * _speed.Value * _slidingSpeedFactor * Time.deltaTime);
	}

	void Teleport()
	{

	}

	void Launch(Vector3 launchVector)
	{
		_velocity += launchVector;
	}

	//Head Bobbing !Stolen from the internet
	void HeadBob(float x, float z)
	{
		if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
		{
			//Player is moving
			_bobTimer += Time.deltaTime * _bobbingSpeed;
			_playerCam.localPosition = new Vector3(_playerCam.localPosition.x,
				_defPosY + Mathf.Sin(_bobTimer) * _bobbingAmount, _playerCam.localPosition.z);
		}
		else
		{
			//Idle
			_bobTimer = 0;
			_playerCam.localPosition = new Vector3(_playerCam.localPosition.x,
				Mathf.Lerp(_playerCam.localPosition.y, _defPosY, Time.deltaTime * _bobbingSpeed), _playerCam.localPosition.z);
		}
	}

	//Ground Detection !Stolen from the internet
	bool GroundRay(Vector3 rayStart, Vector3 rayDirection, float rayDistance, out RaycastHit hit)
	{
		//Ray groundRay = new Ray(rayStart, rayDirection);
		bool onHit = Physics.Raycast(rayStart, rayDirection, out hit, rayDistance, layerMask);
		if (onHit)
		{
			//Debug.Log("Bee");
			return true;
		}

		return false;
		/*
        if (Physics.Raycast(groundRay, out hit, rayDistance))
        {
            if (GROUND_TAG == "null" || hit.collider.gameObject.tag == GROUND_TAG)
            {
                //_slopeDirection = hit.normal;
                //Debug.DrawRay(transform.position, slopeDirection, Color.magenta, 1f);
                return true;
            }
        }
        return false;*/
	}
}
