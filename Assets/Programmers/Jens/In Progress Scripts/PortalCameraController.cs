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
    object _lock = new object();
    
    public float nearClipLimit = 0.2f;
    // Start is called before the first frame update
    void Awake()
    {
        _playerCamera = Camera.main; //This searches on tags and is therefore slow, should be considered for change
        _internalCamera = GetComponentInChildren<Camera>();
        _internalCamera.fieldOfView = _playerCamera.fieldOfView;
        _internalCamera.enabled = false;
        _screen = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        CameraEffect.Renders += Render;  //Ignore this, it's just an awful subscriptions to a static Action
    }
    private void OnDisable()
    {
        CameraEffect.Renders -= Render;  //Ignore this, it's just an awful subscriptions to a static Action
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

    public void Render()
    {
        if (!VisibleFromCamera(_screen, _playerCamera))
        {
            _screen.enabled = false;
        }
        var m = transform.localToWorldMatrix * _targetPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
        _internalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        SetNearClipPlane();
        _internalCamera.Render();
        _screen.enabled = true;
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
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
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
    private void OnTriggerEnter(Collider other)
    {
        if (_active == true && other.tag == "Player")
        {
            LockPortal(1.0f);
            _targetPortal.LockPortal(1.0f);
            Vector3 m = transform.position - other.transform.position;
            _internalCamera.enabled = false;
            //fattar inte varför, men objektet kan bara flyttas om det stängs av...
            other.gameObject.SetActive(false);
            other.transform.position = _targetPortal.transform.position - m;
            other.transform.rotation = _targetPortal.transform.rotation;
            other.gameObject.SetActive(true);
        }
    }
}
