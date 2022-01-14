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

    public Rect worldSize;

    public float cameraSpeed;

    public float startPos;

    public float maxDistance;

    public bool twoPlayer;

    public bool roundStart = false;

    private IEnumerator roundStartCoroutine;
    public float timeBeforeRoundStart;

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

    public void EnableControls()
    {
        if (!MainS.instance.player1.Combat1.enabled)
            MainS.instance.player1.Combat1.Enable();
        if (twoPlayer)
        {
            if (!MainS.instance.player2.Combat2.enabled)
                MainS.instance.player2.Combat2.Enable();
        }
    }

    public void DisableControls()
    {
        MainS.instance.player1.Combat1.Disable();
        if (MainS.instance.player2.Combat2.enabled)
            MainS.instance.player2.Combat2.Disable();
    }

    public void RoundStart()
    {
        mainCam.transform.position = new Vector3(4, 1.5f, 0);
        p1Script.ResetPosition(startPos);
        p2Script.ResetPosition(startPos);
        MainS.instance.um.inGame.DisplayRoundText("GET READY!");
        SFXManager.sfxInstance.audio.PlayOneShot(MainS.instance.um.inGame.narratorVoices[0]);
        p1Script.PlayMatchIntroAnimation();
        p2Script.PlayMatchIntroAnimation();
        roundStartCoroutine = RoundStartCoroutine();
        StartCoroutine(roundStartCoroutine);
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
            StopCoroutine(roundStartCoroutine);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0, worldSize.y, worldSize.x), new Vector3(0, worldSize.height, worldSize.x));
        Gizmos.DrawLine(new Vector3(0, worldSize.y, worldSize.x), new Vector3(0, worldSize.y, worldSize.width));
        Gizmos.DrawLine(new Vector3(0, worldSize.height, worldSize.x),
            new Vector3(0, worldSize.height, worldSize.width));
        Gizmos.DrawLine(new Vector3(0, worldSize.y, worldSize.width),
            new Vector3(0, worldSize.height, worldSize.width));
    }

    public void UpdateObjects()
    {
        CameraFollow(player1.transform.position);
        CameraFollow(player2.transform.position);
        mainCam.transform.position = new Vector3(mainCam.transform.position.x,
            mainCam.transform.position.y,
            Mathf.Clamp(mainCam.transform.position.z, worldSize.x, worldSize.width));
        if (roundStart)
        {
            p1Script.Movement(MainS.instance.player1.Combat1.Move.ReadValue<Vector2>());
            p2Script.Movement(MainS.instance.player2.Combat2.Move.ReadValue<Vector2>());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
    }

    public void CameraFollow(Vector3 playerTransform)
    {
        if (playerTransform.z >= mainCam.transform.position.z + maxDistance / 2)
            MainS.instance.fm.mainCam.transform.position = Vector3.Lerp(mainCam.transform.position,
                mainCam.transform.position + new Vector3(0, 0, 0.2f), cameraSpeed * Time.deltaTime);
        if (playerTransform.z <= mainCam.transform.position.z - maxDistance / 2)
            MainS.instance.fm.mainCam.transform.position = Vector3.Lerp(mainCam.transform.position,
                mainCam.transform.position + new Vector3(0, 0, -0.2f), cameraSpeed * Time.deltaTime);
    }

    public void UseMove(int playerPort, MoveType moveType)
    {
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

    public void PerformDash(int playerPort)
    {
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
        Destroy(player1);
        Destroy(player2);
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
}