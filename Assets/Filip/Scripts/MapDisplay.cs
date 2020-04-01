using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [Header("Drop")]

    [SerializeField] Renderer _textureRenderer;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData)
    {
        _meshFilter.sharedMesh = meshData.CreateMesh();

        //ÄNDRA SENARE HITTA SÄTT
        _meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().GetScale();

    }
}
