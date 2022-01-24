using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public bool isEnemyAttacking;
    public bool isKnockDown;
    public bool isInAttackRange;
    public bool opponentKnockedDown;
    public GapCloseState gapCloseState;
    public AttackState attackState;
    public BlockState blockState;
    public int percentSuccess;

    public override State RunCurrentState()
    {
        bool nerf = Random.Range(0, 100) > percentSuccess;
        if (nerf)
        {
            return this;
        }
        isKnockDown = MainScript.instance.fm.stateMachine.scriptToUse.IsKnockedDown();
        Vector2 movement = isKnockDown ? new Vector2(1, 0) : new Vector2(0, 0);
        MainScript.instance.fm.stateMachine.scriptToUse.inputVector = movement;
        MainScript.instance.fm.stateMachine.scriptToUse.Movement(-1, true);
        FighterS opponent = MainScript.instance.fm.p1Script;
        opponentKnockedDown = opponent.IsKnockedDown();
        isEnemyAttacking = opponent.IsAttacking();
        isInAttackRange = !(MainScript.instance.fm.CheckDistanceBetweenPlayer() > 2f);
        if (isKnockDown)
            return this;
        if (opponentKnockedDown)
            return this;
        if (isEnemyAttacking)
            return blockState;
        if (isInAttackRange)
            return attackState;
        return gapCloseState;
    }
}