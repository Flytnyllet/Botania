using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class ThreadedDataRequester : MonoBehaviour
{
    static ThreadedDataRequester instance;

    private void Awake()
    {
        instance = FindObjectOfType<ThreadedDataRequester>(); //ÄNDRA SEN

    }

    static ConcurrentQueue<Action> _callbacks = new ConcurrentQueue<Action>();
    static ConcurrentQueue<Action> _restrictedCallbacks = new ConcurrentQueue<Action>();
    //Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

    // // Har kommenterat ut för tillfället, då jag letar efter ett problem
    // // med att klienter drar 9GB ram för mig, vet inte om det är här än.

    //public static void RequestData(Func<object> generateData, Action<object> callback)
    //{
    //    ThreadStart threadStart = delegate { instance.DataThread(generateData, callback); };

    //    new Thread(threadStart).Start();
    //}

    //void DataThread(Func<object> generateData, Action<object> callback)
    //{
    //    object data = generateData();

    //    //If heightMaphreadqueue is being accessed by another thread, WAIT! :D 
    //    lock (_dataQueue)
    //    {
    //        _dataQueue.Enqueue(new ThreadInfo(callback, data));
    //    }
    //}

    public static void AddToCallbackQueue(Action callback)
    {
        try
        {
            _callbacks.Enqueue(callback);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public static void AddToRestrictedCallbackQueue(Action callback)
    {
        try
        {
            _restrictedCallbacks.Enqueue(callback);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void Update()
    {
        //if (_dataQueue.Count > 0)
        //{
        //    for (int i = 0; i < _dataQueue.Count; i++)
        //    {
        //        ThreadInfo threadInfo = _dataQueue.Dequeue();
        //        threadInfo.callback(threadInfo.parameter);
        //    }
        //}
        while (_callbacks.TryDequeue(out var callback))
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        int i = 10;
        while (_restrictedCallbacks.TryDequeue(out var callback) && i > 0)
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            i--;
        }
    }

    //IEnumerator Callback(Action<object> callback, object parameter, int waitAmount)
    //{
    //    float waitTime = UnityEngine.Random.Range(0, 1);
    //    yield return new WaitForSecondsRealtime(waitTime);
    //    callback(parameter);
    //}



    public static void RequestData(Func<object> dataGenerator, Action<object> callback)
    {
        Task.Run(() =>
        {
            try
            {
                var data = dataGenerator();

                _callbacks.Enqueue(() => callback(data));

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }




    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
