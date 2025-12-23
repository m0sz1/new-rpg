using UnityEngine;

public class StateMachine
{ 
    public EntityState currentState { get; private set; } //¼ÇÂ¼µ±Ç°×´Ì¬


    public void Initialize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }
}
