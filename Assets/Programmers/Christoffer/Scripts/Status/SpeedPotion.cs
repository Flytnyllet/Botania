using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : Potion_Template
{
	float flat;
	float factor;
	float duration;
	public SpeedPotion(float factor, float duration)
	{
		this.factor = factor;
		this.duration = duration;
	}
	public SpeedPotion(float factor, float flat, float duration)
	{
		this.factor = factor;
		this.flat = flat;
		this.duration = duration;
	}
	public override void PotionEffectStart(FPSMovement p)
    {
        //simpler code (preference)
        p._speed.AddModifier(new StatModifier(flat, StatType.Flat, this), duration);
        p._speed.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
    }

    public override void PotionEffectEnd(FPSMovement p)
    {
        p._speed.RemoveAllModifiers(this);
    }
}
