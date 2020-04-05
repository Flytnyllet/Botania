using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
	[SerializeField] MonoBehaviour _targetScript;

	void Start()
	{
		_targetScript.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		
		if(other.tag == "Player")
		{
			Debug.Log("StartRunning!");
			_targetScript.enabled = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("StopRunning!");
			_targetScript.enabled = false;
		}
	}
}
