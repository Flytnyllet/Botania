using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Timer
{
    [SerializeField, Range(0, 1000)] float _time;
    [SerializeField, Range(0.01f, 1000)] float _duration;

    public float Time
    {
        get { return _time; }
        set
        {
            if (value >= _time)
                _time = value;
            else
                throw new Exception("Time can only count up!");
        }
    }

    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    public Timer(float duration, float time = 0)
    {
        if (duration <= 0)
            throw new Exception("Duration can not be 0 or less");
        else
            _duration = duration;
        if (time < 0)
            throw new Exception("Time can not be less than 0");
        else
            _time = time;
    }

    public Timer(Timer timer)
    {
        _time = timer._time;
        _duration = timer._duration;
    }

    public bool Expired()
    {
        return _time > _duration;
    }

    public void Reset()
    {
        _time = 0;
    }

    public float Ratio()
    {
        return _time / _duration;
    }
}
