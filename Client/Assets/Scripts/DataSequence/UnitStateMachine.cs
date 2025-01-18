using System;
using System.Collections.Generic;
using GameEngine.DataSequence;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.Characters.State
{
    public class UnitStateMachine
    {
        private UnitState currentState;

        private Dictionary<Type, IState> stateDict = new();

        private Transform transform;
        
        public UnitStateMachine(Transform transform)
        {
            this.transform = transform;
        }

        public void AddState(params IState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                Type type = states[i].GetType();
                if(stateDict.ContainsKey(type))
                {
                    Debug.LogError($"[StateMachine] already has state!! {type.Name}");
                    return;
                }

                ((UnitState)states[i]).SetLink(transform, ChangeState);
                
                stateDict.Add(type, states[i]);
            }
        }
        
        public void ChangeState(Type stateType)
        {
            currentState?.Exit();
            if (stateDict.TryGetValue(stateType, out var newState) == false)
            {
                Debug.LogError($"{stateType.Name}은 존재하지 않습니다");
                return;
            }
            
            newState.Enter();

            currentState = (UnitState)newState;

            Debug.Log($"[{transform.name}] current state : {currentState.GetType().Name}");
        }

        public void Update()
        {
            currentState?.TickUpdate(Time.deltaTime);
        }
        
        public void FixedUpdate()
        {
            currentState?.FixedUpdate(Time.fixedDeltaTime);
        }
    }
}