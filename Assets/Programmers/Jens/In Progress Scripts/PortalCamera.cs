using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    MeshRenderer _screen;
    public void SetObscuringPortal(MeshRenderer screen)
    {
        _screen = screen;
    }
    private void OnPreRender()
    {
        //_screen.enabled = false;
    }
    private void OnPostRender()
    {
        //_screen.enabled = true;
    }
}
