using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    Camera _camera;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    [SerializeField] RotationAxes axes = RotationAxes.MouseXAndY;
    [SerializeField] float sensitivityX = 10F;
    [SerializeField] float sensitivityY = 10F;

    [SerializeField] float minimumX = -360F;
    [SerializeField] float maximumX = 360F;

    [SerializeField] float minimumY = -60F;
    [SerializeField] float maximumY = 60F;

    [SerializeField] bool smoothing = false;
	[Range(0f, 1f)] public float smoothValue = 0.5f;
	Vector3 rotationVelocity = new Vector3(0f,0f,0f);

    float rotationY = 0F;
	float rotationX = 0F;

	FPSMovement fpsMove;


    public float Sensitivity
    {
        get
        {
            return sensitivityX;
        }
        set
        {
            sensitivityX = value;
            sensitivityY = value;
        }
    }

    void Update()
    {
        if (CharacterState.Control_State == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
        {
            if (axes == RotationAxes.MouseXAndY)
            {

                /*if (fpsMove != null)
                {
                    Vector3 parentX = fpsMove.transform.localEulerAngles;
                    parentX.y += Input.GetAxis("Mouse X") * sensitivityX;
                    fpsMove.transform.localEulerAngles = parentX;
                }
                else
                {
                    rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                }*/

				//if(smoothing)
				//{
				//	Debug.Log("SMOOOOOOOOTH");

				//	rotationX = 0;

				//	rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

				//	rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				//	rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

				//	Vector3 targetPosition = new Vector3(-rotationY, rotationX, transform.localEulerAngles.z); 

				//	transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, targetPosition, ref rotationVelocity, smoothValue);

				//	//rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				//	//rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				//	//rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

				//	//rotationVelocity = new Vector2(-rotationY, rotationX);

				//	//transform.localEulerAngles += rotationVelocity;
				//	//rotationVelocity = Vector2.zero;
				//	//rotationVelocity *= smoothValue;

				//	//transform.localEulerAngles = Vector3.Lerp(new Vector3(-rotationY, rotationX, transform.localEulerAngles.z), transform.localEulerAngles, smoothValue);
				//}
				//else
				//{

					rotationX = 0;

					rotationX = transform.localEulerAngles.y + Input.GetAxis(InputKeyWords.MOUSE_X) * sensitivityX;

					rotationY += Input.GetAxis(InputKeyWords.MOUSE_Y) * sensitivityY;
					rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

					transform.localEulerAngles = new Vector3(-rotationY, rotationX, transform.localEulerAngles.z);
				//}
            }

            // This is not used currently
            /*else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }*/
        }
    }

    void Start()
    {
        _camera = Camera.main;
        if (transform.GetComponentInParent<FPSMovement>() != null)
        {
            fpsMove = transform.GetComponentInParent<FPSMovement>();
        }
        //if(!networkView.isMine)
        //enabled = false;

        // Make the rigid body not change rotation
        //if (rigidbody)
        //rigidbody.freezeRotation = true;
    }
}