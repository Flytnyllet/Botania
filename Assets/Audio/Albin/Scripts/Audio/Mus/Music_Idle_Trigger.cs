using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Idle_Trigger : MonoBehaviour
{
    private float _idleTime;
    private bool _isIdleTrigger = default;
    private int _firstFrame = 1;
    private int _lastFrame = 0;

    private void Update()
    {
        if (_firstFrame != _lastFrame)
        {
            if (!Music_Manager.Instance.StartMenu && Input.GetAxis(InputKeyWords.HORIZONTAL) == 0 && Input.GetAxis(InputKeyWords.VERTICAL) == 0)
            {
                if (Random.Range(0f, 1f) > 0.8f)
                    _isIdleTrigger = true;
                else { _isIdleTrigger = false; }

                //Debug.Log("Tried to set the IdleTimer. Result?");
                _lastFrame = _firstFrame;
            }
        }

        if (Input.GetAxis(InputKeyWords.VERTICAL) > 0.01f || Input.GetAxis(InputKeyWords.HORIZONTAL) > 0.01f)
        {
            _isIdleTrigger = false;
            _idleTime = 0;
            _lastFrame = 0;

            if (Music_Manager.Instance.IsIdleMusic)
                Music_Manager.Instance.Stop_TriggerMusic();
        }


        if (_isIdleTrigger)
        {
            //Debug.Log("IdleTriggerTimer is set.");
            _idleTime = _idleTime + Time.fixedDeltaTime;
            Debug.Log(_idleTime);
        }

        if (_idleTime > 7)
        {
            Music_Manager.Instance.Init_Music(6);
            _isIdleTrigger = false;
        }
    }
}