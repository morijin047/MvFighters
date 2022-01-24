using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : State
{
    public IdleState idleState;
    public bool isEnemyAttacking;
    public int percentSuccess;
    public override State RunCurrentState()
    {
        bool nerf = Random.Range(0, 100) > percentSuccess;
        if (nerf)
        {
            return this;
        }
        MainS.instance.fm.stateMachine.scriptToUse.ForceAction("GuardAll");
        FighterS opponent = MainS.instance.fm.p1Script;
        isEnemyAttacking = opponent.IsAttacking();
        if (isEnemyAttacking)
            return this;
        MainS.instance.fm.stateMachine.scriptToUse.ForceAction("Idle");
        return idleState;
    }
}
