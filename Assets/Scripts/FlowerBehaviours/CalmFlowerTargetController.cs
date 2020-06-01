using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalmFlowerTargetController : MonoBehaviour
{
    public static CalmFlowerTargetController Instance;
    List<Transform> calmFlowers = new List<Transform>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("ValidReferenceException: Object reference set to an instance of an object");
            Destroy(this.gameObject);
        }
        SetClosestDistance();
    }

    public void Subscribe(Transform transform)
    {
        calmFlowers.Add(transform);
        SetClosestDistance();
    }
    public void Unsubscribe(Transform transform)
    {
        calmFlowers.Remove(transform);
        SetClosestDistance();
    }

    //Kan optimeras genom att bytas ut mot en sorterad stack eller någon skit, men har inte tid just nu.
    void SetClosestDistance()
    {
        Vector3 closest = new Vector4(0, 99999, 0, 0);
        float closestDistance = 99999;
        foreach (Transform tran in calmFlowers)
        {
            float tempDist = Vector3.Distance(Player.GetPlayerTransform().position, tran.position);
            if (tempDist < closestDistance)
            {
                closestDistance = tempDist;
                closest = tran.position;
            }
        }
        Shader.SetGlobalVector("gAudioPosition", closest);
    }
}
