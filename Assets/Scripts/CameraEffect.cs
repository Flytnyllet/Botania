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
    Material _material = null;
    public static Action Renders; //awful static Action

    private void OnPreRender()
    {
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
    private void OnEnable()
    {
        EventManager.Subscribe("StartCameraEffect", ActivateEffect);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe("startCameraEffect", ActivateEffect);
    }


    public void DeactivateEffect()
    {
        _material = null;
    }
    public void ActivateEffect(Material material)
    {
        _material = material;
    }
    public void ActivateEffect(Material material, float time)
    {
        _material = material;
        Task task = Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromSeconds(time));
        });
        _material = null;
    }
    void ActivateEffect( EventParameter eventParam)
    {
        _material = eventParam.materialParam;
        Task task = Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromSeconds(eventParam.floatParam));
        });
        _material = null;
    }
    //public void ActivateEffect(float time)
    //{
    //    _active = true;
    //    Task task = Task.Run(async () =>
    //    {
    //        await Task.Delay(System.TimeSpan.FromSeconds(time));
    //        _active = false;
    //    });
    //}


    //Detta är för att kontrollera culling för portallerna

}
