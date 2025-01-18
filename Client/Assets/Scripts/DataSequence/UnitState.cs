using System;
using GameEngine.DataSequence;
using UnityEngine;

namespace GameEngine.Characters.State
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
        
        public abstract void Enter();
        public abstract void Exit();
        public abstract void TickUpdate(float time);
    }
}