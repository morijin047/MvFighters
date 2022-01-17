using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SubsystemsImplementation;
using Object = UnityEngine.Object;

public class FighterS : MonoBehaviour
{
    // SETUP
    public Rigidbody rb;
    private Animator animator;
    public Fighter stats;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        EventManager.AddDamageListener(TakeHit);
        EventManager.AddDamageListener(MoveLand);
        minZWorld = -5f;
        maxZWorld = 5f;
    }

    private int playerPort;

    public void SetPort(int playerPort)
    {
        this.playerPort = playerPort;
        gameObject.layer = LayerMask.NameToLayer("Player" + playerPort);
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = LayerMask.NameToLayer("Player" + playerPort);
        }
    }

    public int GetPort()
    {
        return playerPort;
    }

    public void ResetPosition(float startPos)
    {
        if (playerPort == 1)
            transform.position = new Vector3(0, 0, -startPos);
        if (playerPort == 2)
            transform.position = new Vector3(0, 0, startPos);
        ChangeAnimationState(CharacterState.Idle);
        moveLand = true;
        knockedDown = false;
        canAttack = true;
        HitStunOver();
        isCrouched = false;
        firsTap = false;
        secondTap = false;
        running = false;
        dashing = false;
        wavedashing = false;
        canDashAgain = true;
        teching = false;
        grabbing = false;
        if (hitbox.gameObject.activeInHierarchy)
            DeActivateHitbox();
        currentHp = stats.maxHp;
        lastTimePressedRun = Time.time;
        currentdashTime = stats.DashTime;
        TurnPlayer();
    }

    //ANIMATION
    private bool animPlaying = false;
    private CharacterState state;

    public void ChangeAnimationState(CharacterState newAnimation)
    {
        if (state == newAnimation && (newAnimation == CharacterState.Idle || newAnimation == CharacterState.Walking ||
                                      newAnimation == CharacterState.Running)) return;
        animator.Play(newAnimation.ToString(), -1, 0);
        state = newAnimation;
    }

    public void ChangeAnimationState(MoveType newAnimation)
    {
        CharacterState temp;
        string newString = newAnimation.ToString();
        // if (isCrouched)
        //     newString += "2";
        if (CharacterState.TryParse(newString, out temp))
        {
            ChangeAnimationState(temp);
        }
        else
        {
            Debug.Log("Error animation");
        }
    }

    public void AnimOver()
    {
        animPlaying = false;
    }

    //MOVEMENT
    private Vector2 inputVector;

    public void Movement(Vector2 input)
    {
        inputVector = input;

        CheckAirborne();

        //JUMP Performed
        if (inputVector.y > 0 && isGrounded || forcedJump && isGrounded)
        {
            StartJumpSquat();
        }

        CheckCrouch();

        //Winddow for double tapping to perform a run
        if (Time.time - lastTimePressedRun > runDelay && secondTap &&
            inputVector.normalized.x != directionRun.normalized.x)
        {
            firsTap = false;
            secondTap = false;
        }

        if (knockedDown && !teching)
        {
            if (Time.time - currentKnock >= knockWait && (inputVector.x != 0 || inputVector.y != 0))
            {
                TechRecovery();
            }
        }

        //Actual Movement Code
        if (isGrounded && !animPlaying && !knockedDown && !grabbing)
        {
            if (inputVector.x != 0 && !isCrouched)
            {
                //Pressed a direction once
                if (!firsTap && !secondTap)
                {
                    firsTap = true;
                    secondTap = false;
                    directionRun = inputVector;
                }

                //Pressed the same direction twice in a short delay
                if (Time.time - lastTimePressedRun < runDelay && secondTap)
                {
                    running = true;
                }

                //Running
                if (running)
                {
                    //Face Left
                    if (transform.rotation.y < 1)
                    {
                        if (transform.forward.z > 0)
                        {
                            if (inputVector.x >= 0)
                            {
                                PerformRun();
                            }
                            else
                            {
                                Dash();
                            }
                        }
                    }
                    //Facing Right
                    else
                    {
                        if (transform.forward.z < 0)
                        {
                            if (inputVector.x <= 0)
                            {
                                PerformRun();
                            }
                            else
                            {
                                Dash();
                            }
                        }
                    }
                }
                //Walking
                else
                {
                    transform.Translate(new Vector3(0, 0,
                        transform.forward.z * inputVector.x * Time.deltaTime * stats.walkSpeed));
                    if (!animPlaying)
                        ChangeAnimationState(CharacterState.Walking);
                }
            }
            //Idle Code
            else
            {
                //Check short pause in order to tap again for a run
                if (firsTap && !secondTap)
                {
                    secondTap = true;
                    lastTimePressedRun = Time.time;
                }

                //Stop run and reset the double tap logic
                if (running)
                {
                    running = false;
                    firsTap = false;
                    secondTap = false;
                }

                if (!animPlaying)
                {
                    if (isCrouched)
                    {
                        ChangeAnimationState(CharacterState.Crouching);
                    }

                    else
                    {
                        ChangeAnimationState(CharacterState.Idle);
                        canCancel = true;
                        TurnPlayer();
                    }
                }
            }
        }

        //Frame Calc
        CurrentHitStunFrame();
        CurrentAttackFrame();
        CurrentJumpFrame();

        //CameraLimit
        CameraLimit();

        //Dash Performed
        if (dashing)
        {
            Dashing();
        }

        if (wavedashing)
            WaveDashing();
    }

    private bool isGrounded = true;

    void CheckAirborne()
    {
        RaycastHit hit;
        int layerMask = 1 << 6;
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 0.1f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, layerMask) && !jumpSquat)
        {
            isGrounded = true;
            canDashAgain = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool isCrouched = false;

    public bool CheckCrouch()
    {
        if (inputVector.y < 0 && isGrounded)
        {
            isCrouched = true;
        }
        else
        {
            isCrouched = false;
        }

        return isCrouched;
    }

    private float runDelay = 0.1f;
    private float lastTimePressedRun;
    private bool firsTap;
    private bool secondTap;
    private bool running;
    private Vector2 directionRun;

    public void PerformRun()
    {
        transform.Translate(new Vector3(0, 0,
            transform.forward.z * inputVector.x * Time.deltaTime * stats.runspeed));
        if (!animPlaying)
            ChangeAnimationState(CharacterState.Running);
    }

    private float minZWorld;
    private float maxZWorld;

    public void CameraLimit()
    {
        float maxDist = MainS.instance.fm.maxDistance;
        Vector3 centerScreen = MainS.instance.fm.mainCam.transform.position;
        Vector3 viewPos = transform.position;
        viewPos.z = Math.Clamp(viewPos.z, centerScreen.z - maxDist / 2, centerScreen.z + maxDist / 2);
        transform.position = viewPos;
    }

    public void TurnPlayer()
    {
        if (playerPort == 1)
        {
            if (transform.position.z - MainS.instance.fm.player2.transform.position.z < 0)
            {
                transform.rotation = new Quaternion(0f, 0, 0f, 0f);
            }

            if (transform.position.z - MainS.instance.fm.player2.transform.position.z >= 0)
            {
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            }
        }

        if (playerPort == 2)
        {
            if (transform.position.z - MainS.instance.fm.player1.transform.position.z < 0)
            {
                transform.rotation = new Quaternion(0f, 0, 0f, 0f);
            }

            if (transform.position.z - MainS.instance.fm.player1.transform.position.z >= 0)
            {
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            }
        }
    }

    //ATTACK
    private bool canAttack = true;
    public int currentHp;

    public void Attack(MoveType moveType)
    {
        if (currentMove != null)
        {
            if (CheckCancel(moveType))
                canCancel = true;
        }

        if (!isInHitstun && canAttack && canCancel && moveLand && !grabbing)
        {
            if (isCrouched && moveType is MoveType.A or MoveType.B or MoveType.C)
            {
                if (MoveType.TryParse(moveType.ToString() + "2", out moveType))
                {
                    //Debug.Log(moveType);
                }
            }

            foreach (var m in stats.moveSet)
            {
                if (m.type == moveType)
                {
                    currentMove = m;
                }
            }

            if (currentMove.properties.Contains(MoveProperty.AntiAir))
            {
                airInvulnerable = true;
            }

            if (currentMove.properties.Contains(MoveProperty.SuperArmor))
            {
                superArmor = true;
            }

            ChangeAnimationState(moveType);
            animPlaying = true;
            //head.GetComponent<HurtBox>().HurtboxSwitch(true);
            currentFrame = 0;
            currentJumpFrame = 255;
            canCancel = false;
            moveLand = false;
            canAttack = false;
        }
    }

    public float currentFrame = -1;

    public void CurrentAttackFrame()
    {
        if (currentFrame >= 0)
        {
            currentFrame++;
            //currentFrame += 0.5f;
            if (currentFrame > currentMove.GetTotalFrames())
            {
                AttackOver();
                AnimOver();
            }
            else if (currentFrame > currentMove.activeFrame + currentMove.startupFrame)
            {
                if (hitbox.gameObject.activeInHierarchy)
                    DeActivateHitbox();
            }

            else if (currentFrame > currentMove.startupFrame)
            {
                if (!hitbox.gameObject.activeInHierarchy)
                    ActivateHitbox(currentMove);
            }
            else
            {
                if (hitbox.gameObject.activeInHierarchy)
                    DeActivateHitbox();
            }
        }
    }

    public void AttackOver()
    {
        currentFrame = -1;
        canAttack = true;
        moveLand = true;
        DeActivateSpecialProperties();
        if (hitbox.gameObject.activeInHierarchy)
            DeActivateHitbox();
    }

    public void AttackLand()
    {
        //AttackOver();
        if (hitbox.gameObject.activeInHierarchy)
            DeActivateHitbox();
        moveLand = true;
        canAttack = true;
        currentFrame = currentMove.activeFrame + currentMove.startupFrame;
    }

    public void DeActivateSpecialProperties()
    {
        if (superArmor)
            superArmor = false;
        if (airInvulnerable)
            airInvulnerable = false;
    }

    private bool canCancel = true;
    public Move currentMove;

    public bool CheckCancel(MoveType typeOfMove)
    {
        int priority;
        bool returnValue = false;
        switch (typeOfMove)
        {
            case MoveType.A:
                if (currentMove.type == typeOfMove || currentMove.type == MoveType.A2)
                    returnValue = true;
                break;
            case MoveType.A2:
                if (currentMove.type == typeOfMove || currentMove.type == MoveType.A)
                    returnValue = true;
                break;
            case MoveType.B:
                if (currentMove.MovePriority < 2)
                    returnValue = true;
                break;
            case MoveType.B2:
                if (currentMove.MovePriority < 2)
                    returnValue = true;
                break;
            case MoveType.C:
                if (currentMove.MovePriority < 3)
                    returnValue = true;
                break;
            case MoveType.C2:
                if (currentMove.MovePriority < 3)
                    returnValue = true;
                break;
            case MoveType.MotionF:
                if (currentMove.MovePriority < 4)
                    returnValue = true;
                break;
            case MoveType.MotionB:
                if (currentMove.MovePriority < 4)
                    returnValue = true;
                break;
            case MoveType.Super:
                if (currentMove.MovePriority < 5)
                    returnValue = true;
                break;
            case MoveType.Grab:
                returnValue = false;
                break;
        }

        return returnValue;
    }

    private bool moveLand = true;

    public void MoveLand(DamageEventArg arg)
    {
        if (arg.player == playerPort)
        {
            return;
        }

        moveLand = true;
        canAttack = true;
        if (hitbox.gameObject.activeInHierarchy)
            DeActivateHitbox();
        DeActivateSpecialProperties();
    }

    //HITBOX
    public NewHitbox hitbox;

    public void ActivateHitbox(Move move)
    {
        hitbox.ActivateHitbox(move, playerPort);
    }

    public void DeActivateHitbox()
    {
        hitbox.DeactivateHitbox();
    }

    //TECH RECOVERYY
    public IEnumerator techCoroutine;
    private bool teching = false;

    public void TechRecovery()
    {
        ChangeAnimationState(CharacterState.Recovery);
        teching = true;
        techCoroutine = TechCoroutine();
        StartCoroutine(techCoroutine);
    }

    public IEnumerator TechCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stats.techSpeed);
            teching = false;
            HitStunOver();
            knockedDown = false;
            ChangeAnimationState(CharacterState.Idle);
            StopCoroutine(techCoroutine);
        }
    }

    //DASH
    private float currentdashTime;
    private bool dashing = false;
    private bool canDashAgain;
    private Vector2 dashDirection;
    private bool wavedashing = false;

    public void Dash()
    {
        if (!dashing && !wavedashing && canAttack && canDashAgain && !knockedDown && !grabbing)
        {
            Vector2 inputVector = new Vector2();
            switch (playerPort)
            {
                case 1:
                    inputVector = MainS.instance.player1.Combat1.Move.ReadValue<Vector2>();
                    break;
                case 2:
                    inputVector = MainS.instance.player2.Combat2.Move.ReadValue<Vector2>();
                    break;
            }

            canAttack = false;
            float slideForce = 0f;

            if (jumpSquat)
            {
                ChangeAnimationState(CharacterState.Crouching);
                slideForce = 0.66f;
                JumpOver();
                jumpSquat = false;
                wavedashing = true;
            }
            else
            {
                dashing = true;
                ChangeAnimationState(CharacterState.Dash);
                slideForce = 0.66f;
            }

            animPlaying = true;
            canDashAgain = false;
            if (transform.rotation.y < 1)
            {
                if (inputVector.x >= 0)
                    dashDirection = new Vector2(transform.forward.z * slideForce, inputVector.y);
                if (inputVector.x < 0)
                    dashDirection = new Vector2(-transform.forward.z * slideForce / 1.5f, inputVector.y);
            }
            else
            {
                if (inputVector.x > 0)
                    dashDirection = new Vector2(-transform.forward.z * slideForce / 1.5f, inputVector.y);
                if (inputVector.x <= 0)
                    dashDirection = new Vector2(transform.forward.z * slideForce, inputVector.y);
            }
        }
    }


    private IEnumerator endLagDashCoroutine;

    public void Dashing()
    {
        if (currentdashTime <= 0)
        {
            dashing = false;
            endLagDashCoroutine = EndLagDashCoroutine(0.3f);
            StartCoroutine(endLagDashCoroutine);
        }
        else
        {
            currentdashTime -= Time.deltaTime;
            rb.velocity = new Vector3(0, 0, stats.dashSpeed * dashDirection.x);
        }
    }

    public void WaveDashing()
    {
        if (currentdashTime <= 0)
        {
            wavedashing = false;
            DashOver();
        }
        else
        {
            currentdashTime -= Time.deltaTime;
            rb.velocity = new Vector3(0, 0, stats.dashSpeed * dashDirection.x);
        }
    }

    public IEnumerator EndLagDashCoroutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            DashOver();
            StopCoroutine(endLagDashCoroutine);
        }
    }

    public void DashOver()
    {
        rb.velocity = Vector3.zero;
        canAttack = true;
        dashDirection = new Vector2();
        currentdashTime = stats.DashTime;
        AnimOver();
    }

    //JUMP
    private bool jumpSquat = false;
    private CharacterState stateBeforeJump;
    private float currentJumpFrame = -1;
    private bool jumping = false;

    public void StartJumpSquat()
    {
        if (currentMove != null)
        {
            foreach (var p in currentMove.properties)
            {
                if (p == MoveProperty.JumpCancel)
                    canCancel = true;
            }
        }

        if (canAttack && canCancel && moveLand && !isInHitstun && !knockedDown && !grabbing)
        {
            jumpSquat = true;
            currentJumpFrame = 0;
            animPlaying = true;
            stateBeforeJump = CharacterState.Running == state ? CharacterState.Running : CharacterState.Walking;
            ChangeAnimationState(CharacterState.Jumping);
            //isGrounded = false;
        }
    }

    public void Jumping()
    {
        if (stateBeforeJump == CharacterState.Walking)
            rb.AddForce(new Vector3(0, stats.jumpforce, inputVector.normalized.x * stats.jumpforce / 3));
        else
            rb.AddForce(new Vector3(0, stats.jumpforce, inputVector.normalized.x * stats.jumpforce * 0.75f));
        transform.Translate(new Vector3(0, 0.2f, 0));
        isGrounded = false;
    }

    public void CurrentJumpFrame()
    {
        if (currentJumpFrame >= 0 && jumpSquat)
        {
            currentJumpFrame++;

            if (currentJumpFrame > stats.jumpSquat)
            {
                Jumping();
                currentJumpFrame = 0;
                jumping = true;
                canAttack = false;
                jumpSquat = false;
            }
        }

        if (currentJumpFrame >= 0 && jumping)
        {
            currentJumpFrame++;
            if (currentJumpFrame > stats.jumpFrames)
            {
                JumpOver();
                AnimOver();
            }
        }

        if (rb.velocity.y < 0)
        {
            rb.AddForce(new Vector3(0, -stats.floatiness, 0));
        }
    }

    public void JumpOver()
    {
        currentJumpFrame = -1;
        jumping = false;
        canAttack = true;
    }

    //GETTING HIT
    private bool isInHitstun = false;
    private bool canbeHit = true;
    private bool airInvulnerable = false;
    private bool superArmor = false;
    private bool guardingAll = false;
    private bool guardingLow = false;
    private bool guardingHigh = false;

    public void TakeHit(DamageEventArg eventArg)
    {
        if (eventArg.player != playerPort)
        {
            return;
        }

        if (canbeHit && !knockedDown)
        {
            if (transform.rotation.y < 1)
            {
                if (inputVector.x < 0 && !isInHitstun && CheckOverHead(eventArg) && CheckLow(eventArg) &&
                    CheckGrab(eventArg))
                {
                    currentHitstunFrame = eventArg.move.hitstun;
                    currentHitstunFrame /= 2;
                    ChangeAnimationState(CharacterState.Blocking);
                }

                else
                {
                    if (eventArg.move.properties.Contains(MoveProperty.KnockDown))
                    {
                        KnockDown();
                        ApplyHit(eventArg);
                    }
                    else if (eventArg.move.type == MoveType.Grab)
                    {
                        Grab(eventArg);
                    }
                    else
                    {
                        if (superArmor)
                        {
                            currentHp -= eventArg.move.damage / 2;
                        }
                        else if (!airInvulnerable)
                        {
                            ChangeAnimationState(CharacterState.GettingHit);
                            ApplyHit(eventArg);
                        }
                    }
                }
            }
            else
            {
                if (inputVector.x > 0 && !isInHitstun && CheckOverHead(eventArg) && CheckLow(eventArg) &&
                    CheckGrab(eventArg))
                {
                    currentHitstunFrame = eventArg.move.hitstun;
                    currentHitstunFrame /= 2;
                    ChangeAnimationState(CharacterState.Blocking);
                }

                else
                {
                    if (eventArg.move.properties.Contains(MoveProperty.KnockDown))
                    {
                        KnockDown();
                        ApplyHit(eventArg);
                    }
                    else if (eventArg.move.type == MoveType.Grab)
                    {
                        Grab(eventArg);
                    }
                    else
                    {
                        if (superArmor)
                        {
                            currentHp -= eventArg.move.damage / 2;
                        }
                        else if (!airInvulnerable)
                        {
                            ChangeAnimationState(CharacterState.GettingHit);
                            ApplyHit(eventArg);
                        }
                    }
                }
            }

            if (!superArmor && (!airInvulnerable))
            {
                if (currentMove != null)
                    currentFrame = currentMove.GetTotalFrames();
                isInHitstun = true;
                AnimOver();
                if (hitbox.gameObject.activeInHierarchy)
                    DeActivateHitbox();
                animPlaying = true;
                if (currentFrame > 0)
                {
                    AttackOver();
                }

                if (currentJumpFrame > 0)
                    JumpOver();
            }

            if (currentHp < 0)
            {
                if (MainS.instance.state == GameState.TrainingCombat)
                {
                    MainS.instance.fm.ResetPositions();
                }
                else
                {
                    MainS.instance.um.inGame.DisplayRoundText("K.O!");
                    MainS.instance.um.inGame.StartCoroutineRoundText(2f);
                    SFXManager.sfxInstance.audio.PlayOneShot(MainS.instance.um.inGame.narratorVoices[2]);
                    MainS.instance.fm.CheckWinner();
                }
            }
        }
    }

    public void ApplyHit(DamageEventArg eventArg)
    {
        PushedAway(eventArg.move.knockbackForce);
        currentHp -= eventArg.move.damage;
        currentHitstunFrame = eventArg.move.hitstun;
    }

    public bool CheckOverHead(DamageEventArg arg)
    {
        bool blockSuccess = true;
        foreach (var p in arg.move.properties)
        {
            if (p == MoveProperty.Overhead)
            {
                if (inputVector.y < 0)
                {
                    blockSuccess = false;
                }

                if (guardingHigh)
                    blockSuccess = true;
                if (guardingLow)
                    blockSuccess = false;
            }
        }

        if (guardingAll)
            blockSuccess = true;
        return blockSuccess;
    }

    public bool CheckLow(DamageEventArg arg)
    {
        bool blockSuccess = true;
        foreach (var p in arg.move.properties)
        {
            if (p == MoveProperty.Low)
            {
                if (inputVector.y >= 0)
                {
                    blockSuccess = false;
                }

                if (guardingLow)
                    blockSuccess = true;
            }
        }

        if (guardingAll)
            blockSuccess = true;
        return blockSuccess;
    }

    public bool CheckGrab(DamageEventArg arg)
    {
        bool blockSuccess = arg.move.type != MoveType.Grab;
        return blockSuccess;
    }

    public bool grabbing = false;

    public void Grab(DamageEventArg arg)
    {
        ChangeAnimationState(CharacterState.GettingHit);
        //MainS.instance.fm.DisableControls();
        if (playerPort == 1)
            MainS.instance.fm.p2Script.grabbing = true;
        if (playerPort == 2)
            MainS.instance.fm.p1Script.grabbing = true;
        currentHitstunFrame = 999f;
        delayedKnockDownCoroutine = DelayedKnockDown(2f, arg);
        StartCoroutine(delayedKnockDownCoroutine);
    }

    public void PushedAway(Vector3 knockback)
    {
        rb.AddForce(new Vector3(0, knockback.y, knockback.z * -transform.forward.z));
    }

    private bool knockedDown = false;
    private float knockWait = 2f;
    private float currentKnock;
    public IEnumerator delayedKnockDownCoroutine;

    public void KnockDown()
    {
        ChangeAnimationState(CharacterState.KnockDown);
        knockedDown = true;
        currentKnock = Time.time;
        canAttack = false;
    }

    public IEnumerator DelayedKnockDown(float delay, DamageEventArg arg)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            canbeHit = false;
            if (playerPort == 1)
            {
                MainS.instance.fm.p2Script.grabbing = false;
                MainS.instance.fm.p2Script.Attack(MoveType.Grab2);
            }

            if (playerPort == 2)
            {
                MainS.instance.fm.p1Script.grabbing = false;
                MainS.instance.fm.p1Script.Attack(MoveType.Grab2);
            }

            currentHp -= arg.move.damage;
            currentHitstunFrame = arg.move.hitstun;
            //MainS.instance.fm.EnableControls();
            StopCoroutine(delayedKnockDownCoroutine);
        }
    }

    float currentHitstunFrame = -1;

    public void CurrentHitStunFrame()
    {
        if (currentHitstunFrame > 0)
        {
            currentHitstunFrame--;
            //hitstunRemaining += 0.5f;
            if (currentHitstunFrame <= 1)
            {
                HitStunOver();
            }
        }
    }

    public void HitStunOver()
    {
        currentHitstunFrame = -1;
        AnimOver();
        isInHitstun = false;
        canbeHit = true;
        canAttack = true;
    }

    //Round Animation
    public void PlayRoundWinAnimation()
    {
        ChangeAnimationState(CharacterState.Winner);
    }

    public void PlayMatchIntroAnimation()
    {
        ChangeAnimationState(CharacterState.Intro);
    }

    private bool forcedJump = false;

    public void ForceAction(string action)
    {
        switch (action)
        {
            case "Idle":
                inputVector = new Vector2(0, 0);
                guardingLow = false;
                guardingHigh = false;
                guardingAll = false;
                forcedJump = false;
                break;
            case "GuardLow":
                if (transform.rotation.y < 1)
                {
                    inputVector = new Vector2(-1, 0);
                }
                else
                {
                    inputVector = new Vector2(1, 0);
                }

                guardingLow = true;
                guardingHigh = false;
                guardingAll = false;
                forcedJump = false;
                break;
            case "GuardHigh":
                if (transform.rotation.y < 1)
                {
                    inputVector = new Vector2(-1, 0);
                }
                else
                {
                    inputVector = new Vector2(1, 0);
                }

                guardingLow = false;
                guardingHigh = true;
                guardingAll = false;
                forcedJump = false;
                break;
            case "GuardAll":
                if (transform.rotation.y < 1)
                {
                    inputVector = new Vector2(-1, 0);
                }
                else
                {
                    inputVector = new Vector2(1, 0);
                }

                guardingLow = false;
                guardingHigh = false;
                guardingAll = true;
                forcedJump = false;
                break;
            case "Jump":
                inputVector = new Vector2(0, 1);
                guardingLow = false;
                guardingHigh = false;
                guardingAll = false;
                forcedJump = true;
                break;
            case "AI":
                //use dummy code
                break;
        }
    }
}