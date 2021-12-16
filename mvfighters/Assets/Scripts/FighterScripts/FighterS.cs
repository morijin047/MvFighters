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
    public Fighter stats;

    private CharacterState state;

    public Rigidbody rb;

    private Animator animator;

    private bool animPlaying = false;

    private bool isGrounded = true;

    private Vector2 inputVector;

    public HitBox hitbox;

    private int playerPort;

    float currentHitstunFrame = -1;

    private float hitstunRemaining;

    private float currentFrame = -1;

    public int currentHp;

    private Move currentMove;

    private Move lastMoveHitBy;

    private bool isInHitstun = false;

    private bool canAttack = true;

    private bool canCancel = true;

    private bool moveLand = true;

    private bool hitBoxActivatated = false;

    private bool canbeHit = true;

    private bool isCrouched = false;

    private bool jumpSquat = false;

    private float currentJumpFrame = -1;

    private bool jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        state = CharacterState.Idle;
        animator = GetComponent<Animator>();
        EventManager.AddDamageListener(TakeHit);
        EventManager.AddDamageListener(MoveLand);
        currentHp = stats.maxHp;
        moveLand = true;
        ResetPosition();
    }

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

    public void ResetPosition()
    {
        if (playerPort == 1)
            transform.position = new Vector3(0, 0, -3);
        if (playerPort == 2)
            transform.position = new Vector3(0, 0, 3);
    }
    
    void MoveLand(DamageEventArg arg)
    {
        moveLand = true;
        canAttack = true;
        DeActivateHitbox();
    }

    void ChangeAnimationState(CharacterState newAnimation)
    {
        if (state == newAnimation && (newAnimation == CharacterState.Idle || newAnimation == CharacterState.Walking ||
                                      newAnimation == CharacterState.Running)) return;
        animator.Play(newAnimation.ToString(), -1, 0);
        state = newAnimation;
    }

    void ChangeAnimationState(MoveType newAnimation)
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

    void CheckAirborne()
    {
        RaycastHit hit;
        int layerMask = 1 << 6;
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 0.1f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, layerMask) && !jumpSquat)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Movement(Vector2 input)
    {
        inputVector = input;

        if (Input.GetKey(KeyCode.M))
            rb.AddForce(new Vector3(0, 0, 250f));

        CheckAirborne();
        if (inputVector.y > 0 && isGrounded)
        {
            StartJumpSquat();
        }

        if (inputVector.y < 0 && isGrounded)
        {
            isCrouched = true;
        }
        else
        {
            isCrouched = false;
        }

        if (isGrounded && !animPlaying)
        {
            if (inputVector.x != 0 && !isCrouched)
            {
                transform.Translate(new Vector3(0, 0, transform.forward.z * inputVector.x * Time.deltaTime * stats.walkSpeed));
                if (!animPlaying)
                    ChangeAnimationState(CharacterState.Walking);
            }
            else
            {
                if (!animPlaying)
                {
                    if (isCrouched)
                        ChangeAnimationState(CharacterState.Crouching);
                    else
                    {
                        ChangeAnimationState(CharacterState.Idle);
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
                        
                }
            }
        }

        // if (currentHitstunFrame == 0)
        // {
        //     animator.Rebind();
        //     currentHitstunFrame++;
        // }
        if (currentHitstunFrame > 0)
        {
            hitstunRemaining = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;
            //hitstunRemaining += 0.5f;
            if (hitstunRemaining >= currentHitstunFrame)
            {
                currentHitstunFrame = -1;
                AnimOver();
                isInHitstun = false;
                canbeHit = true;
            }
        }

        // if (currentFrame == 0)
        // {
        //     animator.Rebind();
        //     animator.Update(0f);
        //     currentFrame++;
        // }
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

    public void Attack(MoveType moveType)
    {
        if (currentMove != null)
        {
            if (CheckCancel(moveType))
                canAttack = true;
        }

        if (!isInHitstun && canAttack && canCancel && moveLand)
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

    public bool CheckCrouch()
    {
        return isCrouched;
    }

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

        if (canAttack && canCancel && moveLand)
        {
            jumpSquat = true;
            currentJumpFrame = 0;
            animPlaying = true;
            ChangeAnimationState(CharacterState.Jumping);
            //isGrounded = false;
        }
    }

    public void Jumping()
    {
        rb.AddForce(new Vector3(0, stats.jumpforce, inputVector.normalized.x * stats.jumpforce / 3));
        transform.Translate(new Vector3(0, 0.2f, 0));
        isGrounded = false;
    }

    public void AnimOver()
    {
        animPlaying = false;
    }

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

    public void PushedAway(Vector3 knockback)
    {
        rb.AddForce(new Vector3(0, knockback.y, knockback.z * -transform.forward.z));
    }

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
                if (inputVector.x > 0 && !isInHitstun && CheckOverHead(eventArg) && CheckLow(eventArg))
                {
                    currentHitstunFrame = eventArg.move.hitstun;
                    currentHitstunFrame /= 2;
                    ChangeAnimationState(CharacterState.Blocking);
                }

                else
                {
                    ChangeAnimationState(CharacterState.GettingHit);
                    currentHp -= eventArg.move.damage;
                    currentHitstunFrame = eventArg.move.hitstun;
                }
            }
            else
            {
                if (inputVector.x < 0 && !isInHitstun)
                {
                    currentHitstunFrame = eventArg.move.hitstun;
                    currentHitstunFrame /= 2;
                    ChangeAnimationState(CharacterState.Blocking);
                }

                else
                {
                    ChangeAnimationState(CharacterState.GettingHit);
                    currentHp -= eventArg.move.damage;
                    currentHitstunFrame = eventArg.move.hitstun;
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
                switch (playerPort)
                {
                    case 1:
                        EventManager.InvokeRoundEnd(new RoundEndEventArg(2));
                        break;
                    case 2:
                        EventManager.InvokeRoundEnd(new RoundEndEventArg(1));
                        break;
                }
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
}