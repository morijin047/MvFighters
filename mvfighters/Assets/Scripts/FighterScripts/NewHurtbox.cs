using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewHurtbox : MonoBehaviour
{
    public BoxCollider collider;

    public Vector3 hurtboxSize;

    private void Start()
    {
        collider.size = hurtboxSize * 2;
    }

    public void GetHitBy(Move move, int playerThatUsedAttack)
    {
        int playerHit = 0;
        switch (playerThatUsedAttack)
        {
            case 1:
                playerHit = 2;
                break;
            case 2:
                playerHit = 1;
                break;
        }
        EventManager.InvokeDamage(new DamageEventArg(playerHit, move));
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var colliderTransform = collider.transform;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, 
            new Vector3(hurtboxSize.x * 2, hurtboxSize.y * 2, hurtboxSize.z * 2)); // Because size is halfExtents
    }

    private void Update()
    {
        
    }
}
