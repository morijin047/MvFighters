using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MainS : MonoBehaviour
{
    public FighterM fm;

    public UIManager um;

    public InputManager player1;
    
    public InputManager player2;

    public static MainS instance = null;

    public GameState state;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        player1 = new InputManager();
        player2 = new InputManager();
        PrepareInputManager(player1, 1);
        state = GameState.Combat;
        EventManager.AddRoundEndListener(RoundEnd);
    }

    public void PrepareInputManager(InputManager player, int playerPort)
    {
        player1.Combat1.Enable();
        player1.Combat1.A.performed += context => fm.UseMove(1, MoveType.A);
        player1.Combat1.B.performed += context => fm.UseMove(1, MoveType.B);
        player1.Combat1.C.performed += context => fm.UseMove(1, MoveType.C);
        player1.Combat1.MotionF.performed += context => fm.UseMove(1, MoveType.MotionF);
        player1.Combat1.MotionB.performed += context => fm.UseMove(1, MoveType.MotionB);
        player1.Combat1.Grab.performed += context => fm.UseMove(1, MoveType.Grab);
        player2.Combat2.Enable();
        player2.Combat2.A.performed += context => fm.UseMove(2, MoveType.A);
        player2.Combat2.B.performed += context => fm.UseMove(2, MoveType.B);
        player2.Combat2.C.performed += context => fm.UseMove(2, MoveType.C);
        player2.Combat2.MotionF.performed += context => fm.UseMove(2, MoveType.MotionF);
        player2.Combat2.MotionB.performed += context => fm.UseMove(2, MoveType.MotionB);
        player2.Combat2.Grab.performed += context => fm.UseMove(2, MoveType.Grab);
    }

    public void RoundEnd(RoundEndEventArg eventarg)
    {
        state = GameState.Menu;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Combat)
            fm.UpdateObjects();
    }
}
