using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Trigger : MonoBehaviour
{
    //private System_Music music;
    //private GameObject friend;
    //private GameObject playerDir;

    //private void Awake()
    //{
    //    music = FindObjectOfType<System_Music>();
    //    friend = GameObject.FindGameObjectWithTag("AIFriend");
    //    playerDir = GameObject.FindGameObjectWithTag("MainCamera");
    //}

    //private void Update()
    //{
    //    Vector3 dir = (playerDir.transform.position - friend.transform.position).normalized;
    //    float dot = Vector3.Dot(dir, playerDir.transform.forward);

    //    if (dot > -1f && dot < -0.7f)
    //    {
    //        music.SetParameter("zero_Escape", 0f);
    //        music.SetParameter("zero", 6f);
    //        Debug.Log("Player saw Friend");
    //        Destroy(gameObject);
    //    }
    //}
}
