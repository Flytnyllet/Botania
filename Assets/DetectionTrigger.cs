using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
	[SerializeField] DetectionTriggerTarget[] _targetScripts;

    //Det här skriptet måste ha en kollider, och måste placeras som barn till det objekt som den hanterar
    private void Awake()
    {
        _targetScripts = GetComponentsInParent<DetectionTriggerTarget>();
        //_targetScripts = GetComponents<DetectionTriggerTarget>();
    }

    void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
            foreach (DetectionTriggerTarget target in _targetScripts)
            {
                target.EnterTrigger(other.transform);
            }
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			//_targetScript.Trigger();
        }
	}
}


public interface DetectionTriggerTarget
{
    void EnterTrigger(Transform transform);
    void LeaveTrigger(Transform transform);
}