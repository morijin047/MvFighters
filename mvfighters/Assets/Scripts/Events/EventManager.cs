using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    static public DamageListener damageEvent;

    static public RoundEndListener roundEndEvent;

    public static void AddDamageListener(DamageListener listener)
    {
        if (damageEvent == null)
        {
            damageEvent = listener;
        }
        else
        {
            damageEvent += listener;
        }
    }

    public static void InvokeDamage(DamageEventArg eventArg)
    {
        damageEvent.Invoke(eventArg);
    }
    
    public static void AddRoundEndListener(RoundEndListener listener)
    {
        if (roundEndEvent == null)
        {
            roundEndEvent = listener;
        }
        else
        {
            roundEndEvent += listener;
        }
    }

    public static void InvokeRoundEnd(RoundEndEventArg eventArg)
    {
        roundEndEvent.Invoke(eventArg);
    }
}
