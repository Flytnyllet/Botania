using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System;

[CreateAssetMenu(menuName = "MaterialMaker")]
public class GroundMaterialGenerator : ScriptableObject
{
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] Shader _shader;
    [SerializeField] int _noiseDetailLevel = 1;
    [SerializeField] Texture2D _mainTex;
    [SerializeField] Texture2D _emission;
    [SerializeField] [Range(0, 1)] float _mainTexStart = 0.8f;
    [SerializeField] [Range(0, 1)] float _mainTexStop = 0.7f;
    [SerializeField] float _mainTexScale = 4f;
    const int LAYER_COUNT = 4;
    [SerializeField] Layer[] _layers = new Layer[LAYER_COUNT];
    //[SerializeField] Material _material;
    // Update is called once per frame
    object obj;

    void OnValidate()
    {
        if (_layers.Length != LAYER_COUNT)
        {
            Debug.LogWarning("Don't change the '_layers' field's array size!");
            Array.Resize(ref _layers, LAYER_COUNT);
        }
    }

    public Material MakeMaterial(int size, Vector2 pos, MeshRenderer renderer)
    {
        Material _material = new Material(_shader);
        Texture2D noiseTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        noiseTex.wrapMode = TextureWrapMode.Clamp;
        var colour = noiseTex.GetRawTextureData();
        Color[] colorMap = new Color[1];
        Task.Run(() =>
        {
            try
            {

                colorMap = new Color[size * size];
                float[][,] noises = new float[LAYER_COUNT][,];
                for (int i = 0; i < LAYER_COUNT; i++)
                {
                    if (_layers[i].GetNoise != null)
                    {
                        noises[i] = Noise.GenerateNoiseMap(size, size, _noiseDetailLevel, _layers[i].GetNoise.NoiseSettingsDataMerge, pos);
                        noises[i] = Noise.Clamp(noises[i], _layers[i].GetNoise);
                    }
                    else
                    {
                        noises[i] = new float[size, size];
                    }
                }
                float f = 0;
                int a = 0, b = 0;
                int aa = 0;
                int bb = 0;
                //byte[] noiseByte = new byte[size * size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float large = Math.Max(noises[0][x, y], noises[1][x, y]);
                        large = Math.Max(large, noises[2][x, y]);
                        if (large > f)
                        {
                            f = large;
                            aa = x;
                            bb = y;
                        }
                        //ThreadedDataRequester.AddToCallbackQueue(() => { Debug.Log(noises[1][x, y]); });
                        // colour[y * size + x] = (byte)(255 * noise[x, y]);
                        colorMap[y * size + x].r = noises[0][x, y];
                        colorMap[y * size + x].g = noises[1][x, y];
                        colorMap[y * size + x].b = noises[2][x, y];
                        colorMap[y * size + x].a = noises[3][x, y];
                    }
                }
                //colour = noiseByte;
                //Debug.Log(noiseByte[1]);
                ThreadedDataRequester.AddToCallbackQueue(() =>
                                {
                                    //noiseTex.LoadRawTextureData(colour);
                                    noiseTex.SetPixels(colorMap);
                                    noiseTex.Apply();
                                    //MaterialPropertyBlock prop = new MaterialPropertyBlock();
                                    //prop.SetTexture("_NoiseTextures", noiseTex);
                                    //renderer.SetPropertyBlock(prop);
                                    _material.SetTexture("_NoiseTexture", noiseTex);
                                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        _material.SetTexture("_AltTex0", _layers[0].GetTexture);
        _material.SetTexture("_AltTex1", _layers[1].GetTexture);
        _material.SetTexture("_AltTex2", _layers[2].GetTexture);
        _material.SetTexture("_AltTex3", _layers[3].GetTexture);
        _material.SetTexture("_MainTex", _mainTex);
        _material.SetTexture("_Emission", _emission);
        _material.SetFloat("baseTextureScale", _mainTexScale);
        _material.SetFloatArray("startStep", _layers.Select(x => x.GetStepValues.startStep).ToArray());
        _material.SetFloatArray("endStep", _layers.Select(x => x.GetStepValues.endStep).ToArray());
        _material.SetFloatArray("altTextureScale", _layers.Select(x => x.GetTextureScale).ToArray());
        _material.SetColorArray("altTextureColour", _layers.Select(x => x.GetColour).ToArray());
        _material.SetFloat("mainTexStop", _mainTexStop);
        _material.SetFloat("mainTexStart", _mainTexStart);

        return _material;
    }

    [System.Serializable]
    public struct Layer
    {
        [SerializeField] NoiseSettingsData _noise;
        //[Header("Note: Alpha starts at 0")]
        [SerializeField] Color _colour;
        [SerializeField] Texture2D _texture;
        [SerializeField] float _textureScale;
        [SerializeField] float _startStep;
        [SerializeField] float _endStep;

        public Color GetColour { get => _colour; }
        public NoiseSettingsData GetNoise { get => _noise; }
        public Texture2D GetTexture { get => _texture; }
        public float GetTextureScale { get => _textureScale; }
        public (float startStep, float endStep) GetStepValues { get => (_startStep, _endStep); }
    }

}
