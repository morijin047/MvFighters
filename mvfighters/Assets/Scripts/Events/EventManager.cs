using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    static public DamageListener damageEvent;

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
}
