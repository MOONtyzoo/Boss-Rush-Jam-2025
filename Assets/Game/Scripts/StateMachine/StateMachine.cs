using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
    private List<State<T>> States;

    [Header("DEBUG")]
    [SerializeField] private bool Debug = true;

    private State<T> ActiveState;
    private State<T> PreviousState;

    private T parent;
    
    protected void InitializeStateMachine(List<State<T>> States) {
        parent = GetComponent<T>();
        this.States = States;
        if (States.Count <= 0) return;
        SetState(States[0]);
    }

    public void SetState(State<T> newState)
    {
        PreviousState = ActiveState;
        ActiveState?.Exit();
        ActiveState = newState;
        ActiveState?.Enter(parent);
        
        if (Debug) {
            print($"PlayerController: {PreviousState?.GetType().ToString() ?? "Null"} -> {ActiveState?.GetType().ToString() ?? "Null"}");
        }
    }

    public void SetState(Type newStateType)
    {
        State<T> newState = States.FirstOrDefault(s => s.GetType() == newStateType);
        if (newState != null)
        {
            SetState(newState);
        }
    }

    public State<T> GetActiveState() => ActiveState;

    protected virtual void Update()
    {
        ActiveState?.Tick(Time.deltaTime);
        ActiveState?.HandleStateTransitions();
    }

    private void FixedUpdate()
    {
        ActiveState?.FixedTick(Time.fixedDeltaTime);
    }
}