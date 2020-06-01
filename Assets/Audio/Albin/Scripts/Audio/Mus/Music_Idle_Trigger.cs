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
            if (!Music_Manager.Instance.StartMenu && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            {
                if (Random.Range(0f, 1f) > 0.95f)
                    StartCoroutine(IdleTriggerTimer());

                //Debug.Log("Tried to set the IdleTimer. Result?");
                _lastFrame = _firstFrame;
            }
            
        }

        if (Input.GetAxis("Horizontal") > 0.02f && Input.GetAxis("Vertical") > 0.02f && _lastFrame == _firstFrame)
        {
            _idleTime = 0;
            _lastFrame = 0;

            if (Music_Manager.Instance.IsIdleMusic)
                Music_Manager.Instance.Stop_TriggerMusic();
        }
    }

    private IEnumerator IdleTriggerTimer()
    {
        //Debug.Log("IdleTriggerTimer is set.");
        _idleTime = _idleTime + Time.fixedDeltaTime;
        if (_idleTime > 7)
            Music_Manager.Instance.Init_Music(5);

        yield return new WaitForSeconds(1f);
    }
}