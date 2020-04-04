using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

//This only works if placed directly on the camera which is supposed to run the effect
//Can be activated by calling "StartCameraEffect" in the eventmanager
public class CameraEffect : MonoBehaviour
{
    Material _material;

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material != null)
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

    //public void ActivateEffect(bool state)
    //{
    //    _active = state;
    //}
    public void ActivateEffect(Material material)
    {
        _material = material;
    }
    public void DeactivateEffect()
    {
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
}
