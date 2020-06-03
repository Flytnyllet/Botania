using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
	[SerializeField] GoodButton craftButton = null;
	[SerializeField] GoodButton potionButton = null;

	// Update is called once per frame
	void Update()
	{
		HighlightButton(craftButton);
		HighlightButton(potionButton);
	}

	void HighlightButton(GoodButton button)
	{
		if (button.ButtonHighlighted() && button.isBaseSize)
		{
			button.transform.localScale = Vector3.one * 1.1f;
			button.isBaseSize = false;
		}
		else if (!button.ButtonHighlighted() && !button.isBaseSize)
		{
			button.transform.localScale = Vector3.one * 1.0f;
			button.isBaseSize = true;
		}
	}
}
