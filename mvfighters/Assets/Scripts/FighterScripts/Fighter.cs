using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter", menuName = "Fighter / Add")]
public class Fighter : ScriptableObject
{
    public float walkSpeed;

    public float jumpforce;
    
    public int maxHp;

    public string name;
    
    public List<Move> moveSet;

    public float jumpSquat;

    public float jumpFrames;

    public float floatiness;
}
