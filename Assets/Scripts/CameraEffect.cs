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
    public static string CAMERA_EFFECT_EVENT_NAME = "StartCameraEffect";
    Material _material = null;

    //awful static Action which should be faster than the central EventManager,
    //This is only used because when used it's run every frame
    public static Action Renders; 

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
        EventManager.Subscribe(CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
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
        ActionDelayer.RunAfterDelayAsync(() => { _material = null; }, time);
    }
    void ActivateEffect(EventParameter eventParam)
    {
        _material = eventParam.materialParam;
        ActionDelayer.RunAfterDelayAsync(() => { _material = null; }, eventParam.floatParam);
        
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
