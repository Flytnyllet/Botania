using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
	CharacterController charCon;

	public float speed = 12f;
	public float jumpForce = 1f;
	public float gravity = -9.81f;
		
	Vector3 velocity;

	public KeyCode ChangeCamera = KeyCode.P;
	Vector3[] camModePositions = new Vector3[2] 
	{   new Vector3(0, 0.8f, 0.15f),
		new Vector3(0, 2.2f, -3.5f) };
	int currentCam = 1;
	Transform PlayerCam;

	public LayerMask layerMask;

	void Start()
	{
		charCon = GetComponent<CharacterController>();
		PlayerCam = transform.Find("PlayerCam");
	}

	void Update()
	{
		//Input
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector3 jump = new Vector3(0, 1f*jumpForce, 0);
		//Debug.DrawRay
		bool groundRay = Physics.Raycast(transform.position, Vector3.down*2, 2f, layerMask);

		if(velocity.y > 0)
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

		if (Input.GetKeyDown(ChangeCamera))
		{
			currentCam =  (currentCam +1) % camModePositions.Length;
			PlayerCam.localPosition = camModePositions[currentCam];
		}

		//Gravity
		charCon.Move(velocity * Time.deltaTime);
		if (!charCon.isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = 0;
		}
	}

	void Walking(float horizontal, float vertical)
	{
		Vector3 move =
			PlayerCam.right * horizontal +
			PlayerCam.forward * vertical;
		charCon.Move(move * speed * Time.deltaTime);
	}

	void Launch(Vector3 launchVector)
	{
		velocity += launchVector;
	}
}
