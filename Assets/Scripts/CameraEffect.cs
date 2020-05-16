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
    const int MAX_EFFECTS = 10;
    RenderTexture[] _tempRenderTextures = new RenderTexture[MAX_EFFECTS];
    public List<Material> _materials = new List<Material>();
    PostProcessVolume _ppVolume;
    Camera _camera;

    //awful static Action which should be faster than the central EventManager,
    //This is only used because when used it's run every frame
    public static Action Renders;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I)) EventManager.TriggerEvent(EventNameLibrary.VISSION_POTION, new EventParameter { floatParam = 5, floatParam2 = 20 });
    //    if (Input.GetKeyDown(KeyCode.O)) Shader.SetGlobalFloat("gEmissionMult", 1);
    //}

    private void OnPreRender()
    {
        Matrix4x4 screenToWorld = (_camera.projectionMatrix * _camera.worldToCameraMatrix).inverse;
        Shader.SetGlobalMatrix("gScreenToWorld", screenToWorld);
        //Controlling the rendering of portals which are subscribed to Renderers from PortalCameraController
        if (Renders != null)
            Renders.Invoke();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_materials.Count == 0)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            int i = 0;
            Graphics.Blit(source, _tempRenderTextures[i], _materials[i]);
            for (i = 1; i < _materials.Count && i < MAX_EFFECTS; i++)
            {
                Debug.Log(i);
                Graphics.Blit(_tempRenderTextures[i - 1], _tempRenderTextures[i], _materials[i]);
            }
            Graphics.Blit(_tempRenderTextures[i - 1], destination);
            //Graphics.Blit(source, _tempRenderTextures[0], _materials[0]);
            //for (int i = 1; i < _materials.Count && i < MAX_EFFECTS; i++)
            //{
            //    Graphics.BlitMultiTap(_tempRenderTextures[0], _tempRenderTextures[0], _materials[i]);
            //}
            //Graphics.Blit(_tempRenderTextures[0], destination);
        }
    }
    private void Awake()
    {
        Debug.Log(RenderSettings.ambientLight);
        _ppVolume = GetComponent<PostProcessVolume>();
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
        Shader.SetGlobalFloat("gEmissionMult", 1);
    }
    private void Start()
    {
        for (int i = 0; i < MAX_EFFECTS; i++)
        {
            _tempRenderTextures[i] = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, (int)_camera.depth);
        }
    }
    //Some events for activating effects
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.DRINK_POTION, ActivateEffect);
        EventManager.Subscribe(EventNameLibrary.SPEED_INCREASE, SpeedDistortion);
        //EventManager.Subscribe(EventNameLibrary.SUPER_HEARING, HearingEffect);
        EventManager.Subscribe(EventNameLibrary.INVISSIBLE, InvissibilityEffect);
        EventManager.Subscribe(EventNameLibrary.LIGHTNING_STRIKE, LightningStrikeEffect);
        EventManager.Subscribe(EventNameLibrary.VISSION_POTION, SetGlobalEmissionMult);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.DRINK_POTION, ActivateEffect);
        EventManager.UnSubscribe(EventNameLibrary.SPEED_INCREASE, SpeedDistortion);
        //EventManager.UnSubscribe(EventNameLibrary.SUPER_HEARING, HearingEffect);
        EventManager.UnSubscribe(EventNameLibrary.INVISSIBLE, InvissibilityEffect);
        EventManager.UnSubscribe(EventNameLibrary.LIGHTNING_STRIKE, LightningStrikeEffect);
        EventManager.UnSubscribe(EventNameLibrary.VISSION_POTION, SetGlobalEmissionMult);
    }


    public void ActivateEffect(Material material)
    {
        _materials.Add(material);
    }
    //Set an effect for a certain amount of time
    public void ActivateEffect(Material material, float time)
    {
        _materials.Add(material);
        ActionDelayer.RunAfterDelayAsync(() => { _materials.Remove(material); }, time);
    }
    void ActivateEffect(EventParameter eventParam)
    {
        if (eventParam.materialParam != null)
        {
            Material material = eventParam.materialParam;
            material.SetFloat("_Lerp", 0);
            _materials.Add(material);
            StartCoroutine(LerpInCameraEffect(material, eventParam.floatParam2, false));
            ActionDelayer.RunAfterDelay(() => { StartCoroutine(LerpInCameraEffect(material, eventParam.floatParam2, true)); }, eventParam.floatParam);
        }
    }
    //floatParam = _potionDuration,
    //floatParam2 = _potionCameraEffectFadeTime
    IEnumerator LerpInCameraEffect(Material mat, float lerpTime, bool remove)
    {
        float time = 0;
        while (time < lerpTime)
        {
            if (remove)
            {
                mat.SetFloat("_Lerp", 1 - time / lerpTime);
            }
            else
            {
                mat.SetFloat("_Lerp", time / lerpTime);
            }
            time += Time.deltaTime;
            yield return null;
        }
        if (remove)
        {
            _materials.Remove(mat);
        }
    }


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

    void LightningStrikeEffect(EventParameter param)
    {
        StartCoroutine(LightningFlash(param.floatParam, param.floatParam2));
    }
    IEnumerator LightningFlash(float flashTime, float flashStrenght)
    {
        Debug.Log("lightning Strike");
        ColorGrading colGrad;
        if (_ppVolume.profile.TryGetSettings(out colGrad))
        {
            colGrad.postExposure.value = flashStrenght;
            float time = 0;
            while (time < flashTime && WorldState.Instance.IsRaining)
            {
                colGrad.postExposure.value = Mathf.Lerp(flashStrenght, 0, time / flashTime);
                time += Time.deltaTime;
                yield return null;
            }
            colGrad.postExposure.value = 0;
        }
    }

    void SetGlobalEmissionMult(EventParameter param)
    {
        StartCoroutine(LerpGlobalEmissionMultiplier(param.floatParam, param.floatParam2));
        StartCoroutine(LerpCromaShift(param.floatParam));
        WorldState.Instance.ChangeFogThickness(param.floatParam);
    }
    IEnumerator LerpGlobalEmissionMultiplier(float lerpTime, float targetValue)
    {
        float time = 0;
        float startValue = Shader.GetGlobalFloat("gEmissionMult");
        Debug.Log(startValue);
        while (time < lerpTime)
        {
            Shader.SetGlobalFloat("gEmissionMult", Mathf.Lerp(startValue, targetValue, time / lerpTime));
            time += Time.deltaTime;
            yield return null;
        }
        Shader.SetGlobalFloat("gEmissionMult", targetValue);
    }
    IEnumerator LerpCromaShift(float lerpTime)
    {
        ChromaticAberration chroma;
        if (_ppVolume.profile.TryGetSettings(out chroma))
        {
            float time = 0;
            float startCromaAn = Mathf.Round(chroma.intensity.value);
            float targetValue = 1 - startCromaAn;
            while (time < lerpTime)
            {
                time += Time.deltaTime;
                chroma.intensity.value = Mathf.Lerp(startCromaAn, targetValue, time / lerpTime);

                yield return null;
            }
        }
    }
}
