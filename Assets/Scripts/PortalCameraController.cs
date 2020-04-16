using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(MeshRenderer))]
public class PortalCameraController : MonoBehaviour
{
    const float EPSILON = 5e-06f;
    Camera _playerCamera;
    Camera _internalCamera;

    [SerializeField] PortalCameraController _targetPortal;
    [SerializeField] Shader _shader;
    MeshRenderer _screen;
    RenderTexture _texture;
    bool _active = true;
    bool _playerInside = false;
    object _lock = new object();

    public float nearClipLimit = 0.2f;
    // Start is called before the first frame update
    void Awake()
    {
        _playerCamera = Camera.main; //This searches on tags and is therefore slow, should be considered for change
        _internalCamera = GetComponentInChildren<Camera>();
        _internalCamera.fieldOfView = _playerCamera.fieldOfView;
        _internalCamera.depth = _playerCamera.depth;
        _internalCamera.enabled = false;
        _screen = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        CameraEffect.Renders += Render;  //awful subscriptions to a static Action
    }
    private void OnDisable()
    {
        CameraEffect.Renders -= Render;  //awful subscriptions to a static Action
    }

    private void Start()
    {
        //Creates Texture for Quad
        if (_texture == null || _texture.width != Screen.width || _texture.height != Screen.height)
        {
            if (_texture != null)
            {
                _texture.Release();
            }
            _texture = new RenderTexture(Screen.width, Screen.height, 0);
            _internalCamera.targetTexture = _texture;
            _targetPortal._screen.material.SetTexture("_MainTex", _texture);
            _targetPortal._screen.material.shader = _shader;
        }
    }

    bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    Vector3 RotateOnY(Vector3 vec, float degrees)
    {
        float x = vec.x;
        float z = vec.z;
        float cos = Mathf.Cos(degrees);
        float sin = Mathf.Sin(degrees);
        vec.x = x * cos - sin * z;
        vec.z = x * sin + z * cos;

        return vec;
    }


    public void Render()
    {

        if (!VisibleFromCamera(_screen, _playerCamera))
        {
            _screen.enabled = false;
        }

        var m = transform.localToWorldMatrix * _targetPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
        //Vector3 pos = m.GetColumn(3);
        _internalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        SetNearClipPlane();
        _internalCamera.Render();
        if (!_playerInside)
        {
            _screen.enabled = true;
        }
    }

    public void LockPortal(float time)
    {
        lock (_lock)
        {
            _active = false;
            var task = Task.Run(async () =>
            {
                await Task.Delay(System.TimeSpan.FromSeconds(1.0f));
                _active = true;
            });
        }
    }
    void SetNearClipPlane()
    {
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - _internalCamera.transform.position));

        Vector3 camSpacePos = _internalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = _internalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) - EPSILON;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            _internalCamera.projectionMatrix = _playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            _internalCamera.projectionMatrix = _playerCamera.projectionMatrix;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _screen.enabled = true;
        _playerInside = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        _screen.enabled = false;
        _playerInside = true;
        //Make sure you only enter from one side
        Vector3 offset = transform.position - other.transform.position;
        if (_active == true && other.tag == "Player" && Vector3.Dot(offset, transform.forward) > 0)
        {
            LockPortal(1.0f);
            _targetPortal.LockPortal(1.0f);
            _internalCamera.enabled = false;
            var m = _targetPortal.transform.worldToLocalMatrix * transform.localToWorldMatrix *  other.transform.localToWorldMatrix;
            //fattar inte varför, men objektet kan bara flyttas om det stängs av...
            other.gameObject.SetActive(false);
            other.transform.position = _targetPortal.transform.position - offset;
            //Quaternion.Angle(transform.rotation, _targetPortal.transform.rotation)
            //Rotate around the target portal so that the player gets the same relative position as to the portal they enter
            other.transform.RotateAround(_targetPortal.transform.position, _targetPortal.transform.up, _targetPortal.transform.eulerAngles.y-transform.eulerAngles.y);
            other.gameObject.SetActive(true);
        }
    }
}
