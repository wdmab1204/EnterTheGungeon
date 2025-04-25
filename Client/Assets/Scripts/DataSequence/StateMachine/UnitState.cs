using System;
using UnityEngine;

namespace GameEngine.DataSequence.StateMachine
{
    public abstract class UnitState : IState
    {
        protected Action<Type> changeState;

        protected Transform transform;

        public void SetLink(Transform transform, Action<Type> changeState)
        {
            this.transform = transform;
            this.changeState = changeState;
        }
        
        public virtual void Enter(){}
        public virtual void Exit(){}
        public virtual void TickUpdate(float time){}
        public virtual void FixedUpdate(float time){}
    }
}