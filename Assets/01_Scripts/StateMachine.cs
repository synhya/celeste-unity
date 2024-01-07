using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StateMachine
{
    private Dictionary<int, StateActions> stateDict;
    private int currentStateIdx = -1;
    
    // to avoid null check
    private Func<int> currentUpdateAction = () => 0;

    public bool Lock = false;

    public int State
    {
        get => currentStateIdx;
        set {
            OnStateChange(value);
            currentStateIdx = value;
        }
    }

    private struct StateActions
    {
        public Action begin;
        public Action end;
        public Func<int> update;
    }
    
    public StateMachine(int? capacity = null)
    {
        stateDict = capacity.HasValue ? new Dictionary<int, StateActions>(capacity.Value) : 
            new Dictionary<int, StateActions>();
    }

    public void SetCallbacks(int state, Func<int> update, Action begin, Action end)
    {
        if(!stateDict.TryAdd(state, new StateActions
           {
               update = update,
               begin = begin,
               end = end
           })) 
            Debug.LogError("Adding Same State Twice!");
    }

    private void OnStateChange(int newState) 
    {
        if(currentStateIdx == newState) return;
        
        // 들어갈때 coroutine, update 모두 실행.
        if (stateDict.TryGetValue(newState, out var stateActions))
        {
            if (stateDict.TryGetValue(currentStateIdx, out var prevActions))
            {
                prevActions.end?.Invoke();
            }
            
            stateActions.begin?.Invoke();
            currentUpdateAction = stateActions.update;
        }
    }

    // call on callee
    public void Update()
    {
        // if different -> state change
        State = currentUpdateAction();
    }
}

