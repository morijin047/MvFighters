using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
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

        if (playerPort == 1)
            hitbox.mask = LayerMask.GetMask("Player2");

        if (this.playerPort == 2)
        {
            transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            hitbox.mask = LayerMask.GetMask("Player1");
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
        animPlaying = false;
        isCrouched = false;
        firsTap = false;
        secondTap = false;
        running = false;
        dashing = false;
        canDashAgain = true;
        isInHitstun = false;
        teching = false;
        grabbing = false;
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
        if (inputVector.y > 0 && isGrounded)
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
                        DeActivateHitbox();
                        ChangeAnimationState(CharacterState.Crouching);
                    }
                        
                    else
                    {
                        ChangeAnimationState(CharacterState.Idle);
                        DeActivateHitbox();
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
                canAttack = true;
        }

        if (!isInHitstun && canAttack && canCancel && moveLand && !grabbing)
        {
            if (isCrouched && moveType is MoveType.A or MoveType.B or MoveType.C)
            {
                if (MoveType.TryParse(moveType.ToString() + "2", out moveType))
                {
                    Debug.Log(moveType);
                }
            }

            foreach (var m in stats.moveSet)
            {
                if (m.type == moveType)
                {
                    currentMove = m;
                }
            }

            hitbox.UseResponder(currentMove);
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
    
    private float currentFrame = -1;
    public void CurrentAttackFrame()
    {
        if (currentFrame >= 0)
        {
            currentFrame = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;
            //currentFrame += 0.5f;
            if (currentFrame > currentMove.GetTotalFrames() - currentMove.endingFrame)
            {
                canCancel = true;
            }

            if (currentFrame > currentMove.GetTotalFrames())
            {
                AnimOver();
                currentFrame = -1;
                canAttack = true;
                moveLand = true;
            }
            else if (currentFrame > currentMove.activeFrame + currentMove.startupFrame)
            {
                if (hitBoxActivatated)
                    DeActivateHitbox();
            }

            else if (currentFrame > currentMove.startupFrame)
            {
                if (!hitBoxActivatated)
                    ActivateHitbox();
            }
            else
            {
                if (hitBoxActivatated)
                    DeActivateHitbox();
            }
        }
    }
    
    private bool canCancel = true;
    private Move currentMove;
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
    void MoveLand(DamageEventArg arg)
    {
        moveLand = true;
        canAttack = true;
        DeActivateHitbox();
    }
    
    //HITBOX
    public HitBox hitbox;
    private bool hitBoxActivatated = false;
    public void ActivateHitbox()
    {
        hitbox.StartCheckingCollision(currentMove.hitboxSize, currentMove.hitboxPosition);
        hitBoxActivatated = true;
    }
    public void DeActivateHitbox()
    {
        hitbox.StopCheckingCollision();
        hitBoxActivatated = false;
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
    private bool dashing;
    private bool canDashAgain;
    private Vector2 dashDirection;
    public void Dash()
    {
        if (!dashing && canAttack && canDashAgain && !knockedDown && !grabbing)
        {
            dashing = true;
            canAttack = false;
            canDashAgain = false;
            ChangeAnimationState(CharacterState.Dash);
            animPlaying = true;
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

            if (transform.rotation.y < 1)
            {
                if (inputVector.x >= 0)
                    dashDirection = new Vector2(transform.forward.z, inputVector.y);
                if (inputVector.x < 0)
                    dashDirection = new Vector2(-transform.forward.z * 0.75f, inputVector.y);
            }
            else
            {
                if (inputVector.x > 0)
                    dashDirection = new Vector2(-transform.forward.z, inputVector.y);
                if (inputVector.x <= 0)
                    dashDirection = new Vector2(transform.forward.z * 0.75f, inputVector.y);
            }
        }
    }
    public void Dashing()
    {
        if (currentdashTime <= 0)
        {
            dashing = false;
            canAttack = true;
            dashDirection = new Vector2();
            currentdashTime = stats.DashTime;
            rb.velocity = Vector3.zero;
            AnimOver();
        }
        else
        {
            currentdashTime -= Time.deltaTime;
            rb.velocity = new Vector3(0, 0, stats.dashSpeed * dashDirection.x);
        }
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
                    canAttack = true;
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
            currentJumpFrame = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;

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
            currentJumpFrame = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;
            if (currentJumpFrame > stats.jumpFrames)
            {
                currentJumpFrame = -1;
                jumping = false;
                canAttack = true;
                AnimOver();
            }
        }
        
        if (rb.velocity.y < 0)
        {
            rb.AddForce(new Vector3(0, -stats.floatiness, 0));
        }
    }
    
    //GETTING HIT
    private bool isInHitstun = false;
    private Move lastMoveHitBy;
    private bool canbeHit = true;
    public void TakeHit(DamageEventArg eventArg)
    {
        if (lastMoveHitBy != null)
        {
            if (lastMoveHitBy.type != eventArg.move.type)
                canbeHit = true;
        }

        if (eventArg.player == playerPort && canbeHit)
        {
            if (currentMove != null)
                currentFrame = currentMove.GetTotalFrames();

            if (transform.rotation.y != 0)
            {
                if (inputVector.x > 0 && !isInHitstun && CheckOverHead(eventArg) && CheckLow(eventArg) && CheckGrab(eventArg))
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
                        currentHp -= eventArg.move.damage;
                        currentHitstunFrame = eventArg.move.hitstun; 
                    } else if (eventArg.move.type == MoveType.Grab)
                    {
                        Grab(eventArg);
                    }
                    else
                    {
                        ChangeAnimationState(CharacterState.GettingHit);
                        currentHp -= eventArg.move.damage;
                        currentHitstunFrame = eventArg.move.hitstun; 
                    }
                }
            }
            else
            {
                if (inputVector.x < 0 && !isInHitstun && CheckOverHead(eventArg) && CheckLow(eventArg) && CheckGrab(eventArg))
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
                        currentHp -= eventArg.move.damage;
                        currentHitstunFrame = eventArg.move.hitstun; 
                    }
                    else if (eventArg.move.type == MoveType.Grab)
                    {
                        Grab(eventArg);
                    }
                    else
                    {
                        ChangeAnimationState(CharacterState.GettingHit);
                        currentHp -= eventArg.move.damage;
                        currentHitstunFrame = eventArg.move.hitstun; 
                    }
                }
            }

            isInHitstun = true;
            AnimOver();
            DeActivateHitbox();
            PushedAway(eventArg.move.knockbackForce);
            animPlaying = true;
            if (currentFrame > 0)
                currentFrame = 255;
            if (currentJumpFrame > 0)
                currentJumpFrame = 255;
            canbeHit = false;
            lastMoveHitBy = eventArg.move;
            if (currentHp < 0)
            {
                MainS.instance.um.inGame.DisplayRoundText("K.O!");
                MainS.instance.um.inGame.StartCoroutineRoundText(2f);
                SFXManager.sfxInstance.audio.PlayOneShot(MainS.instance.um.inGame.narratorVoices[2]);
                MainS.instance.fm.CheckWinner();
            }
        }
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
            }
        }

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
            }
        }

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
    private float hitstunRemaining;
    public void CurrentHitStunFrame()
    {
        if (currentHitstunFrame > 0)
        {
            hitstunRemaining = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;
            //hitstunRemaining += 0.5f;
            if (hitstunRemaining >= currentHitstunFrame)
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
}