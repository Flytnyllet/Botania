using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;
using UnityEngine.Rendering.PostProcessing;

//This only works if placed directly on the camera which is supposed to run the effect
//Can be activated by calling "StartCameraEffect" in the eventmanager
public class CameraEffect : MonoBehaviour
{
    public static string CAMERA_EFFECT_EVENT_NAME = "StartCameraEffect";
    public static string CAMERA_SPEED_DISTORT = "SpeedDistort";

    Material _material = null;
    PostProcessVolume _ppVolume;


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
    private void Awake()
    {
        _ppVolume = GetComponent<PostProcessVolume>();
    }
    //Some events for activating effects
    private void OnEnable()
    {
        EventManager.Subscribe(CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
        EventManager.Subscribe(CAMERA_SPEED_DISTORT, SpeedDistortion);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
        EventManager.UnSubscribe(CAMERA_SPEED_DISTORT, SpeedDistortion);
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

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        Debug.Log("DERP");
    //                                   //intparam = targetDistortion,  floatParam = distort time to lerp
    //        EventParameter param = new EventParameter() { intParam = 40, floatParam = 2 };
    //        EventManager.TriggerEvent(CAMERA_SPEED_DISTORT, param);
    //        param.intParam = 0;
    //        ActionDelayer.RunAfterDelay(() => { EventManager.TriggerEvent(CAMERA_SPEED_DISTORT, param); }, 5);
    //    }
    //}



    /// <summary>
    /// intparam = targetDistortion,  floatParam = distort time to lerp
    /// </summary>
    /// <param name="param"></param>
    void SpeedDistortion(EventParameter param)
    {
        StartCoroutine(SpeedDistort((float)param.intParam, param.floatParam));
    }


    IEnumerator SpeedDistort(float targetDistort, float time)
    {
        LensDistortion distortionLayer;
        if (_ppVolume.profile.TryGetSettings(out distortionLayer))
        {
            float distort = (targetDistort - distortionLayer.intensity.value) / time;
            time += Time.time;
            Debug.Log(distort);
            while (Time.time < time)
            {
                distortionLayer.intensity.value += Time.deltaTime * distort;
                yield return null;
            }
            distortionLayer.intensity.value = targetDistort;
        }
    }
}
