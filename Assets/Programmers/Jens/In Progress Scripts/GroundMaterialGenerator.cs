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
    [SerializeField][Range(0,1)] float _mainTexStart = 0.8f;
    [SerializeField] Texture2D _emission;
    [SerializeField] Layer _layer;
    //[SerializeField] Material _material;
    // Update is called once per frame
    object obj;

    public Material MakeMaterial(int size, Vector2 pos, MeshRenderer renderer)
    {
        Material _material = new Material(_shader);
        Texture2D noiseTex = new Texture2D(size, size, TextureFormat.R8, false);
        noiseTex.wrapMode = TextureWrapMode.Clamp;
        var colour = noiseTex.GetRawTextureData();
        Color[] colorMap = new Color[1];
        Task.Run(() =>
        {
            try
            {

                colorMap = new Color[size * size];
                float[,] noise = Noise.GenerateNoiseMap(size, size, _noiseDetailLevel, _layer.GetNoise.NoiseSettingsDataMerge, pos);
                noise = Noise.Clamp(noise, _layer.GetNoise);
                //byte[] noiseByte = new byte[size * size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        // colour[y * size + x] = (byte)(255 * noise[x, y]);
                        colorMap[y * size + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, 1, noise[x, y]));

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
        _material.SetTexture("_AltTex", _layer.GetTexture());
        _material.SetTexture("_MainTex", _mainTex);
        _material.SetTexture("_Emission", _emission);
        _material.SetColor("baseColor", _layer.GetTint());
        _material.SetFloat("baseTextureScale", _layer.GetTextureScale());
        _material.SetFloat("mainTexStop", _layer.GetMainStop());
        _material.SetFloat("mainTexStart", _mainTexStart);

        return _material;

    }

    [System.Serializable]
    public struct Layer
    {
        [SerializeField] NoiseSettingsData _noise;
        [SerializeField] Texture2D _texture;
        [SerializeField] [Range(0, 1)] float _mainTexStop;
        [SerializeField, Range(0, 1)] float _textureStrenght;
        [SerializeField] Color _tint;
        [SerializeField, Range(0, 1)] float _textureScale;

        public NoiseSettingsData GetNoise { get => _noise; }
        public Texture2D GetTexture() { return _texture; }
        public Color GetTint() { return _tint; }
        public float GetTextureScale() { return _textureScale; }
        public float GetTextureStrenght() { return _textureStrenght; }
        public float GetMainStop() { return _mainTexStop; }
    }

}
