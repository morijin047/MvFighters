using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEventArg
{
    public int player;
    public Move move;
    
    public DamageEventArg(int player, Move move)
    {
        this.player = player;
        this.move = move;
    }
}

public delegate void DamageListener(DamageEventArg eventArg);
