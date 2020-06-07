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
    public override bool PotionEffectStart(FPSMovement playerMovement)
    {
        //Awful hack for last minute fixes
        ABILITY_FLAG flag = CharacterState.GetFlagFromString(effect);
        bool AbillityActive;
        if (flag == ABILITY_FLAG.LEVITATE)
        {
            AbillityActive = (CharacterState.IsAbilityFlagActive(ABILITY_FLAG.LEVITATE) || CharacterState.IsAbilityFlagActive(ABILITY_FLAG.SLOWFALL));
        }
        else if (flag == ABILITY_FLAG.TELEPORT)
        {
            AbillityActive = (playerMovement.MayTeleport || CharacterState.IsAbilityFlagActive(flag));
        }
        else
        {
            AbillityActive = CharacterState.IsAbilityFlagActive(flag);
        }


        if (!AbillityActive)
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
                EventManager.TriggerEvent(EventNameLibrary.CALMING_POTION,
                    new EventParameter());
                ActionDelayer.RunAfterDelay(() =>
                {
                    EventManager.TriggerEvent(EventNameLibrary.CALMING_POTION,
                    new EventParameter());
                }, duration);
                break;
            case ABILITY_FLAG.VISSION:
                EventManager.TriggerEvent(EventNameLibrary.VISSION_POTION,
                    new EventParameter { floatParam = 8, floatParam2 = 6 });
                ActionDelayer.RunAfterDelay(() =>
                {
                    EventManager.TriggerEvent(EventNameLibrary.VISSION_POTION,
                        new EventParameter { floatParam = 2, floatParam2 = 1 });
                }, duration);
                break;
            case ABILITY_FLAG.TELEPORT:
                EventManager.TriggerEvent(EventNameLibrary.TELEPOT, new EventParameter());
                break;
            default:
                break;
        }
    }

}
