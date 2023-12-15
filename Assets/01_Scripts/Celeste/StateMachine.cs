using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class StateMachine : MonoBehaviour
{
    private Dictionary<int, StateActions> stateDict;
    private int currentStateIdx;
    
    // to avoid null check
    private Func<int> currentUpdateAction = () => 0;

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
        public IEnumerator coroutine;
    }
    
    public static StateMachine AttachStateMachine(GameObject gameObject, int? capacity = null)
    {
        var newStateMachine = gameObject.AddComponent<StateMachine>();
        newStateMachine.hideFlags = HideFlags.HideInInspector;
        newStateMachine.Initialize(capacity);
        
        return newStateMachine;
    }

    /// <summary>
    /// set capacity -> faster as no need to reallocate
    /// https://www.dotnetperls.com/capacity
    /// </summary>
    /// <param name="capacity"></param>
    private void Initialize(int? capacity)
    {
        stateDict = capacity.HasValue ? new Dictionary<int, StateActions>(capacity.Value) : 
            new Dictionary<int, StateActions>();
    }

    public void SetCallbacks(int state, Func<int> update, IEnumerator coroutine, Action begin, Action end)
    {
        RemoveState(state);
        
        stateDict.Add(state, new StateActions()
        {
            update = update,
            coroutine = coroutine,
            begin = begin,
            end = end
        });
    }

    public void RemoveState(int state)
    {
        if (stateDict.ContainsKey(state))
            stateDict.Remove(state);
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
                
                // if(prevActions.coroutine != null)
                //     StopCoroutine(prevActions.coroutine);
            }
            
            stateActions.begin?.Invoke();
            currentUpdateAction = stateActions.update;

            if (stateActions.coroutine != null)
                StartCoroutine(stateActions.coroutine);
        }
    }

    private void Update()
    {
        // if different -> state change
        State = currentUpdateAction.Invoke();
    }
}

