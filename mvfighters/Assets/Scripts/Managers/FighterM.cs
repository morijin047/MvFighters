using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterM : MonoBehaviour
{
    public GameObject player1;

    public GameObject player2;

    private FighterS p1Script;

    private FighterS p2Script;

    private void Start()
    {
        p1Script = player1.GetComponent<FighterS>();
        p2Script = player2.GetComponent<FighterS>();
        p1Script.SetPort(1);
        p2Script.SetPort(2);
    }

    public void UpdateObjects()
    {
        p1Script.Movement(MainS.instance.player1.Combat1.Move.ReadValue<Vector2>());
        p2Script.Movement(MainS.instance.player2.Combat2.Move.ReadValue<Vector2>());
    }

    public void UseMove(int playerPort, MoveType moveType)
    {
        switch (playerPort)
        {
            case 1:
                // if (p1Script.CheckCrouch())
                // {
                //     p1Script.Attack(moveType);
                // }
                // else
                    p1Script.Attack(moveType);
                break;
            case 2:
                p2Script.Attack(moveType);
                break;
        }
    }
}