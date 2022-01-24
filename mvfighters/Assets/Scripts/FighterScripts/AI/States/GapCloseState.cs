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
    public int percentSuccess;
    public override State RunCurrentState()
    {
        bool nerf = Random.Range(0, 100) > percentSuccess;
        if (nerf)
        {
            return this;
        }
        FighterS opponent = MainScript.instance.fm.p1Script;
        MainScript.instance.fm.stateMachine.scriptToUse.inputVector = new Vector2(-opponent.transform.forward.z, 0);
        MainScript.instance.fm.stateMachine.scriptToUse.Movement( -1, true);
        isEnemyAttacking = opponent.IsAttacking();
        isInAttackRange = !(MainScript.instance.fm.CheckDistanceBetweenPlayer() > 2f);
        if (isEnemyAttacking)
            return blockState;
        if (isInAttackRange)
            return attackState;
        return this;
    }
}
