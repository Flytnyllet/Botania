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

    public Material _material = null;
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
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
    //Some events for activating effects
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
        EventManager.Subscribe(EventNameLibrary.SPEED_INCREASE, SpeedDistortion);
        EventManager.Subscribe(EventNameLibrary.SUPER_HEARING, HearingEffect);
        EventManager.Subscribe(EventNameLibrary.INVISSIBLE, InvissibilityEffect);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.CAMERA_EFFECT_EVENT_NAME, ActivateEffect);
        EventManager.UnSubscribe(EventNameLibrary.SPEED_INCREASE, SpeedDistortion);
        EventManager.UnSubscribe(EventNameLibrary.SUPER_HEARING, HearingEffect);
        EventManager.UnSubscribe(EventNameLibrary.INVISSIBLE, InvissibilityEffect);
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


    void InvissibilityEffect(EventParameter param)
    {
        StartCoroutine(InvissibleEffect((float)param.intParam, param.floatParam, param.floatParam2));
    }
    IEnumerator InvissibleEffect(float durration, float blueOffset, float ChromaticAb)
    {
        ChromaticAberration chroma;
        ColorGrading colourGrad;
        if (_ppVolume.profile.TryGetSettings(out chroma))
        {
            if (_ppVolume.profile.TryGetSettings(out colourGrad))
            {
                float time = 0;
                float startColour = colourGrad.mixerRedOutBlueIn.value;
                float startCromaAn = chroma.intensity.value;
                while (time < durration)
                {
                    time += Time.deltaTime;
                    colourGrad.mixerRedOutBlueIn.value = Mathf.Lerp(startColour, blueOffset, time / durration);
                    chroma.intensity.value = Mathf.Lerp(startCromaAn, ChromaticAb, time / durration);

                    yield return null;
                }
            }
        }
    }

    void HearingEffect(EventParameter param)
    {
        StartCoroutine(HearingEffectRoutine((float)param.intParam, param.floatParam, param.floatParam2));
    }
    IEnumerator HearingEffectRoutine(float duration, float noiseStrenght, float vigneteStrenght)
    {
        Grain grainLayer;
        Vignette vignetteLayer;
        if (_ppVolume.profile.TryGetSettings(out grainLayer))
        {
            if (_ppVolume.profile.TryGetSettings(out vignetteLayer))
            {
                float startVignetteValue = vignetteLayer.smoothness.value;
                float startGrainValue = grainLayer.intensity.value;
                float time = 0;
                while (time < duration)
                {
                    time += Time.deltaTime;
                    vignetteLayer.smoothness.value = Mathf.Lerp(startVignetteValue, vigneteStrenght, time / duration);
                    grainLayer.intensity.value = Mathf.Lerp(startGrainValue, noiseStrenght, time / duration);
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// intparam = targetDistortion,  floatParam = distort time to lerp
    /// </summary>
    /// <param name="param"></param>
    void SpeedDistortion(EventParameter param)
    {
        StartCoroutine(SpeedDistort((float)param.intParam, param.floatParam));
    }
    IEnumerator SpeedDistort(float targetDistort, float duration)
    {
        LensDistortion distortionLayer;
        if (_ppVolume.profile.TryGetSettings(out distortionLayer))
        {
            float distort = (targetDistort - distortionLayer.intensity.value) / duration;
            duration += Time.time;
            Debug.Log(distort);
            while (Time.time < duration)
            {
                distortionLayer.intensity.value += Time.deltaTime * distort;
                yield return null;
            }
            distortionLayer.intensity.value = targetDistort;
        }
    }
}
