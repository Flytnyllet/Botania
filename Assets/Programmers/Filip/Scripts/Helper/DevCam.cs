using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCam : MonoBehaviour
{
    [SerializeField] bool _useDevCam = true;

    Camera _camera;

    [SerializeField] Transform _playerCamera;
    [SerializeField] CharacterController _characterController;
    [SerializeField] GameObject _instructions;
    [SerializeField] float _speed = 3f;
    [SerializeField] float _tiltSpeed = 0.25f;
    [SerializeField] float _speedModifier = 1f;
    [SerializeField] float _tiltSpeedModifier = 1f;
    [SerializeField] int _devLayer = 20;
    [SerializeField] FPSMovement _movementScript;
    [SerializeField] Interactor _interactorScript;
    [SerializeField] GameObject _crosshairHolder;

    public bool _activated = false;
    int _originalLayer;
    float _tilt = 0f;

    private void Awake()
    {
        _camera = Camera.main;
        _instructions.SetActive(false);

        if (!_useDevCam)
            Destroy(this);

        _originalLayer = gameObject.layer;
    }

    private void Update()
    {

        bool activate = Input.GetKeyDown(KeyCode.F1);
        if (activate)
        {
            RenderSettings.fog = true;

            _speedModifier = 1f;
            _tilt = 0;
            _tiltSpeedModifier = 1f;
            _playerCamera.localEulerAngles = new Vector3(_playerCamera.localEulerAngles.x, _playerCamera.localEulerAngles.y, _tilt);
            _movementScript.enabled = _activated;
            _interactorScript.enabled = _activated;
            _crosshairHolder.SetActive(_activated);
            _instructions.SetActive(!_activated);
            _activated = !_activated;
            if (_activated)
                gameObject.layer = _devLayer;
            else
                gameObject.layer = _originalLayer;
        }

        if (_activated)
        {

            bool showInstructions = Input.GetKeyDown(KeyCode.F2);
            bool shouldCollide = Input.GetKeyDown(KeyCode.F3);
            bool resetSpeed = Input.GetKeyDown(KeyCode.F4);
            bool resetTilt = Input.GetKeyDown(KeyCode.F5);
            bool removeFog = Input.GetKeyDown(KeyCode.F6);
            bool smoothCamera = Input.GetKeyDown(KeyCode.F7);
            Vector2 scroll = Input.mouseScrollDelta;
            bool shouldChangeSpeed = Input.GetKey(KeyCode.LeftShift);

            bool shouldChangeTilt = Input.GetKey(KeyCode.Z);

            if (showInstructions)
                _instructions.SetActive(!_instructions.activeSelf);
            if (shouldCollide)
                gameObject.layer = gameObject.layer == _devLayer ? _originalLayer : _devLayer;
            if (resetSpeed)
                _speedModifier = 1f;
            if (resetTilt)
            {
                _tilt = 0;
                _tiltSpeedModifier = 1f;
            }
            if (removeFog)
                RenderSettings.fog = !RenderSettings.fog;

            if (smoothCamera)
            {
                if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
                    CharacterState.SetControlState(CHARACTER_CONTROL_STATE.Dev);
                else
                    CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
            }

            if (scroll != Vector2.zero && shouldChangeSpeed)
            {
                if (scroll.y > 0)
                    _speedModifier *= 1.25f;
                else
                    _speedModifier *= 0.8f;
            }

            if (scroll != Vector2.zero && shouldChangeTilt)
            {
                if (scroll.y > 0)
                    _tiltSpeedModifier *= 1.25f;
                else
                    _tiltSpeedModifier *= 0.8f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_activated)
        {
            Vector4 movement = new Vector4(Input.GetAxis(InputKeyWords.HORIZONTAL), Input.GetAxis(InputKeyWords.VERTICAL), Input.GetAxis(InputKeyWords.JUMP), Input.GetAxis(InputKeyWords.DUCK));
            bool tiltLeft = Input.GetKey(KeyCode.Alpha1);
            bool tiltRight = Input.GetKey(KeyCode.Alpha3);

            Tilt(tiltLeft, tiltRight);
            Walking(movement, _speedModifier);
        }
    }


    void Walking(Vector4 movement, float modifier)
    {
        if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.Dev)
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.x /= _camera.pixelWidth;
            mousePos.y /= _camera.pixelHeight;
            float temp = mousePos.x;
            mousePos.x = mousePos.y;
            mousePos.y = temp;
            mousePos = mousePos * 2 - new Vector2(1, 1);
            _playerCamera.Rotate(mousePos * 10 * Time.deltaTime);
        }

        Vector3 lookDir = _playerCamera.forward;
        Vector3 move = _playerCamera.right.normalized * movement.x + lookDir.normalized * movement.y + _playerCamera.up.normalized * movement.z + _playerCamera.up * -movement.w;
        _characterController.Move(move.normalized * _speed * modifier * Time.deltaTime);
    }

    void Tilt(bool left, bool right)
    {
        if (left)
            _tilt += _tiltSpeed * _tiltSpeedModifier;
        if (right)
            _tilt -= _tiltSpeed * _tiltSpeedModifier;

        _playerCamera.localEulerAngles = new Vector3(_playerCamera.localEulerAngles.x, _playerCamera.localEulerAngles.y, _tilt);
    }
}
