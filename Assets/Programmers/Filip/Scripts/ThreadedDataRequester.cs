using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour
{
    static ThreadedDataRequester instance;

    private void Awake()
    {
        instance = FindObjectOfType<ThreadedDataRequester>(); //ÄNDRA SEN
    }

    Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();


    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate { instance.DataThread(generateData, callback); };

        new Thread(threadStart).Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();

        //If heightMaphreadqueue is being accessed by another thread, WAIT! :D 
        lock (_dataQueue)
        {
            _dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    private void Update()
    {
        if (_dataQueue.Count > 0)
        {
            for (int i = 0; i < _dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = _dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
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
