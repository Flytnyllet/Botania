using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Flat,
    PercentageAdd,
    PercentMult,
}

public class StatModifier
{
    public readonly float Value;
    public readonly StatType Type;
    public readonly int Order;
    public readonly object Source;

    public StatModifier(float value, StatType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }
    // automatically calls value, type and int representation 
    public StatModifier(float value, StatType type) : this(value, type, (int)type, null) { } 

    public StatModifier(float value, StatType type, int order) : this(value, type, order, null) { } 

    public StatModifier(float value, StatType type, object source) : this(value, type, (int)type, source) { } 
}
