using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox: MonoBehaviour {
    public LayerMask mask;
    public bool useSphere = false;
    
    public float radius = 0.5f;
    public Color inactiveColor;
    public Color collisionOpenColor;
    public Color collidingColor;

    private ColliderState _state;
    
    private IHitboxResponder _responder = null;
    
    public Vector3 currentHitboxSize;

    public Vector3 currentPosition;

    private void Start()
    {
        transform.position = currentPosition;
    }

    private void CheckGizmoColor() {
        switch(_state) {
            case ColliderState.Closed:
                Gizmos.color = inactiveColor;
                break;
            case ColliderState.Open:
                Gizmos.color = collisionOpenColor;
                break;
            case ColliderState.Colliding:
                Gizmos.color = collidingColor;
                break;
        }
    }
    
    private void OnDrawGizmos()
    {
        //transform.Translate(currentPosition);
        transform.localPosition = currentPosition;
        if (_state == ColliderState.Closed) { return; }
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.rotation, transform.localScale);
            Gizmos.DrawCube(transform.position, new Vector3(currentHitboxSize.x * 2, currentHitboxSize.y * 2, currentHitboxSize.z * 2)); // Because size is halfExtents
    }
    
    public void StartCheckingCollision(Vector3 hitboxToUse, Vector3 positionToUse) {
        _state = ColliderState.Open; 
        transform.gameObject.SetActive(true);
        currentHitboxSize = hitboxToUse;
        currentPosition = positionToUse;
    }

    public void StopCheckingCollision() {
        _state = ColliderState.Closed; 
        transform.gameObject.SetActive(false);
        currentHitboxSize = new Vector3(0,0,0);
        currentPosition = new Vector3(0,0,0);
    }
    
    private void Update() {
        
        if (_state == ColliderState.Closed || !transform.gameObject.activeInHierarchy) { return; }
        Collider[] colliders = Physics.OverlapBox(transform.position, currentHitboxSize, transform.rotation, mask);
        
        for (int i = 0; i < colliders.Length; i++) {
            Collider aCollider = colliders[i];
            _responder?.CollisionedWith(aCollider);
        }

        _state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Open;

    }
    
    public void UseResponder(IHitboxResponder responder) {
        _responder = responder;
    }
}
