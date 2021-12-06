using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Move / Add")]
public class Move : ScriptableObject , IHitboxResponder
{
    public List<MoveProperty> properties;
    public MoveHierarchy hierarchy;
    public MoveCategory category;
    
    public int damage;

    public float hitstun;

    public float startupFrame;

    public float activeFrame;

    public float endingFrame;

    public Vector3 hitboxSize;

    public Vector3 hitboxPosition;

    public Vector3 knockbackForce;
    
    public Move(int damage, float hitstun, float startupFrame, float activeFrame, float endingFrame,
        Vector3 knockbackForce, Vector3 hitboxPosition,Vector3 hitboxSize,MoveHierarchy hierarchy, MoveCategory category)
    {
        this.damage = damage;
        this.hitstun = hitstun;
        this.startupFrame = startupFrame;
        this.activeFrame = activeFrame;
        this.endingFrame = endingFrame;
        
        this.hierarchy = hierarchy;
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

    public void CollisionedWith(Collider collider)
    {
        int playerPort = collider.GetComponentInParent<FighterS>().GetPort();
        HurtBox hurtbox = collider.GetComponent<HurtBox>();
        hurtbox?.GetHitBy(this, playerPort);
    }

    public float GetTotalFrames()
    {
        return startupFrame + activeFrame + endingFrame;
    }
}