using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrashSceneLoader : MonoBehaviour
{
    static TrashSceneLoader instance;
    bool trashBool = true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (trashBool)
            {
                trashBool = false;
                SceneManager.LoadScene("TestPlane");
            }
            else
            {
                trashBool = true;
                SceneManager.LoadScene("TestScene");
            }
        }
    }
}
