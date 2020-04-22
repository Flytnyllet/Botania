using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatType
{
	Speed, Jump, Gravity
}

public class SpeedPotion : Potion_Template
{
	float flat;
	float factor;
	float duration;
	CharacterStatType type;

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
	public SpeedPotion(CharacterStatType type, float factor, float flat, float duration)
	{
		this.factor = factor;
		this.flat = flat;
		this.duration = duration;
		this.type = type;
	}

	public override void PotionEffectStart(FPSMovement p)
	{
		switch (type)
		{
			case CharacterStatType.Jump:
				p._jumpForce.AddModifier(new StatModifier(flat, StatType.Flat, this), duration);
				p._jumpForce.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
				break;
			case CharacterStatType.Gravity:
				p._gravity.AddModifier(new StatModifier(flat, StatType.Flat, this), duration);
				p._gravity.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
				break;

			default:
				p._speed.AddModifier(new StatModifier(flat, StatType.Flat, this), duration);
				p._speed.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
				break;
		}
	}

	public override void PotionEffectEnd(FPSMovement p)
	{
		p._speed.RemoveAllModifiers(this);
	}
}
