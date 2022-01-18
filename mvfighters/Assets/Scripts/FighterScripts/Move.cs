using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Move / Add")]
public class Move : ScriptableObject 
{
    public List<MoveProperty> properties;
    public MoveType type;
    public MoveCategory category;
    
    public int damage;

    public float hitstun;

    public float startupFrame;

    public float activeFrame;

    public float endingFrame;

    public Vector3 hitboxSize;

    public Vector3 hitboxPosition;

    public Vector3 knockbackForce;

    public int MovePriority;

    public AudioClip attackLaunchedSfx;

    public AudioClip attackLandSfx;
    
    public Move(int damage, float hitstun, float startupFrame, float activeFrame, float endingFrame,
        Vector3 knockbackForce, Vector3 hitboxPosition,Vector3 hitboxSize,MoveType type, MoveCategory category)
    {
        this.damage = damage;
        this.hitstun = hitstun;
        this.startupFrame = startupFrame;
        this.activeFrame = activeFrame;
        this.endingFrame = endingFrame;
        
        this.type = type;
        this.hitboxSize = hitboxSize;
        this.hitboxPosition = hitboxPosition;
        this.knockbackForce = knockbackForce;
        this.category = category;
        this.properties = new List<MoveProperty>();
    }

    public void AddProperties(MoveProperty property)
    {
        properties.Add(property);
    }

    public float GetTotalFrames()
    {
        return startupFrame + activeFrame + endingFrame;
    }
}