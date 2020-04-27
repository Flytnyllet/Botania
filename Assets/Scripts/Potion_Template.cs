using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Potion_Template
{
	//abstract public void PotionEffectStart();
	abstract public bool PotionEffectStart(FPSMovement p);
	//abstract public void PotionEffectEnd();
	abstract public void PotionEffectEnd(FPSMovement p);
}
