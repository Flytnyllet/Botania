using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	CharacterController charCon;

	public float speed = 12f;

    void Start()
    {
		charCon = GetComponent<CharacterController>();
    }

    void Update()
    {
		float x = Input.GetAxis(InputKeyWords.HORIZONTAL);
		float y = Input.GetAxis(InputKeyWords.VERTICAL);

		Vector3 move = transform.right *x  + transform.forward *y;

		charCon.Move(move * speed * Time.deltaTime);
    }
}
