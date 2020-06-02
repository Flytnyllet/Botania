using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLibrary : MonoBehaviour
{
    public void SpeedPot(FPSMovement p)
    {
        //simpler code (preference)
        p._speed.AddModifier(new StatModifier(20, StatType.Flat, this), 3.0f);
        //p._speed.AddModifier(new StatModifier(0.3f, StatType.PercentMult, this));
    }

    public void SpeedPotionEnd(FPSMovement p)
    {
        p._speed.RemoveAllModifiers(this);
    }
    public void GravtityPot(FPSMovement p)
    {
        p._gravity.AddModifier(new StatModifier(10, StatType.Flat, this), 10.0f);
    }

    public void GravityPotEnd(FPSMovement p)
    {
        p._gravity.RemoveAllModifiers(this);
    }

    public void JumpPot(FPSMovement p)
    {
        p._jumpForce.AddModifier(new StatModifier(4, StatType.Flat, this), 10.0f);
    }

    public void JumpPotEnd(FPSMovement p)
    {
        p._jumpForce.RemoveAllModifiers(this);
    }
}
