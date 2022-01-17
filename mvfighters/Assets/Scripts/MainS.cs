using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainS : MonoBehaviour
{
    public FighterM fm;

    public UIManager um;

    public InputManager player1;

    public InputManager player2;

    public static MainS instance = null;

    public GameState state;

    //private PlayerInput controls;

    private bool paused = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        player1 = new InputManager();
        player2 = new InputManager();
        player1.GamePadScheme.PickDevicesFrom(Gamepad.all);
        player2.GamePadScheme.PickDevicesFrom(Gamepad.all);
        PrepareInputManager(player1, 1);
        state = GameState.Menu;
        //controls = fm.player1.GetComponent<PlayerInput>();
        um.StartMenuUpdating();
    }

    public void PrepareInputManager(InputManager player, int playerPort)
    {
        //player1.Combat1.Enable();
        player1.Combat1.A.performed += context => fm.UseMove(1, MoveType.A);
        player1.Combat1.B.performed += context => fm.UseMove(1, MoveType.B);
        player1.Combat1.C.performed += context => fm.UseMove(1, MoveType.C);
        player1.Combat1.MotionF.performed += context => fm.UseMove(1, MoveType.MotionF);
        player1.Combat1.MotionB.performed += context => fm.UseMove(1, MoveType.MotionB);
        player1.Combat1.Grab.performed += context => fm.UseMove(1, MoveType.Grab);
        player1.Combat1.Dash.performed += context => fm.PerformDash(1);

        //player2.Combat2.Enable();
        player2.Combat2.A.performed += context => fm.UseMove(2, MoveType.A);
        player2.Combat2.B.performed += context => fm.UseMove(2, MoveType.B);
        player2.Combat2.C.performed += context => fm.UseMove(2, MoveType.C);
        player2.Combat2.MotionF.performed += context => fm.UseMove(2, MoveType.MotionF);
        player2.Combat2.MotionB.performed += context => fm.UseMove(2, MoveType.MotionB);
        player2.Combat2.Grab.performed += context => fm.UseMove(2, MoveType.Grab);
        player2.Combat2.Dash.performed += context => fm.PerformDash(2);

        //player1.MenuMovement1.Enable();
        player1.MenuMovement1.Cancel.performed += context => um.css.CancelSelection(1);
        player1.MenuMovement1.ExtraButton1.performed += context => um.css.RandomizeSelection(1);
        //player2.MenuMovement2.Enable();
        player2.MenuMovement2.Cancel.performed += context => um.css.CancelSelection(2);
        player2.MenuMovement2.ExtraButton1.performed += context => um.css.RandomizeSelection(2);
        
        player1.UI.Cancel.performed += context => um.CancelSelection();
        player1.UI.Submit.performed += context => um.Submit();
    }

    public void GameStart(bool twoPlayer, GameState previousState)
    {
        DisableMenuControls();
        player1.Combat1.Enable();
        if (twoPlayer)
            player2.Combat2.Enable();
        switch (previousState)
        {
            case GameState.Css:
                state = GameState.Combat;
                break;
            case GameState.NetworkCss:
                state = GameState.NetworkCombat;
                break;
            case GameState.TrainingCss:
                state = GameState.TrainingCombat;
                break;
        }
    }

    public void DisableMenuControls()
    {
        if (player1.UI.enabled)
            player1.UI.Disable();
        if (player1.MenuMovement1.enabled)
            player1.MenuMovement1.Disable();
        if (player2.MenuMovement2.enabled)
            player2.MenuMovement2.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Menu)
        {
            if (!um.canvas.isActiveAndEnabled)
                um.canvas.enabled = true;
            um.UpdateMenu();
        }

        if (state is GameState.Css or GameState.NetworkCss or GameState.TrainingCss)
        {
            um.UpdateCSS();
        }

        if (state is GameState.Combat or GameState.TrainingCombat or GameState.NetworkCombat)
        {
            fm.UpdateObjects();

            um.UpdateInGameUI();
            
            if (paused)
            {
                um.ActivatePauseMenu();
            }
            else
            {
                um.DeactivatePauseMenu();
            }
        }

        if (state is GameState.ResultScreen)
        {
            um.UpdateResultScreen();
        }

        // if (paused)
        //um.temp.GetComponentInChildren<TMP_Text>().text = player1.Combat1.Grab.name + " = " + player1.Combat1.Grab;
    }

    public void StartRebindProcess()
    {
        player1.Combat1.Grab.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f)
            .Start();
    }

    public void SetPause(bool boolean)
    {
        paused = boolean;
    }
    
    public bool GetPause()
    {
        return paused;
    }
    
}