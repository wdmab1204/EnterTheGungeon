using System;
using GameEngine.Characters.State;
using UnityEngine;

namespace DataSequence
{
    public class WalkState : UnitState
    {
        private Func<Vector2> getDirection;
        private float speed = 1;
        private float atkRange = 5;
        
        public WalkState(Func<Vector2> getDirection)
        {
            this.getDirection = getDirection;
        }
        
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void TickUpdate(float time)
        {
            Vector2 direction = getDirection();
            transform.Translate(direction * speed * time);
            if (atkRange >= direction.magnitude)
                changeState(typeof(ShootState));
        }
    }
}