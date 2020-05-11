using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}
	
	public void ExitGame()
	{
		if(Application.isEditor)
		{
			Debug.Log("Exiting");
		}
		Application.Quit();
	}
}
