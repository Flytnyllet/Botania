using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.START_RAIN, startCloudCoRoutine);
        EventManager.Subscribe(EventNameLibrary.STOP_RAIN, startCloudCoRoutine);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.START_RAIN, startCloudCoRoutine);
        EventManager.UnSubscribe(EventNameLibrary.STOP_RAIN, startCloudCoRoutine);
    }

    void startCloudCoRoutine(EventParameter param)
    {
        //StartCoroutine(ChangeCloudThickness(param));
    }
    IEnumerator ChangeCloudThickness(float lerpTime, float highSmoothStep, float lowSmoothStep)
    {

        while (true)
        {

            yield return null;
        }


    }
}
