using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Timer
{
    public static Task RunAfterDelay(this Action ToRun, float Delay)
    {
        return Task.Delay(TimeSpan.FromSeconds(Delay)).ContinueWith(previous =>
        {
            try
            {
                ToRun();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }
}
