using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPotion
{
    public void SpeedPot(FPSMovement p)
    {
        //simpler code (preference)
        p._speed.AddModifier(new StatModifier(20, StatType.Flat, this),3.0f);
        p._speed.AddModifier(new StatModifier(0.3f, StatType.PercentMult, this));
    }

    public void SpeedPotionEnd(FPSMovement p)
    {
        p._speed.RemoveAllModifiers(this);
    }
}
