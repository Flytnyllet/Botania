using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCameraController : MonoBehaviour
{
    Camera _playerCamera;
    [SerializeField] Camera _portalCamera;
    [SerializeField] PortalCameraController _targetPortal;
    [SerializeField] MeshRenderer _screen;
    [SerializeField] Shader _shader;
    RenderTexture _viewTexture;


    // Start is called before the first frame update
    void Awake()
    {
        _playerCamera = Camera.main; //This searches on tags and is therefore slow, should be considered for change
        _portalCamera.fieldOfView = _playerCamera.fieldOfView;
        _portalCamera.enabled = false;
    }

    void CreateViewTexture()
    {
        if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
        {
            if (_viewTexture != null)
            {
                _viewTexture.Release();
            }
            _viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            

            _portalCamera.targetTexture = _viewTexture;

            _targetPortal.SetViewTexture(_viewTexture);
        }
    }

    public void SetViewTexture(RenderTexture texture)
    {
        _screen.material.SetTexture("_MainTex", texture);
        _screen.material.shader = _shader;
    }
    private void Update()
    {
        Render();
    }
    public void Render()
    {
        _screen.enabled = false;
        CreateViewTexture();
        // make portal camera transform relatively the same as target portal camera.
        var m = transform.localToWorldMatrix * _targetPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
        _portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        _portalCamera.Render();
        _screen.enabled = true;
    }
}
