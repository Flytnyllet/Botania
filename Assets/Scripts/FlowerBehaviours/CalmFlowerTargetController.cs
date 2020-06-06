using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalmFlowerTargetController
{
    static List<Transform> calmFlowers = new List<Transform>();

    static CalmFlowerTargetController()
    {
        SetClosestDistance();
    }

    public static void Subscribe(Transform transform)
    {
        calmFlowers.Add(transform);
        SetClosestDistance();
    }
    public static void Unsubscribe(Transform transform)
    {
        calmFlowers.Remove(transform);
        SetClosestDistance();
    }

    //Kan optimeras genom att bytas ut mot en sorterad stack eller någon skit, men har inte tid just nu.
    static void SetClosestDistance()
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
