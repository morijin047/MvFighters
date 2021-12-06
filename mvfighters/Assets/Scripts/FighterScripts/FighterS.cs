using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class FighterS : MonoBehaviour
{
    public Fighter stats;

    public List<Move> moveSet;

    private CharacterState state;

    private Rigidbody rb;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animation>();
        state = CharacterState.Idle;
        animator = GetComponent<Animator>();

        EventManager.AddDamageListener(TakeHit);
        EventManager.AddDamageListener(MoveLand);

        stats = new Fighter(2f, 300f, 10000);
        currentHp = stats.maxHp;
        
        // Move aMove = new Move(100, 15f, 1.5f, 1.5f, 3f, new Vector3(0, 0, 50f),
        //     new Vector3(0, 0.5f, 0.5f), new Vector3(0.1f, 0.3f, 0.7f), MoveHierarchy.A, MoveCategory.Melee);
        // Move bMove = new Move(250, 6f, 2.5f, 2f, 4f, new Vector3(0, 0, 100f),
        //     new Vector3(0, 0.1f, 0.5f), new Vector3(0.1f, 0.5f, 0.8f), MoveHierarchy.B, MoveCategory.Melee);
        // Move cMove = new Move(400, 8f, 3.5f, 2f, 6f, new Vector3(0, 0, 150f),
        //     new Vector3(0, 0.5f, 0.5f), new Vector3(0.1f, 0.5f, 1f), MoveHierarchy.C, MoveCategory.Melee);
        // aMove.AddProperties(MoveProperty.JumpCancel);
        // bMove.AddProperties(MoveProperty.JumpCancel);
        // moveSet.Add(aMove);
        // moveSet.Add(bMove);
        // moveSet.Add(cMove);
        // Object[] obj = AssetDatabase.LoadAllAssetsAtPath(Path.Combine("Assets", "Scripts", "FighterScripts", "Moves", "Dummy")) ;
        // foreach (Object move in obj)
        // {
        //     if (move.GetType() == typeof(Move))
        //         moveSet.Add(move as Move);
        // }
        moveLand = true;
    }

    public void SetPort(int playerPort)
    {
        this.playerPort = playerPort;
    }

    public int GetPort()
    {
        return playerPort;
    }
    

    void MoveLand(DamageEventArg arg)
    {
        moveLand = true;
        canAttack = true;
        DeActivateHitbox();
    }

    void ChangeAnimationState(CharacterState newAnimation)
    {
        if (state == newAnimation && (newAnimation == CharacterState.Idle || newAnimation == CharacterState.Walking || newAnimation == CharacterState.Running)) return;
        // if(playerPort == 2)
        //     Debug.Log(newAnimation.ToString());
        
        animator.Play(newAnimation.ToString(), -1, 0);
        state = newAnimation;
    }

    void ChangeAnimationState(MoveHierarchy newAnimation)
    {
        CharacterState temp;
        if (CharacterState.TryParse(newAnimation.ToString(), out temp))
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, layerMask))
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
        
        if(Input.GetKey(KeyCode.M))
            rb.AddForce(new Vector3(0,0, 250f));

        CheckAirborne();
        //Debug.Log(isGrounded);
        //Vector2 inputVector = context.ReadValue<Vector2>();
        if (inputVector.y > 0 && isGrounded)
        {
            Jump();
        }

        //Debug.Log(inputVector);
        if (isGrounded && !animPlaying)
        {
            if (inputVector.x != 0)
            {
                //rb.velocity = new Vector3(0, 0, inputVector.x * Time.deltaTime * 50000);
                if (transform.rotation.y != 0)
                    transform.Translate(new Vector3(0, 0, -inputVector.x * Time.deltaTime * stats.walkSpeed));
                else
                    transform.Translate(new Vector3(0, 0, inputVector.x * Time.deltaTime * stats.walkSpeed));
                // anim.clip = walking;
                if (!animPlaying)
                    ChangeAnimationState(CharacterState.Walking);
            }
            else
            {
                //anim.clip = idle;
                if (!animPlaying)
                    ChangeAnimationState(CharacterState.Idle);
            }
        }

        if (currentHitstunFrame > 0)
        {
            hitstunRemaining = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;

            if (hitstunRemaining >= currentHitstunFrame)
            {
                currentHitstunFrame = -1;
                AnimOver();
                isInHitstun = false;
                canbeHit = true;
            }
        }


        if (currentFrame >= 0)
        {
            currentFrame = animator.GetCurrentAnimatorStateInfo(default).normalizedTime * 10;
            canAttack = false;
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
            else if (currentFrame > currentMove.activeFrame + currentMove.startupFrame )
            {
                if (hitBoxActivatated)
                    DeActivateHitbox();
            }

            else if (currentFrame > currentMove.startupFrame)
            {
                if(!hitBoxActivatated)
                 ActivateHitbox();
            }
            else
            {
                if (hitBoxActivatated)
                    DeActivateHitbox();
            }
        }


        //if (!animPlaying)
        //anim.Play();

        // if (currentFrame >= 0)
        // {
        //     Debug.Log(currentFrame);
        // }

        // if (currentHitstunFrame >= 0)
        // {
        //     Debug.Log(hitstunRemaining);
        // }
    }

    public void Attack(MoveHierarchy moveType)
    {
        // anim.clip = punch;
        //anim.Play();
        if (currentMove != null)
        {
            if (moveType > currentMove.hierarchy)
                canAttack = true;
        }

        if (!isInHitstun && canAttack && canCancel && moveLand)
        {
            foreach (var m in moveSet)
            {
                if (m.hierarchy == moveType)
                    currentMove = m;
            }

            // if (lastMove == null)
            //     lastMove = currentMove;
            // if (lastMove != currentMove)
            //     lastMove = currentMove;
            hitbox.UseResponder(currentMove);
            ChangeAnimationState(moveType);
            animPlaying = true;
            //head.GetComponent<HurtBox>().HurtboxSwitch(true);
            currentFrame = 0;
            canCancel = false;
            moveLand = false;
        }
    }

    public void Jump()
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
            //Vector3 temp = new Vector3(0, stats.jumpforce, inputVector.normalized.x * stats.walkSpeed * 100);
            //Debug.Log(temp);
            
            // rb.velocity = new Vector3(0, stats.jumpforce * Time.deltaTime,
            //     inputVector.x * Time.deltaTime * stats.walkSpeed * 1000);
            rb.AddForce(new Vector3(0, stats.jumpforce, inputVector.normalized.x * stats.walkSpeed * 100));
            transform.Translate(new Vector3(0, 0.2f, 0));
            isGrounded = false;
            //transform.Translate(new Vector3(0, stats.jumpforce * Time.deltaTime, inputVector.x * Time.deltaTime * stats.walkSpeed));
            ChangeAnimationState(CharacterState.Jumping);
        }
    }

    public void AnimOver()
    {
        animPlaying = false;
        // if (head.activeInHierarchy)
        //     head.GetComponent<HurtBox>().HurtboxSwitch(false);
        // if(playerPort == 2)
        //     Debug.Log("Anim Over");
        //ChangeAnimationState(CharacterState.Idle);
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
            if (lastMoveHitBy.hierarchy != eventArg.move.hierarchy)
                canbeHit = true;
        }
        if (eventArg.player == playerPort && canbeHit)
        {
            AnimOver();
            DeActivateHitbox();
            PushedAway(eventArg.move.knockbackForce);
            animPlaying = true;
            isInHitstun = true;
            canbeHit = false;
            lastMoveHitBy = eventArg.move;
            if (currentMove != null)
                currentFrame = currentMove.GetTotalFrames();
            
            if (transform.rotation.y != 0)
            {
                if (inputVector.x > 0 && !isInHitstun)
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
                if (inputVector.x < 0  && !isInHitstun)
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
        }
    }
}