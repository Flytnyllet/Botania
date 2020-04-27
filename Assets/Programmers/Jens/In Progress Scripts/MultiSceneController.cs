using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiSceneController : MonoBehaviour
{
    //
    //This is pretty barebones right now for the sake of showing that it works
    //Proper attention should be put on this **eventually**
    //
    //const string BOOK_SCENE = "BookScene";

    [SerializeField] string[] _sceneNames;


    void Start()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (string scene in _sceneNames)
        {
            StartCoroutine(AddScene(scene, activeScene));
        }
    }

    //Just making sure that these things are being loaded in without freezing the game
	IEnumerator AddScene(string sceneName, Scene mainScene)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SceneManager.SetActiveScene(mainScene);
    }
}
