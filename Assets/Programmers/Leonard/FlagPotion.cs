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
        AddAdditionalEffects();
        return true;
    }

    public override void PotionEffectEnd(FPSMovement p)
    {
        p._speed.RemoveAllModifiers(this);
    }


    void AddAdditionalEffects()
    {
        ABILITY_FLAG flag = CharacterState.GetFlagFromString(effect);
        switch (flag)
        {
            case ABILITY_FLAG.INVISSIBLE:
                break;

            case ABILITY_FLAG.SUPERHEARING:
                break;

            default:
                break;
        }
    }

}
