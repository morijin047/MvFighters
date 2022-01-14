using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHitbox : MonoBehaviour
{
    private LayerMask mask;
    public Color collisionOpenColor;
    private Vector3 currentHitboxSize;
    private Vector3 currentPosition;
    private Move currentMove;
    private int playerPort;
    public BoxCollider hitboxCollider;

    public void ActivateHitbox(Move currentMove, int port)
    {
        gameObject.SetActive(true);
        this.currentMove = currentMove;
        currentPosition = this.currentMove.hitboxPosition;
        currentHitboxSize = this.currentMove.hitboxSize;
        playerPort = port;
        switch (playerPort)
        {
            case 1:
                mask = LayerMask.NameToLayer("Player2");
                gameObject.layer = LayerMask.NameToLayer("Player1");
                break;
            case 2:
                mask = LayerMask.NameToLayer("Player1");
                gameObject.layer = LayerMask.NameToLayer("Player2");
                break;
        }

        hitboxCollider.size = currentHitboxSize * 2;
        hitboxCollider.center = currentPosition * 2;
    }

    private void OnDrawGizmos()
    {
        //transform.Translate(currentPosition);
        if (true)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(hitboxCollider.center, hitboxCollider.size); // Because size is halfExtents
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(hitboxCollider.center, hitboxCollider.size);
        }
    }

    private void Update()
    {
        // if (!gameObject.activeInHierarchy)
        // {
        //     return;
        // }

        // Collider[] colliders = Physics.OverlapBox(hitboxCollider.center, hitboxCollider.size,
        //     hitboxCollider.transform.rotation, mask);
        // foreach (var c in colliders)
        // {
        //     if (c.transform.root == transform)
        //         continue;
        //     c.GetComponent<NewHurtbox>().GetHitBy(currentMove, playerPort);
        //     gameObject.GetComponentInParent<FighterS>().AttackOver();
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer)
            return;
        if (other.gameObject.layer == mask)
        {
            if (other.GetComponent<NewHurtbox>() != null)
            {
                other.GetComponent<NewHurtbox>().GetHitBy(currentMove, playerPort);
                gameObject.GetComponentInParent<FighterS>().AttackLand();
            }
        }
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }
}