using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPotion : Potion_Template
{
	string effect;
	float duration;

	public FlagPotion(string effect, float duration)
	{
		this.effect = effect;
		this.duration = duration;
	}
	public override bool PotionEffectStart(FPSMovement p)
	{
		CharacterState.AddAbilityFlag(effect, duration);
		return true;
	}

	public override void PotionEffectEnd(FPSMovement p)
	{
		p._speed.RemoveAllModifiers(this);
	}
}
