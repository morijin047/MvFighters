using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class HurtBox : MonoBehaviour
{
    public Collider collider;
    private ColliderState _state = ColliderState.Open;
    public Vector3 hurtboxSize = Vector3.one;

    public void GetHitBy(Move move, int playerThatUsedAttack)
    {
        //Debug.Log("Damage");
        int playerHit = GetComponentInParent<FighterS>().GetPort();
        //if(playerHit != playerThatUsedAttack)
            EventManager.InvokeDamage(new DamageEventArg(playerHit, move));
        //return true;
    }

    public void HurtboxSwitch(bool enabled)
    {
        transform.gameObject.SetActive(enabled);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(collider.transform.position, collider.transform.rotation,
            collider.transform.localScale);
        Gizmos.DrawCube(Vector3.zero, 
            new Vector3(hurtboxSize.x * 2, hurtboxSize.y * 2, hurtboxSize.z * 2)); // Because size is halfExtents
    }
}