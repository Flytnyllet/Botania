using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct Velocity
{
    public Transform transform;
    public Vector3 direction;
}
public class TrashMultiMoverScript : MonoBehaviour
{
    public static TrashMultiMoverScript Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    static List<Velocity> _gObjects = new List<Velocity>();
    static List<Action<float>> tasks = new List<Action<float>>();

    //public void Subscribe(Velocity tran)
    //{
    //    _gObjects.Add(tran);
    //}
    //public void UnSubscribe(Velocity tran)
    //{
    //    _gObjects.Remove(tran);
    //}

    public void Subscribe(Action<float> task)
    {
        tasks.Add(task);
    }
    public void UnSubscribe(Action<float> task)
    {
        tasks.Remove(task);
    }

    private void FixedUpdate()
    {
        float f = Time.deltaTime;
        Parallel.For(0, tasks.Count, (int i) => { tasks[i].Invoke(f); });
        //for (int i = 0; i < _gObjects.Count; i++)
        //{
        //    //_gObjects[i].transform.position += _gObjects[i].direction * Time.deltaTime;
        //}
    }



}
