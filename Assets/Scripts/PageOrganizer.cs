using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageOrganizer : MonoBehaviour
{
    public void NextPage()
	{
		PageLoader[] pages = transform.GetComponentsInChildren<PageLoader>();
		foreach (PageLoader page in pages)
		{
			Debug.Log("Found page " + page.gameObject.name);
			page.NextPage();
		}
	}

}
