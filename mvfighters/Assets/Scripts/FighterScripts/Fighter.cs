using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Fighter : ScriptableObject
{
    public float walkSpeed;

    public float jumpforce;
    
    public int maxHp;

    public Fighter(float walkSpeed, float jumpforce, int maxHp)
    {
        this.walkSpeed = walkSpeed;
        this.jumpforce = jumpforce;
        this.maxHp = maxHp;
    }
}
