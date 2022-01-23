using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State currentState;

    public FighterS scriptToUse;

    public bool aiActive;

    public void AssignAIPrefabScript(FighterS script)
    {
        scriptToUse = script;
        aiActive = true;
    }
    
    public void UpdateStates()
    {
        if (aiActive)
            RunStateMachine();
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            SwitchToTheNextStage(nextState);
        }
    }

    private void SwitchToTheNextStage(State nextState)
    {
        currentState = nextState;
    }
}
