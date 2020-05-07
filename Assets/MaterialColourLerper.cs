using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColourLerper : MonoBehaviour
{
    Material _material;
    [SerializeField] Color _startColour;
    [SerializeField] Color _targetColour;
    bool cloudState = true;
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void StartLerp(float lerpTime)
    {
        StartCoroutine(ColourLerp(lerpTime));
    }
    IEnumerator ColourLerp(float lerpTime)
    {
        Debug.Log(cloudState);
        Color colA = _startColour;
        Color colB = _targetColour;
        if (cloudState)
        {
            colA = _targetColour;
            colB = _startColour;
        }
        cloudState = !cloudState;
        float time = 0;
        while (time < lerpTime)
        {
            _material.SetColor("_CloudCol", Color.Lerp(colA, colB, time / lerpTime));
            time += Time.deltaTime;
            yield return null;
        }
    }
}
