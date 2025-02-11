﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

[Serializable]
public class CharacterStats
{
    [SerializeField]
    private float _baseValue;
    public float BaseValue
    {
        get { return _baseValue; }
        set { _baseValue = Value; }
    }

    public virtual float Value
    {
        get
        {
            return CalculateFinalValue();
            //if (isModified || BaseValue != lastBaseValue)
            //{
            //    lastBaseValue = BaseValue;
            //    _value = CalculateFinalValue();
            //    isModified = false;
            //}
            //return _value;
        }
    }

    protected bool isModified = true;
    protected float _value;
    protected float lastBaseValue = float.MinValue; //in case of recalculating BaseValue

    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public CharacterStats()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public CharacterStats(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

    //public virtual void AddModifier(StatModifier mod)
    //{
    //    isModified = true;
    //    statModifiers.Add(mod);
    //    statModifiers.Sort(CompareOrder);
    //}
    public virtual void AddModifier(StatModifier mod, float time)
    {
        float baseVal = BaseValue;
        Debug.Log("Added stat effect, increasing the stat type " + mod.Type + " by " + mod.Value + $" for {time} seconds");
        isModified = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareOrder);
        Task.Delay(TimeSpan.FromSeconds(time)).ContinueWith(previous =>
        {
            try
            {
                //RemoveModifier(mod); //Jag har fuckat lite saker här, sorry /Jens
                statModifiers.Remove(mod);
                isModified = false;
                Debug.Log(statModifiers.Count);
                Debug.Log("Removed stat effect on the stat type " + mod.Type);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }


    public List<StatModifier> GetStatModifiers()
    {
        return statModifiers;
    }

    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isModified = false;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiers(object source)
    {
        bool allRemoved = false;

        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isModified = true;
                statModifiers.RemoveAt(i);
            }
        }
        return allRemoved;
    }

    protected virtual int CompareOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; //no prio 
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = 0;
        float totalPercentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatType.Flat)
            {
                finalValue += statModifiers[i].Value;
            }
            else if (mod.Type == StatType.PercentageAdd)
            {
                //Not applied to final, add as variable
                totalPercentAdd += mod.Value;

                //iterate list and add all mods of same type until another type i encountered || end of list
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatType.PercentageAdd)
                {
                    finalValue *= 1 + totalPercentAdd;
                    totalPercentAdd = 0;
                }
            }
            else if (mod.Type == StatType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4) + BaseValue;
    }
}
