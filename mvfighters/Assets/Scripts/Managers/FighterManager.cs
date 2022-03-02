
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class FighterManager : MonoBehaviour
{
    [HideInInspector] public GameObject player1;

    [HideInInspector] public GameObject player2;

    [HideInInspector] public FighterS p1Script;

    [HideInInspector] public FighterS p2Script;

    public Camera mainCam;

    public Vector2 worldSizeDim;

    [HideInInspector] public Rect worldSize;

    public float cameraSpeed;

    public float startPos;

    public float maxDistance;

    [HideInInspector] public bool twoPlayer;

    [HideInInspector] public bool roundStart = false;

    private IEnumerator roundStartCoroutine;
    public float timeBeforeRoundStart;

    public StateManager stateMachine;

    [HideInInspector] public GameObject stage;
    
    [HideInInspector] public int idP1;
    [HideInInspector] public int idP2;

    public float repulsionForce;
    
    //Function that is called when closing the css and leaving the vs screen loading time
    public void StarGame(string p1, string p2, GameObject stage, bool twoplayer)
    {
        this.stage = stage;
        this.stage = Instantiate(stage);
        player1 = MainScript.instance.GetPooledFighter(p1);
        player1.SetActive(true);
        player1.transform.parent = null;
        player1.GetComponent<Rigidbody>().isKinematic = false;
        player2 = MainScript.instance.GetPooledFighter2(p2);
        player2.SetActive(true);
        player2.transform.parent = null;
        player2.GetComponent<Rigidbody>().isKinematic = false;
        p1Script = player1.GetComponent<FighterS>();
        p2Script = player2.GetComponent<FighterS>();
        p1Script.SetPort(1);
        p2Script.SetPort(2);
        twoPlayer = twoplayer;
        if (!twoPlayer && MainScript.instance.state == GameState.Combat)
        {
            stateMachine.AssignAIPrefabScript(p2Script);
        }
        Vector3 stageTransform = stage.transform.position;
        worldSize = new Rect(- worldSizeDim.x, 0, worldSizeDim.x, worldSizeDim.y);
    }

    //Enable the player(s) controls
    public void EnableControls()
    {
        if (!MainScript.instance.player1.Combat1.enabled)
            MainScript.instance.player1.Combat1.Enable();
        if (twoPlayer)
        {
            if (!MainScript.instance.player2.Combat2.enabled)
                MainScript.instance.player2.Combat2.Enable();
        }
        else
        {
            if (!stateMachine.aiActive)
                stateMachine.aiActive = true;
        }
    }

    //Disable the player(s) controls
    public void DisableControls()
    {
        MainScript.instance.player1.Combat1.Disable();
        if (twoPlayer)
        {
            if (MainScript.instance.player2.Combat2.enabled)
                MainScript.instance.player2.Combat2.Disable();
        }
        else
        {
            if (stateMachine.aiActive)
                stateMachine.aiActive = false;
        }
    }

    //All the cleanup that happens each start of a round
    public void RoundStart()
    {
        mainCam.transform.position = new Vector3(4, 1.5f, worldSize.x + worldSize.width);
        ResetPositions();
        MainScript.instance.um.inGame.DisplayRoundText("GET READY!");
        SFXManager.sfxInstance.audio.PlayOneShot(MainScript.instance.um.inGame.narratorVoices[0]);
        p1Script.PlayMatchIntroAnimation();
        p2Script.PlayMatchIntroAnimation();
        DisableControls();
        MainScript.instance.um.inGame.AssignCharacterIcons();
        roundStartCoroutine = RoundStartCoroutine();
        StartCoroutine(roundStartCoroutine);
    }

    //reset the position of all the fighters currently in the scene and centers the cam on them in order to not get clipped by the camera limitation
    public void ResetPositions()
    {
        p1Script.ResetPosition(startPos, worldSize);
        p2Script.ResetPosition(startPos, worldSize);
        mainCam.transform.position = new Vector3(4, 1.5f, worldSize.x + worldSize.width);
    }

    //Coroutine for the closing of the text and playing the intro animation at the start of each round. This also enable the controls and the fight begins.
    public IEnumerator RoundStartCoroutine()
    {
        while (true)
        {
            p1Script.TurnPlayer();
            p2Script.TurnPlayer();
            yield return new WaitForSeconds(timeBeforeRoundStart);
            roundStart = true;
            MainScript.instance.um.inGame.DisplayRoundText("FIGHT!");
            SFXManager.sfxInstance.audio.PlayOneShot(MainScript.instance.um.inGame.narratorVoices[1]);
            MainScript.instance.um.inGame.StartCoroutineRoundText(2f);
            EnableControls();
            ResetPositions();
            StopCoroutine(roundStartCoroutine);
        }
    }

    //The world border drawn on the scene to visualize where the world ends
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

    //Function to prevent fighters that jump and land on top of their opponent and gets stuck. Function repeated twice for each player
    public bool CheckCharacterOnTopOfEachOther()
    {
        Vector3 p1Position = p1Script.transform.position;
        Vector3 p2Position = p2Script.transform.position;
        bool p2OnTop = p1Position.z + 0.8f > p2Position.z && p1Position.z - 0.8f < p2Position.z;
        bool p2Falling = p2Script.rb.velocity.y < 0;
        if (p1Script.isGrounded && !p2Script.isGrounded && p2OnTop && !p2Falling)
        {
            if (p2Script.transform.rotation.y < 1)
            {
                p2Script.transform.Translate(new Vector3(0,0, p2Script.transform.forward.z * repulsionForce));
            }
            else
            {
                p2Script.transform.Translate(new Vector3(0,0, -p2Script.transform.forward.z * repulsionForce));
            }
            return true;
        }
        bool p1OnTop = p2Position.z + 0.8f > p1Position.z && p2Position.z - 0.8f < p1Position.z;
        bool p1Falling = p1Script.rb.velocity.y < 0;
        if (p2Script.isGrounded && !p1Script.isGrounded && p1OnTop && !p1Falling)
        {
            if (p1Script.transform.rotation.y < 1)
            {
                p1Script.transform.Translate(new Vector3(0,0, p1Script.transform.forward.z * repulsionForce));
            }
            else
            {
                p1Script.transform.Translate(new Vector3(0,0, -p1Script.transform.forward.z * repulsionForce));
            }
            return true;
        }
        return false;
    }

    
    
    // Update function
    public void UpdateObjects()
    {
        CameraMovement();
        if (roundStart)
        {
            if (MainScript.instance.player1.Combat1.Move.ReadValue<Vector2>() == Vector2.zero)
                p1Script.inputVector = Vector2.zero;
            var gamepads = Gamepad.all;
            var arrayDevice = MainScript.instance.player1.Combat1.Move.controls;
            idP1 = arrayDevice[0].device.deviceId;
            
            if (MainScript.instance.portController.keyboardOnly1)
                idP1 = -1;
            p1Script.Movement( idP1, false);
            if (twoPlayer || MainScript.instance.state == GameState.TrainingCombat)
            {
                if (MainScript.instance.player2.Combat2.Move.ReadValue<Vector2>() == Vector2.zero)
                    p2Script.inputVector = Vector2.zero;
                var arrayDevice2 = MainScript.instance.player2.Combat2.Move.controls;
                idP2 = arrayDevice2[1].device.deviceId;
                if (MainScript.instance.portController.keyboardOnly2)
                    idP2 = -1;
                p2Script.Movement(idP2, false);
            }
            p1Script.AnimationCalculation();
            p2Script.AnimationCalculation();
            if (MainScript.instance.state == GameState.Combat && !twoPlayer)
                stateMachine.UpdateStates();
            if (CheckCharacterOnTopOfEachOther())
            {
                Debug.Log("OnTop");
            }
        }
    }

    //Pause performed event that trigger the pause menu
    public void PauseButton(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
                return;
        }
        
        if (MainScript.instance.GetPause())
        {
            MainScript.instance.SetPause(false);
        }
        else
        {
            MainScript.instance.SetPause(true);
            SFXManager.sfxInstance.PlayPauseSound();
        }
    }
    //Camera will move when a player is attempting to leave the edge of the screen if theres still place in the world to move
    public void CameraMovement()
    {
        CameraFollow(player1.transform.position);
        CameraFollow(player2.transform.position);
        CameraCenter();
        Vector3 camPosition = mainCam.transform.position;
        mainCam.transform.position = new Vector3(camPosition.x,
            camPosition.y,
            Mathf.Clamp(camPosition.z, worldSize.x, worldSize.width));
    }

    //Camera will never moves too far of the player thanks to this function
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

    //Camera will always try to center itself in between each fighter
    public void CameraCenter()
    {
        Vector3 camPosition = mainCam.transform.position;
        float middlePoint = (player1.transform.position.z + player2.transform.position.z) / 2;
        mainCam.transform.position = Vector3.Lerp(camPosition,
             new Vector3(camPosition.x, camPosition.y, middlePoint), cameraSpeed * Time.deltaTime);
        
    }

    //Attack performed event that triggers an attack
    public void UseMove(int playerPort, MoveType moveType, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
                return;
        }
        //Input buffer array that is unoperational. Their is currently button mapped to triggers special input
        
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

    //Movement event performed that gives the value to the internal vector inside the fighterScript
    public void PlayerMove(int playerPort, InputAction.CallbackContext context)
    {
       // Debug.Log(context.control.device.name);
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
            
            idP1 = context.control.device.deviceId;
            p1Script.inputVector = context.ReadValue<Vector2>();
            
            return;
        }

        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
                return;
            idP2 = context.control.device.deviceId;
            p2Script.inputVector = context.ReadValue<Vector2>();
            return;
        }
    }
    
    //Dash event performed that triggers a dash
    public void PerformDash(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
        }
        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
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

    //Function called when a match is over. More logic was initially inside this function but as the need decreased over the time, the function became empty/
    public void MatchOver()
    {
        DisableControls();
    }

    //Remove fighters currently active and put them back in the pool
    public void DeleteCurrentFighter()
    {
       player1.SetActive(false);
       player2.SetActive(false);
        Destroy(stage);
    }

    //Method to check which player wins
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

        MainScript.instance.um.inGame.StartDelayedCoroutineRoundText(2f, 2f, winningPort,
            MainScript.instance.um.inGame.narratorVoices[4]);
        roundStart = false;
        EventManager.InvokeRoundEnd(new RoundEndEventArg(winningPort));
    }

    //Method to manually set the hp of a player. Used in the training Mode Script
    public void ForceHp(int hp1, int hp2)
    {
        p1Script.currentHp = hp1 * p1Script.stats.maxHp / 100;
        p2Script.currentHp = hp2 * p2Script.stats.maxHp / 100;
    }

    //Return distance between PLayer
    public float CheckDistanceBetweenPlayer()
    {
        return Vector3.Distance(p1Script.transform.position, p2Script.transform.position);
    }
}