using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatType
{
    Speed, Jump, Gravity, Invisibility, Hearing
}

public class SpeedPotion : Potion_Template
{
    float flat;
    float factor;
    float duration;
    CharacterStatType type;
    StatModifier potionEffect;

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

    public override bool PotionEffectStart(FPSMovement p)
    {
        switch (type)
        {
            case CharacterStatType.Jump:
                if (!p._jumpForce.GetStatModifiers().Exists(x => x.Source == this))
                {
                    potionEffect = new StatModifier(flat, StatType.Flat, this);
                    p._jumpForce.AddModifier(potionEffect, duration);
                    p._jumpForce.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
                    return true;
                }
                break;
            case CharacterStatType.Gravity:
                if (!p._gravity.GetStatModifiers().Exists(x => x.Source == this))
                {
                    potionEffect = new StatModifier(flat, StatType.Flat, this);
                    p._gravity.AddModifier(potionEffect, duration);
                    p._gravity.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
                    return true;
                }
                break;
            case CharacterStatType.Invisibility:
                //Denna har ingen funktion just nu för att blockera att man tar flera potions, men det är ok for now
                CharacterState.AddAbilityFlag(ABILITY_FLAG.INVISSIBLE, 30);
                break;

            case CharacterStatType.Hearing:
                CharacterState.AddAbilityFlag(ABILITY_FLAG.SUPERHEARING, 30);
                break;


            default:
                if (!(p._speed.GetStatModifiers().Exists(x => x.Source == this)))
                {
                    potionEffect = new StatModifier(flat, StatType.Flat, this);
                    p._speed.AddModifier(potionEffect, duration);
                    p._speed.AddModifier(new StatModifier(factor, StatType.PercentMult, this), duration);
                    EventParameter param = new EventParameter() { intParam = -50, floatParam = 1 };
                    EventManager.TriggerEvent(EventNameLibrary.SPEED_INCREASE, param);
                    param.intParam = 0;
                    ActionDelayer.RunAfterDelay(() =>
                    {
                        EventManager.TriggerEvent(EventNameLibrary.SPEED_INCREASE, param);
                    }, duration);
                    return true;
                }
                break;
        }
        return false;
    }

    public override void PotionEffectEnd(FPSMovement p)
    {
        p._speed.RemoveAllModifiers(this);
    }
}
