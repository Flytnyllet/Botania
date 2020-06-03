using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleFlower : MonoBehaviour
{
    bool _IsVissible { get => CharacterState.IsAbilityFlagActive(ABILITY_FLAG.VISSION) || CharacterState.IsAbilityFlagActive(ABILITY_FLAG.CALM_ALL_FLOWERS); }
    Collider _collider;
    bool _pickedUp = false;
    [SerializeField] MeshRenderer[] _renderer;
    [SerializeField] Texture2D _visibleTex;
    [SerializeField] Texture2D _invisibleTex;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    void SetVissibility(EventParameter param = null)
    {
        _collider.enabled = (_IsVissible && !_pickedUp);
        foreach (MeshRenderer renderer in _renderer)
        {
            renderer.material.SetTexture("_Alpha", (_IsVissible && !_pickedUp) ? _visibleTex : _invisibleTex);
        }
    }
    public void Pickup()
    {
        _pickedUp = true;
    }
    private void OnEnable()
    {
        SetVissibility();
        EventManager.Subscribe(EventNameLibrary.VISSION_POTION, SetVissibility);
        EventManager.Subscribe(EventNameLibrary.CALMING_POTION, SetVissibility);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.VISSION_POTION, SetVissibility);
        EventManager.UnSubscribe(EventNameLibrary.CALMING_POTION, SetVissibility);
    }
}
