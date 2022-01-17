using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapCloseState : State
{
    public bool isEnemyAttacking;
    public bool isInAttackRange;
    public IdleState idleState;
    public BlockState blockState;
    public AttackState attackState;
    public override State RunCurrentState()
    {
        FighterS opponent = MainS.instance.fm.p1Script;
        MainS.instance.fm.stateMachine.scriptToUse.Movement(new Vector2(-opponent.transform.forward.z,0));
        isEnemyAttacking = opponent.IsAttacking();
        isInAttackRange = !(MainS.instance.fm.CheckDistanceBetweenPlayer() > 2f);
        if (isEnemyAttacking)
            return blockState;
        if (isInAttackRange)
            return attackState;
        return this;
    }
}
