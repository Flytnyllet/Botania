using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodButton : Button
{
	public bool isBaseSize = true;
    public bool ButtonHighlighted()
	{
		return IsHighlighted();
	}

	void Update()
	{
		HighlightButton(this);
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
