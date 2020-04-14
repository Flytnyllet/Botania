using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceInteraction : MonoBehaviour, Interactable
{
    bool _activated = false;
    [SerializeField] float _meltTime = 1.0f;
    public bool Interact()
    {
        if (!_activated)
        {
            StartCoroutine(Melt());
            return true;
        }
        return false;
    }

    IEnumerator Melt()
    {
        float time = _meltTime;
        Vector3 startSize = transform.localScale;
        while (time > 0)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, startSize, time/ _meltTime);
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
