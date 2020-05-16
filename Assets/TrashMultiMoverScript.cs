using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
public struct Velocity
{
    public Transform tran;
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

    public void Subscribe(Velocity tran)
    {
        _gObjects.Add(tran);
    }
    public void UnSubscribe(Velocity tran)
    {
        _gObjects.Remove(tran);
    }
    object obj = new object();
    private void Update()
    {
        float f = Time.deltaTime;
        for (int i = 0; i < _gObjects.Count; i++)
        {
            _gObjects[i].tran.position += _gObjects[i].direction * f;
        }
    }



}
