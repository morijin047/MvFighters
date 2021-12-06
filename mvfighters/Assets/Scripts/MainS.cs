using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MainS : MonoBehaviour
{
    public FighterM fm;

    public UIManager um;

    public InputManager player1;
    
    public InputManager player2;

    public static MainS instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        player1 = new InputManager();
        player2 = new InputManager();
        PrepareInputManager(player1, 1);

    }

    public void PrepareInputManager(InputManager player, int playerPort)
    {
        player1.Combat1.Enable();
        player1.Combat1.A.performed += context => fm.UseMove(1, MoveHierarchy.A);
        player1.Combat1.B.performed += context => fm.UseMove(1, MoveHierarchy.B);
        player1.Combat1.C.performed += context => fm.UseMove(1, MoveHierarchy.C);
        player2.Combat2.Enable();
        player2.Combat2.A.performed += context => fm.UseMove(2, MoveHierarchy.A);
        player2.Combat2.B.performed += context => fm.UseMove(2, MoveHierarchy.B);
        player2.Combat2.C.performed += context => fm.UseMove(2, MoveHierarchy.C);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fm.UpdateObjects();
    }
}
