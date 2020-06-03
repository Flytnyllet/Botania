using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSMovement : MonoBehaviour
{
    // FAKE SINGLETON
    public static FPSMovement playerMovement;

    // Tag Handling (Replace with LayerMasks)
    const string DUCK_BUTTON = "Duck";
    const string SPRINT_BUTTON = "Sprint";
    const string CHANGE_CAMERA_MODE_BUTTOM = "Camera";
    //[SerializeField] string GROUND_TAG = "null";
    //[SerializeField] string WATER_TAG = "null";

    CharacterController charCon = null;
    public CharacterStats _speed = null;
    public CharacterStats _jumpForce = null;
    public CharacterStats _gravity = null;
    public CharacterFlags _flags = null;
    public Vector3 _velocity = Vector3.zero;

    [Header("Running")]
    [SerializeField] float _crawlSpeedFactor = 0.5f;
    [SerializeField] float _duckDistance = 0.4f;
    [SerializeField] float _sprintSpeedFactor = 2f;
    //[SerializeField] float _slidingSpeedFactor = 0.5f;
    //[SerializeField] float _minSlidingAngle = 25f;
    //[SerializeField] float _slopeWalkCorrection = 2f;
    //Vector3 _slopeDirection;
    [Header("Jumping")]
    [SerializeField] float _groundRayExtraDist = 3f;
    [SerializeField] float _allowedJumpDistance = 0.2f;
    [SerializeField] float _strafingSpeedFactor = 0.8f;
    [SerializeField] LayerMask layerMask = 0;
    Vector3 _cameraStartPosition = Vector3.zero;
    Vector3 _ducking = Vector3.zero;
    bool _inAir = false;

    [Header("Bobbing")]
    [SerializeField] Vector2 _bobbingAmount = new Vector2(0.05f, 0.05f);
    [SerializeField] float _bobbingSpeed = 1f;
    float _bobTimer = 0;
    float _swimBobTimer = 0;

    Transform _playerCam = null;

    [Header("Sound")]
    // SFX Variables
    Player_Emitter _emitPlayerSound = null;
    bool _allowStepSound = true;
    //Vector3 _prevPos = Vector3.zero;
    //float _randWalk = 0;
    //float _timeSinceLastStep = 0;
    //float _travelledDist = 0;
    //[SerializeField] private float _travelDist = 0;

    // Swimming Variables

    [Header("Swimming")]
    [SerializeField] float _swimCorrection = 1.0f;
    [SerializeField] float _waterToPosition = -0.02f;
    [SerializeField] float _waterRayDist = 1f;
    [SerializeField] float _waterForceMod = 8f;
    [SerializeField] float _swimMaxSpeed = 3f;
    [SerializeField] float _swimStartSpeedFactor = 0.5f;
    [SerializeField] float _swimAccelerationTime = 1f;
    [SerializeField] float _underwaterFloatBack = 3f;
    [SerializeField] float _swimBobAmount = 0.05f;
    [SerializeField] float _swimBobSpeed = 1.0f;
    [SerializeField] float _swimDeceleration = 1.0f;
    [SerializeField] float _swinSoundTempo = 1.0f;
    bool _swimSoundMayTrigger = true;
    [SerializeField] LayerMask _waterLayer;

    Vector2 _swimVelocity = new Vector2(0f, 0f);
    Collider _lastWaterChunk;
    bool _inWater = false;
    bool _swimming = false;
    bool _isUnderwater = false;
	bool _cameraAboveSurface = true;
    float _savedMoveModifier = 1.0f;

    [Header("Potion Effects")]
    [SerializeField] float _levitationSpeed = 1f;
	[SerializeField] float _levitationFallSpeed = 3f;

    //[SerializeField] float teleportationTime = 2f;
    [SerializeField] float _teleportationFloatSpeed = 1f;
    [SerializeField] float _teleportPlacementHeight = 10f;
    [SerializeField] [Range(1, 1000)] float _teleportMinDistance = 1f;
    [SerializeField] [Range(80, 2000)] float _teleportMaxDistance = 200f;
    [SerializeField] [Range(50, 500)] float _teleportRay = 100f;
    [SerializeField] LayerMask _groundLayer;
    Color alpha = new Color(0, 0, 0, 1);

    [SerializeField] Image _loadScreen = null;

    // !OBS Weird bug causing script to disable itself when awake is used.
    void Awake()
    {
        playerMovement = this;
        _swimVelocity = new Vector2(0f, 0f);
    }

    /*
	private void OnEnable()
	{
		EventManager.Subscribe(EventNameLibrary.STONED, ResetVelocity);
	}

	private void OnDisable()
	{
		EventManager.UnSubscribe(EventNameLibrary.STONED, ResetVelocity);
	}

	void ResetVelocity(EventParameter param = null)
	{
		_velocity = 0f;
	}
	*/

    void Start()
    {
        //_prevPos = transform.position;
        //_randWalk = Random.Range(0.8f, 1.2f);
        _emitPlayerSound = GetComponent<Player_Emitter>();

        charCon = GetComponent<CharacterController>();
        _playerCam = transform.Find("PlayerCam");
        _cameraStartPosition = _playerCam.localPosition;
        CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    Debug.Log("START SMOTHNESS");
        //    MouseLook camScript = _playerCam.GetComponent<MouseLook>();
        //    camScript.smoothing = !camScript.smoothing;
        //}
    }

    void FixedUpdate()
    {

        bool isStoned = CharacterState.IsAbilityFlagActive(ABILITY_FLAG.STONE);
        bool isLevitating = CharacterState.IsAbilityFlagActive(ABILITY_FLAG.LEVITATE);
        float gravityFactor = 1.0f;

        if (CharacterState.MayMove)
        {
            if (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.TELEPORT))
            {
                Teleportation();
                //Vector3 target = transform.position + Vector3.forward * 20f;
                //Teleport(target, _teleportPlacementHeight);

                return;
            }

            // == Variables ==
            //Input
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize();
            }

            Vector3 jump = new Vector3(0, 1f * _jumpForce.Value, 0);
            float moveModifier = 1.0f;

            //Ground Detection
            //float terrainAngle;
            RaycastHit groundDetection;
            bool grounded =
                (GroundRay(transform.position, Vector3.down, charCon.bounds.size.y / 2 + _groundRayExtraDist, out groundDetection)
                && true);

            RaycastHit waterDetection;
            _inWater = Physics.Raycast(transform.TransformPoint(_cameraStartPosition)
                + 0.01f * Vector3.up, Vector3.down, out waterDetection, _waterRayDist, _waterLayer);
            Debug.DrawRay(_playerCam.position + 0.45f * Vector3.up, Vector3.down * _waterRayDist, Color.red, 2f);

            _isUnderwater = (!isStoned && !_inWater && (_lastWaterChunk == null ? false : _lastWaterChunk.transform.position.y > transform.position.y));

			DivingSounds();

			if (_inAir && charCon.isGrounded)
            {
                _emitPlayerSound.Init_Land();
            }

            // == Functions ==
            if (charCon.isGrounded)
            {
                _inAir = false;
            }

            if (_inWater)
            {
                // _isUnderwater = false;
                _lastWaterChunk = waterDetection.collider;

                transform.position = Vector3.MoveTowards(transform.position, _lastWaterChunk.ClosestPoint(transform.position), _swimCorrection);

            }
            if (isStoned)
            {
                _swimming = false;
                // _isUnderwater = false;
            }
            if (_isUnderwater && !_swimming)//  && _lastWaterChunk.transform.position.y > _playerCam.position.y)
            {
                _velocity = Vector3.up * _underwaterFloatBack * Time.deltaTime;
                charCon.Move(_velocity);
                _inAir = true;
            }
            else if (_inWater && !isStoned)
            {
                transform.localPosition = Vector3.MoveTowards(transform.position, _lastWaterChunk.ClosestPoint(transform.position), _swimCorrection);
                _swimming = true;
                _inAir = true;
                Swimming(moveInput);
            }
            // Everything that can be done while grounded
            if (grounded)
            {
                if (Input.GetButton(SPRINT_BUTTON))
                {
                    moveModifier *= _sprintSpeedFactor;
                }
                // Jump, otherwise Slide, otherwise Walk
                //if (Input.GetButtonDown("Jump") && groundDetection.distance <= charCon.bounds.size.y / 2 + _allowedJumpDistance && !_inAir && !Input.GetButton(DUCK_BUTTON))
                if (Input.GetButtonDown("Jump") && !_inAir && !Input.GetButton(DUCK_BUTTON))
                {
                    Debug.Log("JUMP!");
                    _emitPlayerSound.Init_Jump();
                    _velocity.y = 0;
                    Launch(jump);
                    _inAir = true;
                    _savedMoveModifier = moveModifier;
                }
                //else if (Input.GetButton(DUCK_BUTTON) && terrainAngle > 10f)
                //{
                //	Debug.Log("SLIDING!");
                //	Sliding(x, slopeDirection);
                //}
                else
                {
                    if (Input.GetButtonDown(DUCK_BUTTON))
                    {
                        Ducking(-_duckDistance);
                    }
                    else if (Input.GetButton(DUCK_BUTTON))
                    {
                        moveModifier *= _crawlSpeedFactor;
                    }
                    else if (Input.GetButtonUp(DUCK_BUTTON))
                    {
                        Ducking(0);
                    }

                    Walking(moveInput.x, moveInput.y, groundDetection, moveModifier);

                    // Bobbing
                    if (Settings.GetToggle(Toggles.Headbobbing))
                    {
                        HeadBob(moveInput.x, moveInput.y, moveModifier);
                    }
                }
            }
            else
            {
                //_swimVelocity = Vector2.zero;
                Strafing(moveInput.x, moveInput.y);
            }

            //Gravity
            if ((!_inWater && !isLevitating) || isStoned)
            {
                Gravity(gravityFactor);
            }
            else if (Time.time % 5 < 0.01)
            {
                Debug.Log("Gravity is disabled");
            }

            if (isLevitating)
            {
                charCon.Move(_levitationSpeed * Vector3.up * Time.deltaTime);
                _inAir = true;
                _velocity.y = _levitationFallSpeed;
            }
            //transform.position = new Vector3(transform.position.x, 9.5f, transform.position.z);
        }


    }

	public static bool IsSwimming()
	{
		return playerMovement._swimming;
	}

    public void Teleport(Vector3 position)
    {
        charCon.Move(position - transform.position);
        //transform.position = position;
    }

    public bool PositionCorrection(float placementHeight)
    {
        RaycastHit hit;
        Vector3 rayPoint = transform.position.x * Vector3.right + Vector3.up * _teleportRay + transform.position.z * Vector3.forward;

        if (Physics.Raycast(rayPoint, Vector3.down, out hit, _teleportRay, _groundLayer))
        {
            Vector3 closestPoint = hit.point;

            transform.position = closestPoint + Vector3.up; //hit.collider.ClosestPointOnBounds(closestPoint+Vector3.up*10) + Vector3.up*2;

            if (transform.position.y < 8f)
            {
                Debug.Log("Attempt to correct to water");
                RaycastHit hitWater;
                Vector3 rayPointWater = transform.position.x * Vector3.right + Vector3.up * _teleportRay + transform.position.z * Vector3.forward;
                if (Physics.Raycast(rayPointWater, Vector3.down, out hitWater, _teleportRay, _waterLayer))
                {
                    Vector3 closestWater = hitWater.point;
                    transform.position = closestWater + Vector3.up;
                }
                else
                {
                    Debug.Log("Failed to correct to water");

                }
            }
            //bool foundWater = false;
            //if (Physics.Raycast(transform.position, Vector3.up, out hit, 100f, _waterLayer))
            //{
            //	foundWater = true;
            //	transform.position = hit.collider.ClosestPointOnBounds(transform.position) + Vector3.up;
            //}

            Debug.LogFormat("Position Correction placed player at {0} ater hitting {1} ", transform.position, hit.transform.gameObject.name);

            transform.position += Vector3.up * placementHeight;

            return true;
        }
        /*else if (Physics.Raycast(transform.position, Vector3.up, out hit, 500f, _waterLayer))
		{
			Vector3 closestPoint = hit.point;
			transform.position = closestPoint + Vector3.up;

			Debug.LogFormat("Position Correction placed player at {0} ater hitting {1} ", transform.position, hit.transform.gameObject.name);

			break;
		}*/
        else
        {
            Debug.LogErrorFormat("Position Correction failed to find ground, checking from position {0} to position {1}", rayPoint, rayPoint + Vector3.down * 500f);
        }

        return false;
    }

    void Teleportation()
    {
        CharacterState.SetControlState(CHARACTER_CONTROL_STATE.CUTSCENE);
        StartCoroutine(Teleportation_Execution());
    }

    IEnumerator Fade(float fadeTime, Image image, float change)
    {
        float clock = fadeTime + Time.time;
        while (clock > Time.time)
        {
            image.color += change * alpha * fadeTime * Time.deltaTime;
            yield return null;
        }
    }

    void DelayedTPFade()
    {
        StartCoroutine(Fade(1f, _loadScreen, 1f));
    }

    public IEnumerator PositionCorrection(int placementHeight, int maxFrameAttempts)
    {
        int attempts = 0;
        while (attempts < maxFrameAttempts)
        {
            if (PositionCorrection(_teleportPlacementHeight))
            {
                break;
            }
            else
            {
                charCon.Move(Vector3.forward * 0.01f);
                yield return null;
            }
            attempts++;
        }
    }

    IEnumerator Teleportation_Execution()
    {
        // =====================
        // TELEPORTATION WIND-UP
        // =====================

        _loadScreen.gameObject.SetActive(true);
        _loadScreen.color *= new Color(1f, 1f, 1f, 0f);
        Image _loadIcon = _loadScreen.transform.GetChild(0).GetComponent<Image>();
        _loadIcon.color *= new Color(1f, 1f, 1f, 0f);

        Invoke("DelayedTPFade", 1f);

        while (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.TELEPORT))
        {
            charCon.Move(Vector3.up * _teleportationFloatSpeed * Time.deltaTime);

            yield return null;
        }

        _loadScreen.color += new Color(0f, 0f, 0f, 1f);
        _loadIcon.color += new Color(0f, 0f, 0f, 1f);

        // =====================

        // =====================
        // ACTUAL TELEPORTATION
        // =====================

        float teleportRadius = Random.Range(_teleportMinDistance, _teleportMaxDistance);
        float teleportAngle = Random.Range(0, 360f);
        Vector2 teleportPoint = Mathf.Sin(teleportAngle) * teleportRadius * Vector2.right + Mathf.Cos(teleportAngle) * teleportRadius * Vector2.up;
        Vector3 teleportTarget = transform.position + Vector3.right * teleportPoint.x + Vector3.forward * teleportPoint.y;
        //Vector3.right * transform.position.x * teleportPoint.x + Vector3.forward * transform.position.z * teleportPoint.y;
        Debug.LogFormat("Teleportation target: {0}", teleportTarget);
        Teleport(teleportTarget);

        _loadIcon.color += new Color(0f, 0f, 0f, 1f);
        Fade(1f, _loadScreen, -1f);

        yield return null;

        int maxAttempts = 5;
        int attempts = 0;
        while (attempts < maxAttempts)
        {
            if (PositionCorrection(_teleportPlacementHeight))
            {
                break;
            }
            else
            {
                attempts++;
                charCon.Move(Vector3.forward * 0.01f);
                yield return null;
            }
        }

        yield return null;

        attempts = 0;
        while (Physics.OverlapBox(transform.position, Vector3.one * 0.5f).Length > 1 && attempts < maxAttempts)
        {
            Debug.Log("Correcting position because collision");
            transform.position += Vector3.up * _teleportPlacementHeight;
            attempts++;
            yield return null;
        }

        // =====================

        // =====================
        // TELEPORTATION WIND-DOWN
        // =====================

        CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);

        _gravity.BaseValue = 4f;
        charCon.Move(Vector3.forward * 0.01f);

        while (!charCon.isGrounded)
        {
            //charCon.Move(Vector3.down * _teleportationFloatSpeed * Time.deltaTime);
            yield return null;
        }

        _gravity.BaseValue = 9.81f;
        // =====================
        yield return null;
        float time = 0;
        Color colA = new Color(1, 1, 1, 1);
        Color colB = new Color(0, 0, 0, 1);
        while (time < 1)
        {
            Debug.Log(_loadScreen.color);
            time += Time.deltaTime;
            colA.a = 1 - time;
            colB.a = 1 - time;
            _loadIcon.color = colA;
            _loadScreen.color = colB;
            yield return null;
        }
        _loadScreen.gameObject.SetActive(false);
    }

    void Strafing(float horizontal, float vertical)
    {
        Vector3 lookDir = _playerCam.forward;
        lookDir.y = 0;
        Vector3 move =
            _playerCam.right.normalized * horizontal +
            lookDir.normalized * vertical;
        charCon.Move(move * _speed.Value * _strafingSpeedFactor * _savedMoveModifier * Time.deltaTime);
    }

    void Walking(float horizontal, float vertical, RaycastHit ground, float modifier)
    {
        //Vector2 velocity = new Vector2(horizontal, vertical).normalized;

        Vector3 lookDir = _playerCam.forward;
        lookDir.y = 0;
        Vector3 move =
            _playerCam.right.normalized * horizontal +
            lookDir.normalized * vertical;
        charCon.Move(move.normalized * _speed.Value * modifier * Time.deltaTime);

        //Post move distance to ground check
        /*if (ground.distance <= _slopeWalkCorrection && !_inAir)
		{
			charCon.Move(Vector3.down * ground.distance);
		}*/
    }

    void Ducking(float duckDirection)
    {
        _ducking = new Vector3(0, duckDirection, 0);
        _playerCam.localPosition = _cameraStartPosition + _ducking;
    }

    /*
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
	*/

    void Gravity(float factor)
    {
        if (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.SLOWFALL))
        {
            if (_velocity.y > 0) _velocity.y = 0;
            factor *= 0.075f;
        }

        charCon.Move(_velocity * Time.deltaTime);
        if (!charCon.isGrounded)
        {
            _velocity.y += _gravity.Value * factor * Time.deltaTime;
        }
        else
        {
            _velocity.y = -2f; //_gravity.Value;
        }
    }

    void Swimming(Vector2 inputs)
    {
        //Stabilize Position
        /*float traPosY = transform.position.y;
		transform.position -= traPosY * Vector3.up;
		transform.position += _lastWaterChunk.transform.position.y * Vector3.up; //Mathf.Abs(traPosY - _lastWaterChunk.transform.position.y)*/
        //transform.position = Vector3.MoveTowards(transform.position, _lastWaterChunk.ClosestPoint(transform.position), _swimCorrection);
        charCon.Move(new Vector3(0f, _lastWaterChunk.transform.position.y - transform.position.y, 0f));

        //Debug.Log("Water CHunk Position: " + _lastWaterChunk.transform.position.y);

        Vector2 swimming = inputs * _swimAccelerationTime * _swimMaxSpeed;
        _swimVelocity += swimming;

        if (_swimVelocity.magnitude < _swimStartSpeedFactor * _swimMaxSpeed && inputs.magnitude == 1 && _swimVelocity.magnitude != 0)
        {
            float ratio = (_swimStartSpeedFactor * _swimMaxSpeed) / _swimVelocity.magnitude;
            _swimVelocity *= ratio;
        }
        else if (_swimVelocity.magnitude > _swimMaxSpeed && _swimVelocity.magnitude != 0)
        {
            float ratio = _swimMaxSpeed / _swimVelocity.magnitude;
            _swimVelocity *= ratio;
        }

        Vector3 readyX = _playerCam.right;
        Vector3 readyZ = _playerCam.forward;
        readyX.y = 0f;
        readyZ.y = 0f;
        Vector3 readySwimVelocity = readyX.normalized * _swimVelocity.x + readyZ.normalized * _swimVelocity.y;
        charCon.Move(readySwimVelocity * Time.deltaTime);

        SwimBob(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float SwimTempo = Mathf.Sin(Time.time * _swinSoundTempo);
        if (SwimTempo < 0 && _swimSoundMayTrigger && readySwimVelocity.magnitude > 0.1)
        {
            _emitPlayerSound.Init_Swim(0f);
            _swimSoundMayTrigger = false;
        }
        else if (SwimTempo > 0)
        {
            _swimSoundMayTrigger = true;
        }
        if (inputs.magnitude < 0.1f)
        {
            _swimVelocity -= (_swimVelocity / _swimDeceleration) * Time.deltaTime;
        }

        _swimVelocity -= _swimVelocity * Time.deltaTime;
    }

    void Launch(Vector3 launchVector)
    {
        _velocity += launchVector;
    }

    //Head Bobbing !Stolen from the internet
    void HeadBob(float x, float z, float modifier = 1.0f)
    {
        //_timeSinceLastStep += Time.deltaTime;
        //_travelledDist += (transform.position - _prevPos).magnitude;
        Vector3 cameraLocalPosition = _cameraStartPosition + _ducking;

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
        {
            //Player is moving
            _bobTimer += Time.deltaTime * _bobbingSpeed;
            float sinVal = Mathf.Sin(_bobTimer * modifier);
            float yDirectional = Mathf.Lerp(_playerCam.localPosition.y, cameraLocalPosition.y - Mathf.Abs(sinVal)
                * _bobbingAmount.y * modifier, Time.deltaTime * _swimBobSpeed);
            Vector3 xDirectional = _playerCam.transform.right.normalized * (cameraLocalPosition.x + sinVal * _bobbingAmount.x * modifier);

            //_playerCam.localPosition = Vector3.Lerp(_playerCam.localPosition, new Vector3(xDirectional.x, yDirectional, xDirectional.z), 0.5f);
            _playerCam.localPosition = new Vector3(xDirectional.x, yDirectional, xDirectional.z);

            if (!_inAir && Mathf.Abs(sinVal) > 0.9 && _allowStepSound)
            {
                _allowStepSound = false;
                FootstepsSound();
            }
            else if (Mathf.Abs(sinVal) < 0.9)
            {
                _allowStepSound = true;
            }
        }
        else
        {
            //Idle
            _bobTimer = 0;
            _playerCam.localPosition = new Vector3(_playerCam.localPosition.x,
                Mathf.Lerp(_playerCam.localPosition.y, cameraLocalPosition.y, Time.deltaTime * _bobbingSpeed), _playerCam.localPosition.z);
        }
    }
    void SwimBob(float x, float z)
    {
        Vector3 cameraLocalPosition = _cameraStartPosition + _ducking;

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
        {
            //Player is moving
            _swimBobTimer += Time.deltaTime * _swimBobSpeed;
            _playerCam.localPosition = new Vector3(_playerCam.localPosition.x,
                Mathf.Lerp(_playerCam.localPosition.y, cameraLocalPosition.y + Mathf.Sin(_swimBobTimer) * _swimBobAmount, Time.deltaTime * _swimBobSpeed), _playerCam.localPosition.z);
        }
        else
        {
            //Idle
            _swimBobTimer = 0;
            _playerCam.localPosition = new Vector3(_playerCam.localPosition.x,
                Mathf.Lerp(_playerCam.localPosition.y, cameraLocalPosition.y, Time.deltaTime * _swimBobSpeed), _playerCam.localPosition.z);
        }
    }

    bool GroundRay(Vector3 rayStart, Vector3 rayDirection, float rayDistance, out RaycastHit hit)
    {
        //Ray groundRay = new Ray(rayStart, rayDirection);
        bool onHit = Physics.Raycast(rayStart, rayDirection, out hit, rayDistance, layerMask);
        if (onHit)
        {
            //if (hit.transform.name == "Terrain Chunk")
            //{
            //    Renderer rend = hit.transform.GetComponent<Renderer>();
            //    Texture2D tex = rend.material.GetTexture("_NoiseTextures") as Texture2D;
            //    CharacterState.PositionType = tex.GetPixel((int)hit.textureCoord.x * tex.width, (int)hit.textureCoord.y * tex.height).r;
            //    //Debug.Log(tex.GetPixel((int)hit.textureCoord.x * tex.width, (int)hit.textureCoord.y * tex.height));
            //}
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

	void DivingSounds()
	{
		if(_lastWaterChunk != null)
		{
			if(_cameraAboveSurface == true && _playerCam.transform.position.y < _lastWaterChunk.transform.position.y)
			{
                _emitPlayerSound.Init_EnterUnderwater(1);

				_cameraAboveSurface = false;
			}
			else if(_cameraAboveSurface == false && _playerCam.transform.position.y > _lastWaterChunk.transform.position.y)
			{
                _emitPlayerSound.Init_EnterUnderwater(0);

                _cameraAboveSurface = true;
			}
		}
	}

    void FootstepsSound()
    {
        //if (!_inAir && _travelledDist >= _travelDist + _randWalk)
        //{
        if (transform.position.y < 10.2f)
        {
            _emitPlayerSound.Init_Footsteps(1);
        }
        else
        {
            _emitPlayerSound.Init_Footsteps(0);
        }
        //_travelledDist = 0f;
        //}
        //_prevPos = transform.position;
    }
}
