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
        if (!CharacterState.IsAbilityFlagActive(effect))
        {
            CharacterState.AddAbilityFlag(effect, duration);
            AddAdditionalEffects();
            return true;
        }
        return false;
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
                EventParameter paramI = new EventParameter() { intParam = 2, floatParam = 20f, floatParam2 = 1.0f };
                EventManager.TriggerEvent(EventNameLibrary.INVISSIBLE, paramI);
                paramI.floatParam = 0.0f;
                paramI.floatParam2 = 0.01f;
                ActionDelayer.RunAfterDelay(() => { EventManager.TriggerEvent(EventNameLibrary.INVISSIBLE, paramI); }, duration);
                break;

            case ABILITY_FLAG.SUPERHEARING:
                //EventParameter param = new EventParameter() { intParam = 2, floatParam = 0.75f, floatParam2 = 1.0f };
                //EventManager.TriggerEvent(EventNameLibrary.SUPER_HEARING, param);
                //param.floatParam = 0.05f;
                //param.floatParam2 = 0.2f;
                //ActionDelayer.RunAfterDelay(() => {
                //EventManager.TriggerEvent(EventNameLibrary.SUPER_HEARING, param); }, duration);
                break;

            case ABILITY_FLAG.LEVITATE:


                ActionDelayer.RunAfterDelay(() => { CharacterState.AddAbilityFlag("SLOWFALL", 5f); }, duration);
                break;
            case ABILITY_FLAG.NULL:
                break;
            case ABILITY_FLAG.STONE:
				//EventManager.TriggerEvent(EventNameLibrary.STONED, new EventParameter { });
				break;
            case ABILITY_FLAG.SLOWFALL:
                break;
            case ABILITY_FLAG.CALM_ALL_FLOWERS:
                break;
            case ABILITY_FLAG.VISSION:
                EventManager.TriggerEvent(EventNameLibrary.VISSION_POTION,
                    new EventParameter { floatParam = 8, floatParam2 = 20 });
                ActionDelayer.RunAfterDelay(() =>
                {
                    EventManager.TriggerEvent(EventNameLibrary.VISSION_POTION,
                        new EventParameter { floatParam = 2, floatParam2 = 1 });
                }, duration);
                break;
            default:
                break;
        }
    }

}
