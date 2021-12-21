using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterM : MonoBehaviour
{
    public GameObject player1;

    public GameObject player2;

    private FighterS p1Script;

    private FighterS p2Script;

    public bool twoPlayer;

    public void StarGame(GameObject p1, GameObject p2, bool twoplayer)
    {
        player1 = p1;
        player1 = Instantiate(player1);
        player2 = p2;
        player2 = Instantiate(player2);
        p1Script = player1.GetComponent<FighterS>();
        p2Script = player2.GetComponent<FighterS>();
        p1Script.SetPort(1);
        p2Script.SetPort(2);
        twoPlayer = twoplayer;
    }

    public void UpdateObjects()
    {
        p1Script.Movement(MainS.instance.player1.Combat1.Move.ReadValue<Vector2>());
        p2Script.Movement(MainS.instance.player2.Combat2.Move.ReadValue<Vector2>());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainS.instance.GetPause())
            {
                MainS.instance.SetPause(false);
            }
            else
            {
                MainS.instance.SetPause(true);
            }
            
        }
    }

    public void UseMove(int playerPort, MoveType moveType)
    {
        switch (playerPort)
        {
            case 1:
                p1Script.Attack(moveType);
                break;
            case 2:
                p2Script.Attack(moveType);
                break;
        }
    }

    public void MatchOver(bool forced)
    {
        if (forced)
        {
            Destroy(player1);
            Destroy(player2);
            MainS.instance.player1.Combat1.Disable();
            if (MainS.instance.player2.Combat2.enabled)
                MainS.instance.player2.Combat2.Disable();
        }
    }
}