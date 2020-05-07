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
                    }
                    else
                    {
                        noises[i] = new float[size, size];
                    }
                }
                //byte[] noiseByte = new byte[size * size];
                for (int y = 0; y < size; y++)
                {
                for (int x = 0; x < size; x++)
                {
                    //if (noises[0][x, y] < 0 || noises[0][x, y] > 1) ThreadedDataRequester.AddToCallbackQueue(() => { Debug.LogError("Noise Value out of range"); });
                    //ThreadedDataRequester.AddToCallbackQueue(() => { Debug.Log(noises[1][x, y]); });
                            // colour[y * size + x] = (byte)(255 * noise[x, y]);
                            colorMap[y * size + x].r = noises[0][x, y] * 255;
                        colorMap[y * size + x].g = noises[1][x, y] * 255;
                        colorMap[y * size + x].b = noises[2][x, y] * 255;
                        colorMap[y * size + x].a = noises[3][x, y] * 255;
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
                                    _material.SetTexture("_NoiseTextures", noiseTex);
                                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        _material.SetTexture("_AltTex0", _layers[0].GetTexture ?? new Texture2D(0, 0));
        _material.SetTexture("_AltTex1", _layers[1].GetTexture ?? new Texture2D(0, 0));
        _material.SetTexture("_AltTex2", _layers[2].GetTexture ?? new Texture2D(0, 0));
        _material.SetTexture("_AltTex3", _layers[3].GetTexture ?? new Texture2D(0, 0));
        _material.SetTexture("_MainTex", _mainTex);
        _material.SetTexture("_Emission", _emission);
        _material.SetFloat("baseTextureScale", _mainTexScale);
        _material.SetFloatArray("baseTextureScale", _layers.Select(x => x.GetTextureScale).ToArray());
        _material.SetFloat("mainTexStop", _mainTexStop);
        _material.SetFloat("mainTexStart", _mainTexStart);

        return _material;
    }

    [System.Serializable]
    public struct Layer
    {
        [SerializeField] NoiseSettingsData _noise;
        [SerializeField] Texture2D _texture;
        [SerializeField] float _textureScale;

        public NoiseSettingsData GetNoise { get => _noise; }
        public Texture2D GetTexture { get => _texture; }
        public float GetTextureScale { get => _textureScale; }
    }

}
