using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : MonoBehaviour
{
    protected T RunnerObject;

    // called whenever we enter this state. Good for setting up variables
    public void Enter(T parent)
    {
        RunnerObject = parent;
        Enter();
    }

    protected abstract void Enter();

    // similar to Update
    public abstract void Tick(float deltaTime);

    // similar to FixedUpdate
    public abstract void FixedTick(float fixedDeltaTime);

    // here we put the conditions to change to another state if needed
    public abstract void HandleStateTransitions();

    public abstract void Exit();
}