using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public bool isEnemyHit;
    public bool opponentKnockedDown;
    public IdleState idleState;
    public MoveType currentMove;
    public int percentSuccess;

    public override State RunCurrentState()
    {
        bool nerf = Random.Range(0, 100) > percentSuccess;
        if (nerf)
        {
            return this;
        }
        if (currentMove == null)
            currentMove = MoveType.A;
        FighterS opponent = MainScript.instance.fm.p1Script;
        isEnemyHit = opponent.IsInHitStun();
        if (isEnemyHit && !MainScript.instance.fm.stateMachine.scriptToUse.IsAttacking())
        {
            switch (currentMove)
            {
                case MoveType.A:
                    currentMove = MoveType.B;
                    break;
                case MoveType.B:
                    currentMove = MoveType.C;
                    break;
            }
        }
        else
        {
            currentMove = MoveType.A;
        }
        MainScript.instance.fm.stateMachine.scriptToUse.Attack(currentMove);
        opponentKnockedDown = opponent.IsKnockedDown();
        if (opponentKnockedDown)
            return idleState;
        if (isEnemyHit)
            return this;
        return idleState;
    }
}