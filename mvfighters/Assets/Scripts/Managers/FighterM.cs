using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterM : MonoBehaviour
{
    public GameObject player1;

    public GameObject player2;

    public FighterS p1Script;

    public FighterS p2Script;

    public Camera mainCam;

    public Vector2 worldSizeDim;

    [HideInInspector] public Rect worldSize;

    public float cameraSpeed;

    public float startPos;

    public float maxDistance;

    public bool twoPlayer;

    public bool roundStart = false;

    private IEnumerator roundStartCoroutine;
    public float timeBeforeRoundStart;

    public StateManager stateMachine;

    public GameObject stage;
    
    [HideInInspector] public int idP1;
    [HideInInspector] public int idP2;
    public void StarGame(string p1, string p2, GameObject stage, bool twoplayer)
    {
        this.stage = stage;
        this.stage = Instantiate(stage);
        player1 = MainS.instance.GetPooledFighter(p1);
        player1.SetActive(true);
        player1.transform.parent = null;
        player1.GetComponent<Rigidbody>().isKinematic = false;
        player2 = MainS.instance.GetPooledFighter2(p2);
        player2.SetActive(true);
        player2.transform.parent = null;
        player2.GetComponent<Rigidbody>().isKinematic = false;
        p1Script = player1.GetComponent<FighterS>();
        p2Script = player2.GetComponent<FighterS>();
        p1Script.SetPort(1);
        p2Script.SetPort(2);
        twoPlayer = twoplayer;
        if (!twoPlayer && MainS.instance.state == GameState.Combat)
        {
            stateMachine.AssignAIPrefabScript(p2Script);
        }
        Vector3 stageTransform = stage.transform.position;
        worldSize = new Rect(- worldSizeDim.x, 0, worldSizeDim.x, worldSizeDim.y);
    }

    public void EnableControls()
    {
        if (!MainS.instance.player1.Combat1.enabled)
            MainS.instance.player1.Combat1.Enable();
        if (twoPlayer)
        {
            if (!MainS.instance.player2.Combat2.enabled)
                MainS.instance.player2.Combat2.Enable();
        }
        else
        {
            if (!stateMachine.aiActive)
                stateMachine.aiActive = true;
        }
    }

    public void DisableControls()
    {
        MainS.instance.player1.Combat1.Disable();
        if (twoPlayer)
        {
            if (MainS.instance.player2.Combat2.enabled)
                MainS.instance.player2.Combat2.Disable();
        }
        else
        {
            if (stateMachine.aiActive)
                stateMachine.aiActive = false;
        }
    }

    public void RoundStart()
    {
        mainCam.transform.position = new Vector3(4, 1.5f, 0);
        ResetPositions();
        MainS.instance.um.inGame.DisplayRoundText("GET READY!");
        SFXManager.sfxInstance.audio.PlayOneShot(MainS.instance.um.inGame.narratorVoices[0]);
        p1Script.PlayMatchIntroAnimation();
        p2Script.PlayMatchIntroAnimation();
        DisableControls();
        roundStartCoroutine = RoundStartCoroutine();
        StartCoroutine(roundStartCoroutine);
    }

    public void ResetPositions()
    {
        p1Script.ResetPosition(startPos);
        p2Script.ResetPosition(startPos);
    }

    public IEnumerator RoundStartCoroutine()
    {
        while (true)
        {
            p1Script.TurnPlayer();
            p2Script.TurnPlayer();
            yield return new WaitForSeconds(timeBeforeRoundStart);
            roundStart = true;
            MainS.instance.um.inGame.DisplayRoundText("FIGHT!");
            SFXManager.sfxInstance.audio.PlayOneShot(MainS.instance.um.inGame.narratorVoices[1]);
            MainS.instance.um.inGame.StartCoroutineRoundText(2f);
            EnableControls();
            ResetPositions();
            StopCoroutine(roundStartCoroutine);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (stage != null)
        {
            Vector3 stageTransform = stage.transform.position;
            Gizmos.DrawLine(new Vector3(0, stageTransform.y, worldSize.x), new Vector3(0, worldSize.height, worldSize.x));
            Gizmos.DrawLine(new Vector3(0, worldSize.height, worldSize.x), new Vector3(0, worldSize.height, worldSize.width));
            Gizmos.DrawLine(new Vector3(0, worldSize.height, worldSize.width), new Vector3(0, stageTransform.y, worldSize.width));
            Gizmos.DrawLine(new Vector3(0, stageTransform.y, worldSize.width), new Vector3(0, stageTransform.y, worldSize.x));
        }
    }

    public void UpdateObjects()
    {
        CameraFollow(player1.transform.position);
        CameraFollow(player2.transform.position);
        CameraCenter();
        Vector3 camPosition = mainCam.transform.position;
        mainCam.transform.position = new Vector3(camPosition.x,
            camPosition.y,
            Mathf.Clamp(camPosition.z, worldSize.x, worldSize.width));
        if (roundStart)
        {
            if (MainS.instance.player1.Combat1.Move.ReadValue<Vector2>() == Vector2.zero)
                p1Script.inputVector = Vector2.zero;
            var gamepads = Gamepad.all;
            var arrayDevice = MainS.instance.player1.Combat1.Move.controls;
            idP1 = arrayDevice[0].device.deviceId;
            
            if (MainS.instance.portController.keyboardOnly1)
                idP1 = -1;
            p1Script.Movement( idP1, false);
            if (twoPlayer || MainS.instance.state == GameState.TrainingCombat)
            {
                if (MainS.instance.player2.Combat2.Move.ReadValue<Vector2>() == Vector2.zero)
                    p2Script.inputVector = Vector2.zero;
                var arrayDevice2 = MainS.instance.player2.Combat2.Move.controls;
                idP2 = arrayDevice2[1].device.deviceId;
                if (MainS.instance.portController.keyboardOnly2)
                    idP2 = -1;
                p2Script.Movement(idP2, false);
            }
            p1Script.AnimationCalculation();
            p2Script.AnimationCalculation();
            if (MainS.instance.state == GameState.Combat && !twoPlayer)
                stateMachine.UpdateStates();
        }
    }

    public void PauseButton(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }
        
        if (MainS.instance.GetPause())
        {
            MainS.instance.SetPause(false);
        }
        else
        {
            MainS.instance.SetPause(true);
            SFXManager.sfxInstance.PlayPauseSound();
        }
    }

    public void CameraFollow(Vector3 playerTransform)
    {
        Vector3 camPosition = mainCam.transform.position;
        if (playerTransform.z >=camPosition.z + maxDistance / 2)
            mainCam.transform.position = Vector3.Lerp(camPosition,
                 camPosition + new Vector3(0, 0, 0.2f), cameraSpeed * Time.deltaTime);
        if (playerTransform.z <= camPosition.z - maxDistance / 2)
            mainCam.transform.position = Vector3.Lerp(camPosition,
                camPosition + new Vector3(0, 0, -0.2f), cameraSpeed * Time.deltaTime);
    }

    public void CameraCenter()
    {
        Vector3 camPosition = mainCam.transform.position;
        float middlePoint = (player1.transform.position.z + player2.transform.position.z) / 2;
        mainCam.transform.position = Vector3.Lerp(camPosition,
             new Vector3(camPosition.x, camPosition.y, middlePoint), cameraSpeed * Time.deltaTime);
        
    }

    public void UseMove(int playerPort, MoveType moveType, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }
        
        //foreach motionbufferArray
        //condition forward
        // moveType = MoveType.MotionF;
        //condition backward
        // moveType = MoveType.MotionB;
        //condition super
        // moveType = MoveType.Super;
       
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

    public void PlayerMove(int playerPort, InputAction.CallbackContext context)
    {
        Debug.Log(context.control.device.name);
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
            
            idP1 = context.control.device.deviceId;
            p1Script.inputVector = context.ReadValue<Vector2>();
            
            return;
        }

        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
            idP2 = context.control.device.deviceId;
            p2Script.inputVector = context.ReadValue<Vector2>();
            return;
        }
    }
    
    

    public void PerformDash(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }
        switch (playerPort)
        {
            case 1:
                p1Script.Dash();
                break;
            case 2:
                p2Script.Dash();
                break;
        }
    }


    public void MatchOver()
    {
        DisableControls();
    }

    public void DeleteCurrentFighter()
    {
       player1.SetActive(false);
       player2.SetActive(false);
        Destroy(stage);
    }

    public void CheckWinner()
    {
        p1Script.TurnPlayer();
        p2Script.TurnPlayer();
        int winningPort;
        if (p1Script.currentHp > p2Script.currentHp)
        {
            winningPort = p1Script.GetPort();
            p1Script.ChangeAnimationState(CharacterState.Idle);
            p2Script.KnockDown();
        }
        else
        {
            winningPort = p2Script.GetPort();
            p2Script.ChangeAnimationState(CharacterState.Idle);
            p1Script.KnockDown();
        }

        MainS.instance.um.inGame.StartDelayedCoroutineRoundText(2f, 2f, winningPort,
            MainS.instance.um.inGame.narratorVoices[4]);
        roundStart = false;
        EventManager.InvokeRoundEnd(new RoundEndEventArg(winningPort));
    }

    public void ForceHp(int hp1, int hp2)
    {
        p1Script.currentHp = hp1 * p1Script.stats.maxHp / 100;
        p2Script.currentHp = hp2 * p2Script.stats.maxHp / 100;
    }

    public float CheckDistanceBetweenPlayer()
    {
        return Vector3.Distance(p1Script.transform.position, p2Script.transform.position);
    }
}