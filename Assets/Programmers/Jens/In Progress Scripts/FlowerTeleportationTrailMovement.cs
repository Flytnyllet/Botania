using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTeleportationTrailMovement : MonoBehaviour
{
    [SerializeField] CharacterController _controller;
    [SerializeField] TrailRenderer _trail;
    [SerializeField] float _speed = 1;

    //Unity är retards och tvingar en göra en fet array för att använda deras fuckade TrailRenderer.GetPositions()
    Vector3[] _trailPositions = new Vector3[100];

    public IEnumerator followTarget(Transform target)
    {
        Debug.Log("hello");
        while (Vector3.Distance(this.transform.position, target.position) > 0.4)
        {
            Vector3 direction = (target.position - this.transform.position).normalized;
            _controller.Move(_speed * direction * Time.deltaTime);
            yield return null;
        }

        while ((_trail.GetPositions(_trailPositions)) > 2)
        {
            Vector3 direction = (target.position - this.transform.position).normalized;
            _controller.Move(_speed * direction * Time.deltaTime);
            yield return null;
        }

        this.transform.parent = target;
        this.gameObject.SetActive(false);
    }

}