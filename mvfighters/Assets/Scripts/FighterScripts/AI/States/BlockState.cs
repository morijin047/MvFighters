using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : State
{
    public IdleState idleState;
    public bool isEnemyAttacking;
    public override State RunCurrentState()
    {
        MainS.instance.fm.stateMachine.scriptToUse.ForceAction("GuardAll");
        FighterS opponent = MainS.instance.fm.p1Script;
        isEnemyAttacking = opponent.IsAttacking();
        if (isEnemyAttacking)
            return this;
        MainS.instance.fm.stateMachine.scriptToUse.ForceAction("Idle");
        return idleState;
    }
}
