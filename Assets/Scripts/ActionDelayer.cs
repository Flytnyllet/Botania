using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ActionDelayer
{
    /// <summary>
    /// Runs an Action on a background thread after a delay of f seconds
    /// </summary>
    public static Task RunAfterDelayAsync(Action toRun, float delay)
    {
        return Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(previous =>
        {
            try
            {
                toRun();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }

    /// <summary>
    /// Runs an Action on the main thread after a delay of f seconds
    /// </summary>
    public static Task RunAfterDelay(Action toRun, float delay)
    {
        return Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(previous =>
        {
            try
            {
                toRun();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            //Make sure this runs on the same thread which requested the delay
        },TaskScheduler.FromCurrentSynchronizationContext()); 
    }
    //public static void RunAfterDelay(Action toRun, float delay)
    //{
    //    TimerHolder timer = new GameObject("Timer").AddComponent<TimerHolder>();
    //    timer.StartCoroutine(timer.TimedAction(toRun, delay));
    //}
    //class TimerHolder : MonoBehaviour
    //{
    //    public IEnumerator TimedAction(Action toRun, float delay)
    //    {
    //        yield return new WaitForSeconds(delay);
    //        try
    //        {
    //            toRun();
    //            Destroy(this.gameObject);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError(e);
    //        }
    //    }
    //}
}

