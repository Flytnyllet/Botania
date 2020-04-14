using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
    public void SpeedPot(Test p)
    {
        //simpler code (preference)
        p.Speed.AddModifier(new StatModifier(20, StatType.Flat, this));
        p.Speed.AddModifier(new StatModifier(0.3f, StatType.PercentMult, this));
    }

    public void SpeedPotionEnd(Test p)
    {
        p.Speed.RemoveAllModifiers(this);
    }
}
