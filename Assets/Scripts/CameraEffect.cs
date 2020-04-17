using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;

//This only works if placed directly on the camera which is supposed to run the effect
//Can be activated by calling "StartCameraEffect" in the eventmanager
public class CameraEffect : MonoBehaviour
{
    const string EVENT_NAME = "StartCameraEffect";
    Material _material = null;
    public static Action Renders; //awful static Action which should be faster than the central EventManager

    private void OnPreRender()
    {
        //Controlling the rendering of portals which are subscribed to Renderers from PortalCameraController
        if (Renders != null)
        Renders.Invoke();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            Graphics.Blit(source, destination, _material);
        }
    }
    //Some events for activating effects
    private void OnEnable()
    {
        EventManager.Subscribe(EVENT_NAME, ActivateEffect);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EVENT_NAME, ActivateEffect);
    }


    public void DeactivateEffect()
    {
        _material = null;
    }

    public void ActivateEffect(Material material)
    {
        _material = material;
    }
    //Set an effect for a certain amount of time
    public void ActivateEffect(Material material, float time)
    {
        _material = material;
        Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromSeconds(time));
            _material = null;
        });
    }
    void ActivateEffect( EventParameter eventParam)
    {
        _material = eventParam.materialParam;
        Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromSeconds(eventParam.floatParam));
            _material = null;
        });
    }
    //public void ActivateEffect(float time)
    //{
    //    _active = true;
    //    Task.Run(async () =>
    //    {
    //        await Task.Delay(System.TimeSpan.FromSeconds(time));
    //        _active = false;
    //    });
    //}


    //Detta är för att kontrollera culling för portallerna

}
